using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.CustomViewer.Extensibility;
using WebBrowser = System.Windows.Forms.WebBrowser;

namespace AlarmWorkflow.Windows.UIWidget.GoogleMaps
{
    /// <summary>
    ///     Interaktionslogik für UserControl1.xaml
    /// </summary>
    [Export("MapView", typeof (IUIWidget))]
    public partial class MapView : UserControl, IUIWidget
    {
        private readonly WebBrowser _WebBrowser;
        private Operation _Operation;
        private readonly MapConfiguration _Configuration;
        public MapView()
        {
            InitializeComponent();
            _WebBrowser = new WebBrowser();
            _WebBrowser.ScrollBarsEnabled = false;
            _FormHost.Child = _WebBrowser;
            _Configuration = new MapConfiguration();
            
        }

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
            _Operation = operation;
            String html = buildHTML();
            _WebBrowser.DocumentText = html;
        }

        private string buildHTML()
        {
            string html = null;
            if (_Operation != null)
            {
                String VARIABLES =
                    "directionsDisplay = new google.maps.DirectionsRenderer();" +
                    "var zoomOnAddress = true;" +
                    "var dest = new google.maps.LatLng(0.0,0.0);" +
                    "var address = '" + _Operation.Street + " " + _Operation.StreetNumber + " " +
                    _Operation.ZipCode + " " + _Operation.City + "';" +
                    "var Home = '"+_Configuration.Home+"';" +
                    "var ZoomLevel =" + (_Configuration.ZoomLevel/100.0D).ToString(CultureInfo.InvariantCulture) + ";" +
                    "var mapType = google.maps.MapTypeId."+_Configuration.Maptype+";" +
                    "var mapOptions = {" +
                        "zoom: 10," +
                        "overviewMapControl: true," +
                        "panControl: false," +
                        "streetViewControl: false," +
                        "ZoomControl: "+_Configuration.ZoomControl.ToString().ToLower()+"," +
                        "mapTypeId: mapType" +
                    "};" +
                    "map = new google.maps.Map(document.getElementById(\"map_canvas\")," +
                                       "mapOptions);";
                const string CENTERNOCOORDINATES = "geocoder.geocode( { 'address': address}, function(results, status) {" +
                                                   "if (status == google.maps.GeocoderStatus.OK) {" +
                                                   "dest = results[0].geometry.location;" +
                                                   "var image = new google.maps.MarkerImage('http://maps.google.com/mapfiles/marker-noalpha.png'," +
                                                   "new google.maps.Size(32, 37)," +
                                                   "new google.maps.Point(0,0)," +
                                                   "new google.maps.Point(0,0));" +
                                                   "var shadow = new google.maps.MarkerImage('http://maps.google.com/mapfiles/marker-noalpha.png'," +
                                                   "new google.maps.Size(41, 22)," +
                                                   "new google.maps.Point(0,0)," +
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
                const string SHOWHOME = "geocoder.geocode( { 'address': Home}, function(results, status) {" +
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
                const string TILT = "map.setTilt(45);";
                const string TRAFFIC = "var trafficLayer = new google.maps.TrafficLayer();" +
                                       "trafficLayer.setMap(map);";
                const string SHOWROUTE = "directionsDisplay.setMap(map);" +
                                         "calcRoute(Home, address, zoomOnAddress, ZoomLevel);";
                const string ROUTEFUNC = "function calcRoute(start, end, isZoom, ZoomLevel) {" +
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
                                         
                const string BEGINNHEAD = "<!DOCTYPE html>" +
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
                const string ENDHEAD = "</script>" +
                                       "</head>";
                const string BODY = "<body onload=\"initialize()\">" +
                                    "<div id=\"map_canvas\" style=\"width:100%; height:100%\"></div>" +
                                    "</body></html>";
                var builder = new StringBuilder();
                builder.Append(BEGINNHEAD);
                builder.Append(VARIABLES);
                if (_Configuration.Route)
                {
                    builder.Append(SHOWROUTE);
                }
                else
                {
                    builder.Append(CENTERNOCOORDINATES);
                    builder.Append(SHOWHOME);
                }
                if (_Configuration.Tilt)
                    builder.Append(TILT);
                if (_Configuration.Traffic)
                    builder.Append(TRAFFIC);
                builder.Append("}");
                if (_Configuration.Route)
                    builder.Append(ROUTEFUNC);
                builder.Append(ENDHEAD);
                builder.Append(BODY);
                html = builder.ToString();
            }

            else
                html = "";
            return html;
        }
    }
}