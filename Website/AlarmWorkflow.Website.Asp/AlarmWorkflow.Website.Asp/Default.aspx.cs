using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Xml.XPath;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.ServiceContracts;
using AlarmWorkflow.Windows.UI.Models;

namespace AlarmWorkflow.Website.Asp
{
    /// <summary>
    /// Logic of the Default-page.
    /// </summary>
    public partial class Default : Page
    {
        #region Fields

        private readonly WebsiteConfiguration _configuration;
        protected String JSScripts;
        protected String OSMCode;

        #endregion

        #region Constructors

        public Default()
        {
            _configuration = WebsiteConfiguration.Instance;
            if (_UpdateTimer != null)
            {
                _UpdateTimer.Interval = WebsiteConfiguration.Instance.UpdateIntervall;
            }
        }

        #endregion

        #region Constants

        private const string OSMHead = "var map;" +
                                       "var layer_mapnik;" +
                                       "var layer_tah;" +
                                       "var layer_markers;" +
                                       "function jumpTo(lon, lat, zoom) {" +
                                       "    var x = Lon2Merc(lon);" +
                                       "    var y = Lat2Merc(lat);" +
                                       "    map.setCenter(new OpenLayers.LonLat(x, y), zoom);" +
                                       "    return false;" +
                                       "}" +
                                       " " +
                                       "function Lon2Merc(lon) {" +
                                       "    return 20037508.34 * lon / 180;" +
                                       "}" +
                                       " " +
                                       "function Lat2Merc(lat) {" +
                                       "    var PI = 3.14159265358979323846;" +
                                       "    lat = Math.log(Math.tan( (90 + lat) * PI / 360)) / (PI / 180);" +
                                       "    return 20037508.34 * lat / 180;" +
                                       "}" +
                                       " " +
                                       "function addMarker(layer, lon, lat) {" +
                                       " " +
                                       "    var ll = new OpenLayers.LonLat(Lon2Merc(lon), Lat2Merc(lat));    " +
                                       " " +
                                       "    var marker = new OpenLayers.Marker(ll); " +
                                       "    layer.addMarker(marker);" +
                                       "}" +
                                       " " +
                                       "function getCycleTileURL(bounds) {" +
                                       "   var res = this.map.getResolution();" +
                                       "   var x = Math.round((bounds.left - this.maxExtent.left) / (res * this.tileSize.w));" +
                                       "   var y = Math.round((this.maxExtent.top - bounds.top) / (res * this.tileSize.h));" +
                                       "   var z = this.map.getZoom();" +
                                       "   var limit = Math.pow(2, z);" +
                                       " " +
                                       "   if (y < 0 || y >= limit)" +
                                       "   {" +
                                       "     return null;" +
                                       "   }" +
                                       "   else" +
                                       "   {" +
                                       "     x = ((x % limit) + limit) % limit;" +
                                       " " +
                                       "     return this.url + z + \"/\" + x + \"/\" + y + \".\" + this.type;" +
                                       "   }" +
                                       "}";

        private const string Tilt = "map.setTilt(45);";

        private const string Traffic = "var trafficLayer = new google.maps.TrafficLayer();" +
                                       "trafficLayer.setMap(map);";

        private const string Showroute = "directionsDisplay.setMap(map);" +
                                         "calcRoute(Home, address);";

        private const string RouteFunc = "function calcRoute(start, end) {" +
                                         "var request = {" +
                                         "origin:start," +
                                         "destination:end," +
                                         "travelMode: google.maps.TravelMode.DRIVING" +
                                         "};" +
                                         "directionsService.route(request, function(result, status) {" +
                                         "if (status == google.maps.DirectionsStatus.OK) {" +
                                         "directionsDisplay.setDirections(result);" +
                                         "}" +
                                         "});" +
                                         "}";

        private const string CenterCoord = "var beachMarker = new google.maps.Marker({" +
                                           "position: dest," +
                                           "map: map" +
                                           "});" +
                                           "map.setCenter(dest);" +
                                           "maxZoomService.getMaxZoomAtLatLng(dest, function(response) {" +
                                           "if (response.status == google.maps.MaxZoomStatus.OK) {" +
                                           "var zoom = Math.round(response.zoom * ZoomLevel);" +
                                           "map.setZoom(zoom);" +
                                           "}" +
                                           "});";

        private const string BeginnHead = "var directionsService = new google.maps.DirectionsService();" +
                                          "var directionsDisplay = new google.maps.DirectionsRenderer();" +
                                          "var map;" +
                                          "var maxZoomService = new google.maps.MaxZoomService();" +
                                          "var geocoder = new google.maps.Geocoder();" +
                                          "function initialize() {";

        #endregion Constants

        #region Methods

