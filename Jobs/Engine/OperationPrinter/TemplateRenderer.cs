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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.ObjectExpressions;

namespace AlarmWorkflow.Job.OperationPrinter
{
    /// <summary>
    /// Renders an <see cref="Operation"/>-object using a customizable HTML-page for layouting.
    /// </summary>
    static class TemplateRenderer
    {
        #region Constants

        private const int WebBrowserAfterCompleteGracePeriodMs = 500;
        private static readonly bool KeepTempHtmlAfterFinish;

        #endregion

        #region Constructors

        static TemplateRenderer()
        {
            KeepTempHtmlAfterFinish = Debugger.IsAttached;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Renders the given operation using the provided template file and restricts the output to the given size.
        /// </summary>
        /// <param name="source">The source location to start. This is usually the fire department site.</param>
        /// <param name="operation">The operation to render.</param>
        /// <param name="templateFile">The HTML template file to use for layouting.</param>
        /// <param name="size">The maximum size of the created image.</param>
        /// <param name="timeout">The timeout to use for executing the script. The script will be terminated if it exceeds the duration.</param>
        /// <returns></returns>
        internal static Image RenderOperation(PropertyLocation source, Operation operation, string templateFile, Size size, TimeSpan timeout)
        {
            Image image = null;

            string tempFilePath = GetTemporaryHtmlFilePath(operation, templateFile);
            FileInfo fi = new FileInfo(tempFilePath);

            try
            {
                string html = CreateHtml(templateFile, source, operation);

                using (FileStream stream = fi.Create())
                {
                    fi.Attributes = FileAttributes.Temporary;

                    byte[] content = Encoding.Default.GetBytes(html);
                    stream.Write(content, 0, content.Length);
                }

                image = RenderOperationWithBrowser(fi, size, timeout);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(typeof(TemplateRenderer), ex);
            }
            finally
            {
                if (!KeepTempHtmlAfterFinish)
                {
                    fi.Delete();
                }
            }

            return image;
        }

        private static string GetTemporaryHtmlFilePath(Operation operation, string templateFile)
        {
            return Path.ChangeExtension(templateFile, operation.Id.ToString() + ".htm");
        }

        private static string CreateHtml(string templateFile, PropertyLocation source, Operation operation)
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> lines = File.ReadAllLines(templateFile).Where(_ => !string.IsNullOrWhiteSpace(_));

            foreach (string item in lines)
            {
                string line = item;

                /* Replace some special lines we might expect...
                 */
                if (line.Contains("var var_awSource = null"))
                {
                    line = string.Format("var var_awSource = {0};", Json.Serialize(source));
                }
                else if (line.Contains("var var_awOperation = null"))
                {
                    line = string.Format("var var_awOperation = {0};", Json.Serialize(operation));
                }
                else
                {
                    line = ObjectFormatter.ToString(operation, line);
                }

                sb.AppendLine(line);
            }

            return sb.ToString();
        }

        private static Image RenderOperationWithBrowser(FileInfo file, Size size, TimeSpan timeout)
        {
            ScriptingObject obj = new ScriptingObject();

            using (WebBrowser w = new WebBrowser())
            {
                w.AllowNavigation = true;
                w.ScrollBarsEnabled = false;
                w.ScriptErrorsSuppressed = true;

                w.ObjectForScripting = obj;
                w.Navigate(file.FullName);

                Stopwatch s = Stopwatch.StartNew();

                while (true)
                {
                    if (w.ReadyState == WebBrowserReadyState.Complete && obj.IsClientSideScriptReady)
                    {
                        /* Hint: It appears that if we exit right here, the browser isn't quite finished with drawing yet.
                         * Adding a little grace period followed by a DoEvents() seems to work reliably.
                         */
                        Thread.Sleep(WebBrowserAfterCompleteGracePeriodMs);
                        Application.DoEvents();

                        break;
                    }

                    if (s.ElapsedTicks >= (long)timeout.Ticks)
                    {
                        Logger.Instance.LogFormat(LogType.Warning, typeof(TemplateRenderer), "Exceeded timeout for rendering template! The template may not be complete.");
                        break;
                    }

                    // Pump the message queue for the web browser.
                    Application.DoEvents();

                    Thread.Sleep(1);
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

        #endregion
    }
}