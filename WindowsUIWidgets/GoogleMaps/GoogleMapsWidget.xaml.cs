﻿// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.CustomViewer.Extensibility;

namespace AlarmWorkflow.Windows.UIWidgets.GoogleMaps
{
    /// <summary>
    ///     Interaktionslogik für UserControl1.xaml
    /// </summary>
    [Export("GoogleMapsWidget", typeof(IUIWidget))]
    [Information(DisplayName = "ExportUIWidgetDisplayName", Description = "ExportUIWidgetDescription")]
    public partial class GoogleMapsWidget : IUIWidget
    {
        #region Fields

        private readonly MapConfiguration _configuration;
        private readonly string _googleFile;
        private Operation _operation;

        #endregion Fields

        #region Constants

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

        private const string Tilt = "map.setTilt(45);";

        private const string Traffic = "var trafficLayer = new google.maps.TrafficLayer();" +
                                       "trafficLayer.setMap(map);";

        private const string Showroute = "directionsDisplay.setMap(map);" +
                                         "calcRoute(home, dest, zoomOnAddress, ZoomLevel);";

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
                                         "maxZoomService.getMaxZoomAtLatLng(end, function(response) {" +
                                         "if (response.status == google.maps.MaxZoomStatus.OK) {" +
                                         "var zoom = Math.round(response.zoom * ZoomLevel);" +
                                         "map.setZoom(zoom);" +
                                         "map.setCenter(end);" +
                                         "}" +
                                         "});" +
                                         "}" +
                                         "}" +
                                         "});" +
                                         "}";

        private const string BeginHead = "<!DOCTYPE html>" +
                                          "<html><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" >" +
                                          "<meta http-equiv='Content-Type' content='text/html;charset=UTF-8'>" +
                                          "<meta name=\"viewport\" content=\"initial-scale=1.0, user-scalable=no\" /><style type=\"text/css\">" +
                                          "html { height: 100% } body { height: 100%; margin: 0; padding: 0 } #map_canvas { height: 100% }" +
                                          "</style><script type=\"text/javascript\" src=\"https://maps.googleapis.com/maps/api/js?key={KEY}\"></script>" +
                                          "<script type=\"text/javascript\">" +
                                          "var directionsService = new google.maps.DirectionsService();" +
                                          "var directionsDisplay = new google.maps.DirectionsRenderer();" +
                                          "var map;" +
                                          "var maxZoomService = new google.maps.MaxZoomService();" +
                                          "var geocoder = new google.maps.Geocoder();" +
                                          "function initialize() { window.onerror = function(){return true;};";

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
            _googleFile = Path.Combine(Path.GetTempPath(), "google.html");
            _configuration = new MapConfiguration();

        }

        #endregion Constructors

        #region IUIWidget Members

        void IUIWidget.Close()
        {
        }

        bool IUIWidget.Initialize()
        {
            return true;
        }

        UIElement IUIWidget.UIElement => this;

        string IUIWidget.ContentGuid => "9203FBB1-1464-4D7F-8B59-BDB8847B361C";

        string IUIWidget.Title => "Google Maps";

        void IUIWidget.OnOperationChange(Operation operation)
        {
            if (operation == null)
            {
                return;
            }
            _operation = operation;
            string html = BuildHtml();
            File.WriteAllText(_googleFile, html);
            _webBrowser.Navigate(_googleFile);
        }

        #endregion IUIWidget Members

        #region Methods

        private string BuildHtml()
        {
            if (_operation != null)
            {
                StringBuilder builder = new StringBuilder();
                if (!_operation.Einsatzort.HasGeoCoordinates)
                {
                    return "<h2>Konnte Geocodes fuer Zielort nicht bestimmen! Ggf. ist der Geocoding Job nicht aktiv?</h2>";
                }
                string variables =
                    "directionsDisplay = new google.maps.DirectionsRenderer();" +
                    "var zoomOnAddress = " + _configuration.ZoomOnAddress.ToString().ToLower() + ";" +
                    "var dest = new google.maps.LatLng(" + _operation.Einsatzort.GeoLatitudeString + "," + _operation.Einsatzort.GeoLongitudeString + ");" +
                    "var home = '" + _configuration.Home + "';" +
                    "var ZoomLevel =" + (_configuration.ZoomLevel / 100.0D).ToString(CultureInfo.InvariantCulture) + ";" +
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

                builder.AppendLine(BeginHead.Replace("{KEY}", _configuration.GoogleMapsKey));
                builder.AppendLine(variables);
                builder.AppendLine(_configuration.Route ? Showroute : CenterCoord);
                if (_configuration.Tilt)
                {
                    builder.AppendLine(Tilt);
                }
                if (_configuration.Traffic)
                {
                    builder.AppendLine(Traffic);
                }

                builder.AppendLine("}");
                if (_configuration.Route)
                {
                    builder.AppendLine(RouteFunc);
                }
                builder.AppendLine(EndHead);
                builder.AppendLine(Body);
                return builder.ToString();
               
            }

            return string.Empty;
        }

        #endregion Methods
    }
}