        private void CheckForUpdate()
        {
            Operation operation;
            if (!TryGetLatestOperation(out operation))
            {
                RedirectToErrorPage();
            }
            else
            {
                if (operation == null)
                {
                    RedirectToNoAlarm();
                }
                else
                {
                    if (operation.Id.ToString(CultureInfo.InvariantCulture) == HttpContext.Current.Request["id"])
                    {
                    }
                    else
                    {
                        Response.Redirect("Default.aspx?id=" + operation.Id);
                    }
                }
            }
        }

        private void RedirectToNoAlarm()
        {
            Response.Redirect("Idle.aspx");
        }

        private void RedirectToErrorPage()
        {
            Response.Redirect("Error.aspx");
        }

        private bool TryGetLatestOperation(out Operation operation)
        {
            int maxAgeInMinutes = WebsiteConfiguration.Instance.MaxAge;
            bool onlyNonAcknowledged = WebsiteConfiguration.Instance.NonAcknowledgedOnly;
            // For the moment, we are only interested about the latest operation (if any).
            const int limitAmount = 1;

            operation = null;

            try
            {
                using (WrappedService<IAlarmWorkflowServiceInternal> service = InternalServiceProxy.GetServiceInstance())
                {
                    if (service.IsFaulted)
                    {
                        return false;
                    }
                    IList<int> ids = service.Instance.GetOperationIds(maxAgeInMinutes, onlyNonAcknowledged, limitAmount);
                    if (ids.Count > 0)
                    {
                        // Retrieve the operation with full detail to allow us to access the route image
                        OperationItem operationItem = service.Instance.GetOperationById(ids[0], OperationItemDetailLevel.Full);
                        operation = operationItem.ToOperation();
                    }
                    return true;
                }
            }
            catch (EndpointNotFoundException)
            {
                RedirectToErrorPage();
            }
            return false;
        }


        private void SetAlarmDisplay()
        {
            Operation operation;
            GetOperation(Request["id"], out operation);
            SetAlarmContent(operation);
            Dictionary<string, string> result = GetGeocodes(operation.Einsatzort.Street + " " + operation.Einsatzort.StreetNumber + " " +
                                                            operation.Einsatzort.ZipCode + " " + operation.Einsatzort.City);
            if (result == null || result.Count != 2)
            {
                trMap.Visible = false;
            }
            else
            {
                JSScripts = GoogleMaps(operation, result);
                JSScripts += OSM(result);
            }
        }

        private void SetAlarmContent(Operation operation)
        {
            DebugLabel.ForeColor = Color.Red;
            DebugLabel.Text = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            DebugLabel.Text += " - " + operation.OperationGuid.ToString();
            DebugLabel.Text += " - " + operation.Id;
            DebugLabel.Text += " - " + operation.IsAcknowledged;
            lbPicture.Text = operation.Picture;
            lbOther.Text = operation.Comment + " " + operation.OperationPlan + " " + operation.Keywords;
            lbAddress.Text = operation.Einsatzort.Street + " " + operation.Einsatzort.StreetNumber + " " + operation.Einsatzort.ZipCode + " " + operation.Einsatzort.City;
            lbObject.Text = operation.Einsatzort.Property;
            lbResources.Text = operation.Resources.ToString();
        }

        private void GetOperation(string id, out Operation operation)
        {
            operation = null;

            try
            {
                using (WrappedService<IAlarmWorkflowServiceInternal> service = InternalServiceProxy.GetServiceInstance())
                {
                    OperationItem operationItem = service.Instance.GetOperationById(int.Parse(id), OperationItemDetailLevel.Full);
                    operation = operationItem.ToOperation();
                }
            }
            catch (EndpointNotFoundException)
            {
                RedirectToErrorPage();
            }
        }

