using System.Net;
using System.Text;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace AlarmWorkflow.Windows.UI.Models
{
    /// <summary>
    /// Provides functionality to retrieve the maps data from the internet.
    /// </summary>
    static class MapsServiceHelper
    {
        internal static BitmapImage GetRouteImage(PropertyLocation source, PropertyLocation destination, int width, int height)
        {
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
                // TODO: Read "status" element!
                string status = docResponse.Root.Element("status").Value;
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

            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = res1.GetResponseStream();
            img.EndInit();

            return img;
        }
    }
}
