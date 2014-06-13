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
using System.Web;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Job.OperationPrinter
{
    static class RoutePlanHelper
    {
        internal static string GetRouteAsStoredFile(PropertyLocation source, PropertyLocation destination)
        {
            if (!source.IsMeaningful)
            {
                Logger.Instance.LogFormat(LogType.Warning, typeof(RoutePlanHelper), Properties.Resources.NoSourceLocationDefined);
                return null;
            }
            if (!destination.IsMeaningful)
            {
                Logger.Instance.LogFormat(LogType.Warning, typeof(RoutePlanHelper), Properties.Resources.NoDestinationLocationAvailable);
                return null;
            }

            try
            {
                return GetRouteAsStoredFileCore(source, destination);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, typeof(RoutePlanHelper), Properties.Resources.RoutePlanHelperError);
                Logger.Instance.LogException(typeof(RoutePlanHelper), ex);
            }

            return null;
        }

        private static string GetRouteAsStoredFileCore(PropertyLocation source, PropertyLocation destination)
        {
            int width = 800;
            int height = 800;

            // https://developers.google.com/maps/documentation/directions/?hl=de

            string originText = source.ToString();
            string destinationText = destination.ToString();

            StringBuilder sbInitialRequest = new StringBuilder();
            sbInitialRequest.Append("http://maps.google.com/maps/api/directions/xml?");
            sbInitialRequest.AppendFormat("origin={0}", HttpUtility.UrlEncode(originText));
            sbInitialRequest.AppendFormat("&destination={0}", HttpUtility.UrlEncode(destinationText));
            sbInitialRequest.Append("&sensor=false");

            WebRequest wreqInitial = WebRequest.Create(sbInitialRequest.ToString());
            XDocument docResponse = null;
            using (WebResponse wresInitial = wreqInitial.GetResponse())
            {
                using (Stream stream = wresInitial.GetResponseStream())
                {
                    docResponse = XDocument.Load(stream);
                }

                string status = docResponse.Root.Element("status").Value;
                if (status != "OK")
                {
                    Logger.Instance.LogFormat(LogType.Error, typeof(RoutePlanHelper), Properties.Resources.MapsRequestFailed, status);
                    return null;
                }
            }

            XElement overviewE = docResponse.Root.Element("route").Element("overview_polyline").Element("points");

            StringBuilder sbContinuationRequest = new StringBuilder();
            sbContinuationRequest.Append("http://maps.google.com/maps/api/staticmap?");
            sbContinuationRequest.AppendFormat("size={0}x{1}", width, height);
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