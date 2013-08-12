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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using AlarmWorkflow.Job.OperationPrinter.Properties;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Job.OperationPrinter
{
    static class RoutePlanHelper
    {
        internal static string GetRouteAsStoredFile(PropertyLocation destination)
        {
            try
            {
                return GetRouteAsStoredFileCore(destination);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, typeof(RoutePlanHelper), Resources.RoutePlanHelperError);
                Logger.Instance.LogException(typeof(RoutePlanHelper), ex);
            }
            return null;
        }

        private static string GetRouteAsStoredFileCore(PropertyLocation destination)
        {
            PropertyLocation source = AlarmWorkflowConfiguration.Instance.FDInformation.Location;
            int width = 800;
            int height = 800;

            // https://developers.google.com/maps/documentation/directions/?hl=de

            string originText = source.ToString();
            string destinationText = destination.ToString();

            // Create initial request
            StringBuilder sbInitialRequest = new StringBuilder();
            sbInitialRequest.Append("http://maps.google.com/maps/api/directions/xml?");
            sbInitialRequest.AppendFormat("origin={0}", originText);
            sbInitialRequest.AppendFormat("&destination={0}", destinationText);
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
                        Logger.Instance.LogFormat(LogType.Warning, typeof(RoutePlanHelper), "The maps-request failed with status '{0}'. This is an indication that the location could not be retrieved. Sorry, but there's no workaround.", status);
                        return null;
                    case "OVER_QUERY_LIMIT":
                        Logger.Instance.LogFormat(LogType.Error, typeof(RoutePlanHelper), "The maps-request failed with status 'OVER_QUERY_LIMIT'. This indicates too many queries within a short timeframe.");
                        return null;

                    case "MAX_WAYPOINTS_EXCEEDED":
                    case "INVALID_REQUEST":
                    case "REQUEST_DENIED":
                    case "UNKNOWN_ERROR":
                    default:
                        Logger.Instance.LogFormat(LogType.Error, typeof(RoutePlanHelper), "The maps-request failed with status '{0}'. Please contact the developers!", status);
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
            // TODO: Maybe allow configuring the thickness and color, especially for b/w-printers?
            sbContinuationRequest.AppendFormat("&sensor=false&path=weight:3|color:red|enc:{0}", overviewE.Value);

            WebRequest wr1 = WebRequest.Create(sbContinuationRequest.ToString());
            using (WebResponse res1 = wr1.GetResponse())
            {
                using (Image image = Image.FromStream(res1.GetResponseStream()))
                {
                    using (FileStream tempFile = File.OpenWrite(Path.GetTempFileName()))
                    {
                        image.Save(tempFile, ImageFormat.Png);
                        return tempFile.Name;
                    }
                }
            }
        }
    }
}