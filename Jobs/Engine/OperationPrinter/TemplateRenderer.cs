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
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.ObjectExpressions;
using AlarmWorkflow.Job.OperationPrinter.Properties;
using System.Drawing.Imaging;

namespace AlarmWorkflow.Job.OperationPrinter
{
    /// <summary>
    /// Renders an <see cref="Operation"/>-object using a customizable HTML-page for layouting.
    /// </summary>
    static class TemplateRenderer
    {
        #region Methods

        /// <summary>
        /// Renders the given operation using the provided template file and restricts the output to the given size.
        /// </summary>
        /// <param name="source">The source location to start. This is usually the fire department site.</param>
        /// <param name="operation">The operation to render.</param>
        /// <param name="templateFile">The HTML template file to use for layouting.</param>
        /// <param name="size">The maximum size of the created image.</param>
        /// <param name="useOFM">Use OFM instead Google Maps</param>
        /// <returns></returns>
        internal static Image RenderOperation(PropertyLocation source, Operation operation, string templateFile, Size size, bool useOFM)
        {
            TemplateObject to = new TemplateObject();
            to.Operation = operation;
            
            if(useOFM)
                to.MapImageFilePath = GetOFMImageFilePath(operation.Einsatzort);
            else
                to.MapImageFilePath = RoutePlanHelper.GetRouteAsStoredFile(source, operation.Einsatzort);
            
            Image image = null;
            try
            {
                string html = CreateHtml(templateFile, to);

                image = RenderOperationWithBrowser(html, size);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(typeof(TemplateRenderer), ex);
            }
            finally
            {
                TryDeleteMapImageFile(to);
            }

            return image;
        }

        private static string GetOFMImageFilePath(PropertyLocation location)
        {
            string file = "OperationPrintOFMHelper.htm";
            string tmpFile = Path.GetTempFileName();
            Size size = new Size(800, 800);

            try
            {
                if (!File.Exists(file))
                {
                    Logger.Instance.LogFormat(LogType.Error, typeof(TemplateRenderer), Resources.OperationPrintTemplateNotFoundError, file);
                    return null;
                }

                string html = File.ReadAllText(file)
                    .Replace("{MapSize.Width}", size.Width.ToString())
                    .Replace("{MapSize.Height}", size.Height.ToString())
                    .Replace("{Location.GeoLatitude}", location.GeoLatitude)
                    .Replace("{Location.GeoLongitude}", location.GeoLongitude);
                
                Image image = RenderOperationWithBrowser(html, size);
                image.Save(tmpFile, ImageFormat.Png);

                return tmpFile;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(typeof(TemplateRenderer), ex);
                return null;
            }
        }

        private static string CreateHtml(string templateFile, TemplateObject to)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string line in File.ReadAllLines(templateFile))
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    sb.AppendLine(ObjectFormatter.ToString(to, line));
                }
            }

            return sb.ToString();
        }
        
        private static Image RenderOperationWithBrowser(string htmlToRender, Size size)
        {
            using (WebBrowser w = new WebBrowser())
            {
                w.AllowNavigation = true;
                w.ScrollBarsEnabled = false;
                w.ScriptErrorsSuppressed = true;

                w.DocumentText = htmlToRender;

                while (w.ReadyState != WebBrowserReadyState.Complete)
                {
                    // Pump the message queue for the web browser.
                    Application.DoEvents();
                }

                // Set the size of the WebBrowser control
                int suggestedWidth = w.Document.Body.ScrollRectangle.Width;
                int suggestedHeight = w.Document.Body.ScrollRectangle.Height;

                int realWidth = (suggestedWidth < size.Width) ? suggestedWidth : size.Width;
                int realHeight = (suggestedHeight < size.Height) ? suggestedHeight : size.Height;

                // If any of the Size-properties is set to zero, use the full size part.
                w.Width = (realWidth > 0) ? realWidth : suggestedWidth;
                w.Height = (realHeight > 0) ? realHeight : suggestedHeight;

                // Get a Bitmap representation of the webpage as it's rendered in the WebBrowser control
                using (Bitmap bitmap = new Bitmap(w.Width, w.Height))
                {
                    w.DrawToBitmap(bitmap, new Rectangle(0, 0, w.Width, w.Height));
                    return (Image)bitmap.Clone();
                }
            }
        }

        private static void TryDeleteMapImageFile(TemplateObject to)
        {
            try
            {
                if (File.Exists(to.MapImageFilePath))
                {
                    File.Delete(to.MapImageFilePath);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(typeof(TemplateRenderer), ex);
            }
        }

        #endregion
    }
}