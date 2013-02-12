using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Windows.UIContracts.Converters
{
    static class GoogleMapsProvider
    {
        internal static byte[] GetRouteImage(PropertyLocation source, PropertyLocation destination)
        {
            int width = 800;
            int height = 800;

            // https://developers.google.com/maps/documentation/directions/?hl=de

            // Create initial request
            StringBuilder sbInitialRequest = new StringBuilder();
            sbInitialRequest.Append("http://maps.google.com/maps/api/directions/xml?origin=");
            sbInitialRequest.AppendFormat("{0} {1},{2},{3}", source.Street, source.StreetNumber, source.ZipCode, source.City);
            sbInitialRequest.Append("&destination=");
            sbInitialRequest.AppendFormat("{0} {1},{2},{3}", destination.Street, destination.StreetNumber, destination.ZipCode, destination.City);
            sbInitialRequest.Append("&sensor=false");

            WebRequest wreqInitial = WebRequest.Create(sbInitialRequest.ToString());
            XDocument docResponse = null;
            using (WebResponse wresInitial = wreqInitial.GetResponse())
            {
                docResponse = XDocument.Load(wresInitial.GetResponseStream());

                // Load the response XML
                string status = docResponse.Root.Element("status").Value;
                switch (status)
                {
                    // TODO: Handle the errors!
                    case "NOT_FOUND":
                    case "ZERO_RESULTS":
                        Logger.Instance.LogFormat(LogType.Warning, typeof(GoogleMapsProvider), "The maps-request failed with status '{0}'. This is an indication that the location could not be retrieved. Sorry, but there's no workaround.", status);
                        return null;
                    case "OVER_QUERY_LIMIT":
                        Logger.Instance.LogFormat(LogType.Error, typeof(GoogleMapsProvider), "The maps-request failed with status 'OVER_QUERY_LIMIT'. This indicates too many queries within a short timeframe.");
                        return null;

                    case "MAX_WAYPOINTS_EXCEEDED":
                    case "INVALID_REQUEST":
                    case "REQUEST_DENIED":
                    case "UNKNOWN_ERROR":
                    default:
                        Logger.Instance.LogFormat(LogType.Error, typeof(GoogleMapsProvider), "The maps-request failed with status '{0}'. Please contact the developers!", status);
                        return null;

                    case "OK":
                        // Everything ok.
                        break;
                }
            }

            // Get the path data
            XElement overviewE = docResponse.Root.Element("route").Element("overview_polyline").Element("points");

            StringBuilder sbContinuationRequest = new StringBuilder();
            sbContinuationRequest.Append("http://maps.google.com/maps/api/staticmap?");
            sbContinuationRequest.AppendFormat("size={0}x{1}", width, height);
            sbContinuationRequest.Append("&sensor=false&path=weight:3|color:red|");
            sbContinuationRequest.AppendFormat("enc:{0}", overviewE.Value);

            WebRequest wr1 = WebRequest.Create(sbContinuationRequest.ToString());
            WebResponse res1 = wr1.GetResponse();

            Image image = Image.FromStream(res1.GetResponseStream());

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                return ms.ToArray();
            }
        }
    }
}
