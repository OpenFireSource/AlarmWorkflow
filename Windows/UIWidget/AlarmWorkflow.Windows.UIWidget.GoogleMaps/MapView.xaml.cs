using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.CustomViewer.Extensibility;
using UserControl = System.Windows.Controls.UserControl;
using WebBrowser = System.Windows.Forms.WebBrowser;

namespace AlarmWorkflow.Windows.UIWidget.GoogleMaps
{
    /// <summary>
    ///     Interaktionslogik für UserControl1.xaml
    /// </summary>
    [Export("MapView", typeof(IUIWidget))]
    public partial class MapView : UserControl, IUIWidget
    {
        #region Fields

        private readonly WebBrowser _WebBrowser;
        private Operation _Operation;
        private readonly MapConfiguration _Configuration;
        private string _TempFile = null;

        #endregion

        #region Constants

        private const string CennerNoCord = "geocoder.geocode( { 'address': address}, function(results, status) {" +
            "if (status == google.maps.GeocoderStatus.OK) {" +
            "dest = results[0].geometry.location;" +
            "var image = new google.maps.MarkerImage('http://maps.google.com/mapfiles/marker-noalpha.png'," +
            "new google.maps.Size(32, 37)," +
            "new google.maps.Point(0,0)," +
            "new google.maps.Point(0,0));" +
            "var shadow = new google.maps.MarkerImage('http://maps.google.com/mapfiles/marker-noalpha.png'," +
            "new google.maps.Size(41, 22)," + "new google.maps.Point(0,0)," +
            "new google.maps.Point(-10, -10));" +
            "var beachMarker = new google.maps.Marker({" +
            "position: dest," +
            "map: map," +
            "shadow: shadow," +
            "icon: image" +
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

        /**
        String CENTERCOORDINATES = "var image = 'http://maps.google.com/mapfiles/marker-noalpha.png';" +
                                   "var beachMarker = new google.maps.Marker({" +
                                   "position: dest," +
                                   "map: map," +
                                   "icon: image" +
                                   "});" +
                                   "map.setCenter(dest);" +
                                   "maxZoomService.getMaxZoomAtLatLng(dest, function(response) {" +
                                   "if (response.status == google.maps.MaxZoomStatus.OK) {" +
                                   "var zoom = Math.round(response.zoom * ZoomLevel);" +
                                   "map.setZoom(zoom);" +
                                   "}" +
                                   "});";
         */

        private const string ShowHome = "geocoder.geocode( { 'address': Home}, function(results, status) {" +
                                "if (status == google.maps.GeocoderStatus.OK) {" +
                                "var homeCoor = results[0].geometry.location;" +
                                "var image = 'http://maps.google.com/mapfiles/marker-noalpha.png';" +
                                "var beachMarker = new google.maps.Marker({" +
                                "position: homeCoor," +
                                "map: map," +
                                "icon: image" +
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

        #endregion

        #region Constructors

        public MapView()
        {
            InitializeComponent();
            _WebBrowser = new WebBrowser
                              {
                                  ScrollBarsEnabled = false
                              };
            _FormHost.Child = _WebBrowser;
            _Configuration = new MapConfiguration();
            _TempFile = Path.GetTempFileName();
            BuildHTML();
        }
        #endregion

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
                if (!String.IsNullOrWhiteSpace(_TempFile))
                {
                    File.Delete(_TempFile);
                }
                return;
            }
            _Operation = operation;
            String html = BuildHTML();
            File.WriteAllText(_TempFile, html);
            _WebBrowser.Navigate(_TempFile);
           
        }

        #endregion

        #region Methods
        private string BuildHTML()
        {
            string _HTML = null;
            if (_Operation != null)
            {
                String VARIABLES =
                    "directionsDisplay = new google.maps.DirectionsRenderer();" +
                    "var zoomOnAddress = true;" +
                    "var dest = new google.maps.LatLng(0.0,0.0);" +
                    "var address = '" + _Operation.Street + " " + _Operation.StreetNumber + " " +
                    _Operation.ZipCode + " " + _Operation.City + "';" +
                    "var Home = '" + _Configuration.Home + "';" +
                    "var ZoomLevel =" + (_Configuration.ZoomLevel / 100.0D).ToString(CultureInfo.InvariantCulture) + ";" +
                    "var mapType = google.maps.MapTypeId." + _Configuration.Maptype + ";" +
                    "var mapOptions = {" +
                        "zoom: 10," +
                        "overviewMapControl: true," +
                        "panControl: false," +
                        "streetViewControl: false," +
                        "ZoomControl: " + _Configuration.ZoomControl.ToString().ToLower() + "," +
                        "mapTypeId: mapType" +
                    "};" +
                    "map = new google.maps.Map(document.getElementById(\"map_canvas\")," +
                                       "mapOptions);";

                StringBuilder builder = new StringBuilder();
                builder.Append(BeginnHead);
                builder.Append(VARIABLES);
                if (_Configuration.Route)
                {
                    builder.Append(Showroute);
                }
                else
                {
                    builder.Append(CennerNoCord);
                    builder.Append(ShowHome);
                }
                if (_Configuration.Tilt)
                { builder.Append(Tilt); }
                if (_Configuration.Traffic)
                { builder.Append(Traffic); }
                builder.Append("}");
                if (_Configuration.Route)
                { builder.Append(RouteFunc); }
                builder.Append(EndHead);
                builder.Append(Body);
                _HTML = builder.ToString();
            }

            else
                _HTML = "";
            return _HTML;
        }
        #endregion
    }
}