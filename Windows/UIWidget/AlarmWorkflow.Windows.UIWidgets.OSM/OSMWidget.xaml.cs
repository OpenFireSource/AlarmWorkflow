// This file is part of AlarmWorkflow.
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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Windows;
using System.Xml.XPath;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.CustomViewer.Extensibility;

namespace AlarmWorkflow.Windows.UIWidgets.OSM
{
    /// <summary>
    ///     Interaktionslogik für UserControl1.xaml
    /// </summary>
    [Export("OSMWidget", typeof(IUIWidget))]
    [Information(DisplayName = "ExportUIWidgetDisplayName", Description = "ExportUIWidgetDescription")]
    public partial class OSMWidget : IUIWidget
    {
        #region Fields

        private readonly string _osmFile;
        private Operation _operation;

        #endregion

        #region Constructors

        public OSMWidget()
        {
            InitializeComponent();
            _osmFile = Path.Combine(Path.GetTempPath(), "osm.html");
            BuildHtml();
        }

        #endregion

        #region IUIWidget Members

        bool IUIWidget.Initialize()
        {
            return true;
        }

        void IUIWidget.OnOperationChange(Operation operation)
        {
            if (operation == null)
            {
                return;
            }
            _operation = operation;
            String html = BuildHtml();
            File.WriteAllText(_osmFile, html);
            _webBrowser.Navigate(_osmFile);
        }

        UIElement IUIWidget.UIElement
        {
            get { return this; }
        }

        string IUIWidget.ContentGuid
        {
            get { return "E70E128B-6A6C-4B9F-9D5A-83360BC52F8C"; }
        }

        string IUIWidget.Title
        {
            get { return "OSM-MAP"; }
        }

        #endregion

        #region Methods

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
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
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
                Logger.Instance.LogFormat(LogType.Error, typeof(OSMWidget), "Could not retrieve geocode for address '{0}'.", address);
                Logger.Instance.LogException(typeof(OSMWidget), ex);
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

        private string BuildHtml()
        {
            string html;
            if (_operation != null)
            {
                 if (!_operation.Einsatzort.HasGeoCoordinates)
                {
                    return "<h2>Konnte Geocodes fuer Zielort nicht bestimmen! Ggf. ist der Geocoding Job nicht aktiv?</h2>";
                }
                html = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">" +
                              "<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"de\" lang=\"de-de\">" +
                              "<head>" +
                              "<meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\" />" +
                              "<meta http-equiv=\"content-script-type\" content=\"text/javascript\" />" +
                              "<meta http-equiv=\"content-style-type\" content=\"text/css\" />" +
                              "<meta http-equiv=\"content-language\" content=\"de\" />" +
                              "<style type=\"text/css\">" +
                              "html { height: 100% } " +
                              "body { height: 100%; margin: 0; padding: 0 } " +
                              "#map { height: 100% } " +
                              "</style>" +
                              "<script type=\"text/javascript\" src=\"http://www.openlayers.org/api/OpenLayers.js\"></script>" +
                              "<script type=\"text/javascript\" src=\"http://www.openstreetmap.org/openlayers/OpenStreetMap.js\"></script>" +
                              " " +
                              "<script type=\"text/javascript\">" +
                            
                              "" +
                              "var map;" +
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
                              "}" +
                              "function drawmap() {    " +
                              "    OpenLayers.Lang.setCode('de');    " +
                              "    var lon = " + _operation.Einsatzort.GeoLongitude + " ;" +
                              "    var lat = " + _operation.Einsatzort.GeoLatitude + " ;" +
                              "    var zoom = 18;" +
                              "    map = new OpenLayers.Map('map', {" +
                              "        projection: new OpenLayers.Projection(\"EPSG:900913\")," +
                              "        displayProjection: new OpenLayers.Projection(\"EPSG:4326\")," +
                              "        controls: [" +
                              "            new OpenLayers.Control.Navigation()," +
                              "            new OpenLayers.Control.LayerSwitcher()," +
                              "            new OpenLayers.Control.PanZoomBar()]," +
                              "        maxExtent:" +
                              "            new OpenLayers.Bounds(-20037508.34,-20037508.34," +
                              "                                    20037508.34, 20037508.34)," +
                              "        numZoomLevels: 18," +
                              "        maxResolution: 156543," +
                              "        units: 'meters'" +
                              "    });" +
                              "    layer_mapnik = new OpenLayers.Layer.OSM.Mapnik(\"Mapnik\");" +
                              "    layer_markers = new OpenLayers.Layer.Markers(\"Address\", { projection: new OpenLayers.Projection(\"EPSG:4326\"), " +
                              "    	                                          visibility: true, displayInLayerSwitcher: false });" +
                              "    map.addLayers([layer_mapnik, layer_markers]);" +
                              "    jumpTo(lon, lat, zoom); " +
                              "    addMarker(layer_markers, lon, lat);" +
                              "}" +
                              "" +
                              "    </script>" +
                              "</head>" +
                              "<body onload=\"drawmap();\">  " +
                              "  <div id=\"map\">" +
                              "  </div>  " +
                              "</body>" +
                              "</html>";
                
            }
            else
            {
                html = "";
                
            }
            return html;

        }

        #endregion
    }
}