#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Forms;
using System.Xml.XPath;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.CustomViewer.Extensibility;

#endregion

namespace AlarmWorkflow.Windows.UIWidgets.GoogleMaps
{
    /// <summary>
    ///     Interaktionslogik für UserControl1.xaml
    /// </summary>
    [Export("GoogleMapsWidget", typeof (IUIWidget))]
    public partial class GoogleMapsWidget : IUIWidget
    {
        #region Fields

        private readonly MapConfiguration _configuration;
        private readonly string _tempFile;
        private readonly WebBrowser _webBrowser;
        private Operation _operation;

        #endregion Fields

        #region Constants

        private const string CennerNoCoord = "geocoder.geocode( { 'address': address}, function(results, status) {" +
                                             "if (status == google.maps.GeocoderStatus.OK) {" +
                                             "dest = results[0].geometry.location;" +
                                             "var beachMarker = new google.maps.Marker({" +
                                             "position: dest," +
                                             "map: map" +
                                             "});" +
                                             "map.setCenter(dest);" +
                                             "maxZoomService.getMaxZoomAtLatLng(dest, function(response) {" +
                                             "if (response.status == google.maps.MaxZoomStatus.OK) {" +
                                             "var zoom = Math.round(response.zoom * ZoomLevel);" +
                                             "map.setZoom(zoom);" +
                                             "}" +
                                             "});" +
                                             "}" +
                                             "});";

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

        private const string ShowHome = "geocoder.geocode( { 'address': Home}, function(results, status) {" +
                                        "if (status == google.maps.GeocoderStatus.OK) {" +
                                        "var homeCoor = results[0].geometry.location;" +
                                        "var beachMarker = new google.maps.Marker({" +
                                        "position: homeCoor," +
                                        "map: map" +
                                        "});" +
                                        "}" +
                                        "});";

        private const string Tilt = "map.setTilt(45);";

        private const string Traffic = "var trafficLayer = new google.maps.TrafficLayer();" +
                                       "trafficLayer.setMap(map);";

        private const string Showroute = "directionsDisplay.setMap(map);" +
                                         "calcRoute(Home, address, zoomOnAddress, ZoomLevel);";

        private const string RouteFunc = "function calcRoute(start, end, isZoom, ZoomLevel) {" +
                                         "var request = {" +
                                         "origin:start," +
                                         "destination:end," +
                                         "travelMode: google.maps.TravelMode.DRIVING" +
                                         "};" +
                                         "directionsService.route(request, function(result, status) {" +
                                         "if (status == google.maps.DirectionsStatus.OK) {" +
                                         "directionsDisplay.setDirections(result);" +
                                         "if (isZoom){" +
                                         "geocoder.geocode( { 'address': end}, function(results, status) {" +
                                         "if (status == google.maps.GeocoderStatus.OK) {" +
                                         "var coor = results[0].geometry.location;" +
                                         "map.setCenter(coor);" +
                                         "maxZoomService.getMaxZoomAtLatLng(coor, function(response) {" +
                                         "if (response.status == google.maps.MaxZoomStatus.OK) {" +
                                         "var zoom = Math.round(response.zoom * ZoomLevel);" +
                                         "map.setZoom(zoom);" +
                                         "}" +
                                         "});" +
                                         "}" +
                                         "});" +
                                         "}" +
                                         "}" +
                                         "});" +
                                         "}";

        private const string BeginnHead = "<!DOCTYPE html>" +
                                          "<html><head><meta name=\"viewport\" content=\"initial-scale=1.0, user-scalable=no\" /><style type=\"text/css\">" +
                                          "html { height: 100% } body { height: 100%; margin: 0; padding: 0 } #map_canvas { height: 100% }" +
                                          "</style><script type=\"text/javascript\"" +
                                          "src=\"https://maps.googleapis.com/maps/api/js?sensor=true\"></script>" +
                                          "<script type=\"text/javascript\">" +
                                          "var directionsService = new google.maps.DirectionsService();" +
                                          "var directionsDisplay = new google.maps.DirectionsRenderer();" +
                                          "var map;" +
                                          "var maxZoomService = new google.maps.MaxZoomService();" +
                                          "var geocoder = new google.maps.Geocoder();" +
                                          "function initialize() {";