        private Dictionary<string, string> GetGeocodes(string address)
        {
            Dictionary<string, string> geocodes = new Dictionary<string, string>();
            string urladdress = HttpUtility.UrlEncode(address);
            string url = "http://maps.googleapis.com/maps/api/geocode/xml?address=" + urladdress + "&sensor=false";

            WebResponse response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
                request.Method = "GET";
                response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        XPathDocument document = new XPathDocument(stream);
                        XPathNavigator navigator = document.CreateNavigator();

                        // get response status
                        XPathNodeIterator statusIterator = navigator.Select("/GeocodeResponse/status");
                        while (statusIterator.MoveNext())
                        {
                            if (statusIterator.Current.Value != "OK")
                            {
                                return null;
                            }
                        }

                        // gets first restult
                        XPathNodeIterator resultIterator = navigator.Select("/GeocodeResponse/result");
                        resultIterator.MoveNext();
                        XPathNodeIterator geometryIterator = resultIterator.Current.Select("geometry");
                        geometryIterator.MoveNext();
                        XPathNodeIterator locationIterator = geometryIterator.Current.Select("location");
                        while (locationIterator.MoveNext())
                        {
                            XPathNodeIterator latIterator = locationIterator.Current.Select("lat");
                            while (latIterator.MoveNext())
                            {
                                geocodes.Add("lat", latIterator.Current.Value);
                            }
                            XPathNodeIterator lngIterator = locationIterator.Current.Select("lng");
                            while (lngIterator.MoveNext())
                            {
                                geocodes.Add("long", lngIterator.Current.Value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, typeof (Default), "Could not retrieve geocode for address '{0}'.", address);
                Logger.Instance.LogException(typeof (Default), ex);
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            return geocodes;
        }

        private string OSM(Dictionary<string, string> result)
        {
            OSMCode = "    OpenLayers.Lang.setCode('de');    " +
                      "    var lon = " + result["long"] + " ;" +
                      "    var lat = " + result["lat"] + " ;" +
                      "    var zoom = 16;" +
                      "    map = new OpenLayers.Map('osmmap', {" +
                      "        projection: new OpenLayers.Projection(\"EPSG:900913\")," +
                      "        displayProjection: new OpenLayers.Projection(\"EPSG:4326\")," +
                      "        controls: [" +
                      "            new OpenLayers.Control.Navigation()," +
                      "            new OpenLayers.Control.LayerSwitcher()," +
                      "            new OpenLayers.Control.PanZoomBar()]," +
                      "        maxExtent:" +
                      "            new OpenLayers.Bounds(-20037508.34,-20037508.34," +
                      "                                    20037508.34, 20037508.34)," +
                      "        numZoomLevels: 16," +
                      "        maxResolution: 156543," +
                      "        units: 'meters'" +
                      "    });" +
                      "    layer_mapnik = new OpenLayers.Layer.OSM.Mapnik(\"Mapnik\");" +
                      "    layer_markers = new OpenLayers.Layer.Markers(\"Address\", { projection: new OpenLayers.Projection(\"EPSG:4326\"), " +
                      "    	                                          visibility: true, displayInLayerSwitcher: false });" +
                      "    map.addLayers([layer_mapnik, layer_markers]);" +
                      "    jumpTo(lon, lat, zoom); " +
                      "    addMarker(layer_markers, lon, lat);";
            return OSMHead;
        }

        private string GoogleMaps(Operation operation, Dictionary<string, string> result)
        {
            StringBuilder builder = new StringBuilder();
            String longitute = result["long"];
            String latitude = result["lat"];
            String variables =
                "directionsDisplay = new google.maps.DirectionsRenderer();" +
                "var zoomOnAddress = true;" +
                "var dest = new google.maps.LatLng(" + latitude + "," + longitute + ");" +
                "var address = '" + operation.Einsatzort.Street + " " + operation.Einsatzort.StreetNumber + " " +
                operation.Einsatzort.ZipCode + " " + operation.Einsatzort.City + "';" +
                "var Home = '" + _configuration.Home + "';" +
                "var ZoomLevel =" + (_configuration.GoogleZoomLevel/100.0D).ToString(CultureInfo.InvariantCulture) + ";" +
                "var mapType = google.maps.MapTypeId." + _configuration.Maptype + ";" +
                "var mapOptions = {" +
                "zoom: 10," +
                "overviewMapControl: true," +
                "panControl: false," +
                "streetViewControl: false," +
                "ZoomControl: " + _configuration.ZoomControl.ToString().ToLower() + "," +
                "mapTypeId: mapType" +
                "};" +
                "map = new google.maps.Map(document.getElementById(\"googlemap\")," +
                "mapOptions);";

            builder.Append(BeginnHead);
            builder.Append(variables);
            builder.Append(_configuration.Route ? Showroute : CenterCoord);
            if (_configuration.Tilt)
            {
                builder.Append(Tilt);
            }
            if (_configuration.Traffic)
            {
                builder.Append(Traffic);
            }
            builder.Append("}");
            if (_configuration.Route)
            {
                builder.Append(RouteFunc);
            }
            builder.Append("google.maps.event.addDomListener(window, \"load\", initialize);");
            return builder.ToString();
        }

        #endregion

        #region Event handlers

        protected void UpdateTimer_Tick(object sender, EventArgs e)
        {
            CheckForUpdate();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs" /> object that contains the event data.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // This page should not feature any postbacks (like the result from the user clicking on a link, button or such).
            if (IsPostBack)
            {
                return;
            }

            if (String.IsNullOrWhiteSpace(Request["id"]))
            {
                CheckForUpdate();
            }
            else
            {
                SetAlarmDisplay();
            }
        }

        #endregion
    }
}