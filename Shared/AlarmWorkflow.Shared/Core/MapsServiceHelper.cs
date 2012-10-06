using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Provides functionality to retrieve the maps data from the internet.
    /// </summary>
    public static class MapsServiceHelper
    {
        /// <summary>
        /// Connects to Google Maps and queries an image displaying the route from the given start to finish position.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>When using this method, make sure that the <paramref name="source"/> and <paramref name="destination"/>-parameters contain meaningful data!
        /// You can check this by calling <see cref="PropertyLocation.IsMeaningful"/> to see if the data is meaningful enough to return a good result.</remarks>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="width">The desired width of the image. A value of '800' is recommended.</param>
        /// <param name="height">The desired height of the image. A value of '800' is recommended.</param>
        /// <returns>The resulting PNG-image as a buffer.</returns>
        public static byte[] GetRouteImage(PropertyLocation source, PropertyLocation destination, int width, int height)
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
                switch (status)
                {
                    default:
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

            // Save the image as PNG
            using (MemoryStream ms = new MemoryStream())
            {
                Image image = System.Drawing.Image.FromStream(res1.GetResponseStream());

                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}
