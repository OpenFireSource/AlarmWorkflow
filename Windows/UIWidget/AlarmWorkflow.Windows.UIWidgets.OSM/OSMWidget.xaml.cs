using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Windows;
using System.Windows.Forms;
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
    public partial class OSMWidget : IUIWidget
    {
        #region Fields

        private readonly string _tempFile;
        private readonly WebBrowser _webBrowser;
        private Operation _operation;

        #endregion

        #region Constructors

        public OSMWidget()
        {
            InitializeComponent();
            _webBrowser = new WebBrowser
                              {
                                  ScrollBarsEnabled = false
                              };
            _webBrowser.FileDownload += _webBrowser_FileDownload;
            _formHost.Child = _webBrowser;
            _tempFile = Path.GetTempFileName()+".html";
            BuildHTML();
        }

        private void _webBrowser_FileDownload(object sender, EventArgs e)
        {

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

        private string BuildHTML()
        {
            string html;
            if (_operation != null)
            {
				Dictionary<String, String> result = new Dictionary<String, String>();
                result = GetGeocodes(_operation.Einsatzort.Street + " " + _operation.Einsatzort.StreetNumber + " " +
                                                                _operation.Einsatzort.ZipCode + " " + _operation.Einsatzort.City);
                if (result == null || result.Count != 2)
                {
                    return "<h2>Konnte Geocodes fuer Zielort nicht bestimmen</h2>";
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
                              "//<![CDATA[" +
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
                              "    //map.addPopup(feature.createPopup(feature.closeBox));" +
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
                              "    var lon = " + result["long"] + " ;" +
                              "    var lat = " + result["lat"] + " ;" +
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
                              "    // Position des Markers" +
                              "    addMarker(layer_markers, lon, lat);" +
                              "}" +
                              "" +
                              "//]]>" +
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