        private const string EndHead = "</script>" +
                                       "</head>";

        private const string Body = "<body onload=\"initialize()\">" +
                                    "<div id=\"map_canvas\" style=\"width:100%; height:100%\"></div>" +
                                    "</body></html>";

        #endregion Constants

        #region Constructors

        public GoogleMapsWidget()
        {
            InitializeComponent();
            _webBrowser = new WebBrowser
                              {
                                  ScrollBarsEnabled = false
                              };
            _FormHost.Child = _webBrowser;
            _configuration = new MapConfiguration();
            _tempFile = Path.GetTempFileName();
            BuildHTML();
        }

        #endregion Constructors

        #region IUIWidget Members

        bool IUIWidget.Initialize()
        {
            return true;
        }

        UIElement IUIWidget.UIElement
        {
            get { return this; }
        }

        string IUIWidget.ContentGuid
        {
            get { return "9203FBB1-1464-4D7F-8B59-BDB8847B361C"; }
        }

        string IUIWidget.Title
        {
            get { return "Google Maps"; }
        }

        void IUIWidget.OnOperationChange(Operation operation)
        {
            if (operation == null)
            {
                if (!String.IsNullOrWhiteSpace(_tempFile))
                {
                    File.Delete(_tempFile);
                }
                return;
            }
            _operation = operation;
            String html = BuildHTML();
            File.WriteAllText(_tempFile, html);
            _webBrowser.Navigate(_tempFile);
        }

        #endregion IUIWidget Members

        #region Methods

        private string BuildHTML()
        {
            string html;
            if (_operation != null)
            {
                StringBuilder builder = new StringBuilder();
                Dictionary<String, String> result = GetGeocodes(_operation.Einsatzort.Street + " " + _operation.Einsatzort.StreetNumber + " " +
                                                                _operation.Einsatzort.ZipCode + " " + _operation.Einsatzort.City);
                if (result.Count != 2)
                {
                    return "";
                }
                String longitute = result["long"];
                String latitude = result["lat"];
                String variables =
                    "directionsDisplay = new google.maps.DirectionsRenderer();" +
                    "var zoomOnAddress = true;" +
                    "var dest = new google.maps.LatLng(" + latitude + "," + longitute + ");" +
                    "var address = '" + _operation.Einsatzort.Street + " " + _operation.Einsatzort.StreetNumber + " " +
                    _operation.Einsatzort.ZipCode + " " + _operation.Einsatzort.City + "';" +
                    "var Home = '" + _configuration.Home + "';" +
                    "var ZoomLevel =" + (_configuration.ZoomLevel/100.0D).ToString(CultureInfo.InvariantCulture) + ";" +
                    "var mapType = google.maps.MapTypeId." + _configuration.Maptype + ";" +
                    "var mapOptions = {" +
                    "zoom: 10," +
                    "overviewMapControl: true," +
                    "panControl: false," +
                    "streetViewControl: false," +
                    "ZoomControl: " + _configuration.ZoomControl.ToString().ToLower() + "," +
                    "mapTypeId: mapType" +
                    "};" +
                    "map = new google.maps.Map(document.getElementById(\"map_canvas\")," +
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
                builder.Append(EndHead);
                builder.Append(Body);
                html = builder.ToString();
            }

            else
            {
                html = "";
            }
            return html;
        }

        /// <summary>
        ///     Returns the longitude and the latitude for a given address
        /// </summary>
        /// <param name="address">Address to search for</param>
        /// <returns>null or dictonary</returns>
        internal static Dictionary<string, string> GetGeocodes(string address)
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
                XPathDocument document = new XPathDocument(response.GetResponseStream());
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
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, typeof (GoogleMapsWidget), "Could not retrieve geocode for address '{0}'.", address);
                Logger.Instance.LogException(typeof (GoogleMapsWidget), ex);
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }

            return geocodes;
        }

        #endregion Methods
    }
}