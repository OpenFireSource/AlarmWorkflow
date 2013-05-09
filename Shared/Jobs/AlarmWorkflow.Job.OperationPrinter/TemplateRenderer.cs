using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Job.OperationPrinter
{
    /// <summary>
    /// Renders an <see cref="Operation"/>-object using a customizable HTML-page for layouting.
    /// </summary>
    static class TemplateRenderer
    {
        /// <summary>
        /// Renders the given operation using the provided template file and restricts the output to the given size.
        /// </summary>
        /// <param name="operation">The operation to render.</param>
        /// <param name="templateFile">The HTML template file to use for layouting.</param>
        /// <param name="size">The maximum size of the created image.</param>
        /// <returns></returns>
        internal static Image RenderOperation(Operation operation, string templateFile, Size size)
        {
            TemplateObject to = new TemplateObject();
            to.Operation = operation;
            to.RouteImageBase64 = RoutePlanHelper.GetRouteAsBase64(operation.Einsatzort);

            // Create HTML to render
            StringBuilder sb = new StringBuilder();
            foreach (string line in File.ReadAllLines(templateFile))
            {
                sb.AppendLine(ObjectFormatter.ToString(to, line));
            }

            return RenderOperationWithBrowser(to, sb.ToString(), size);
        }

        private static Image RenderOperationWithBrowser(TemplateObject to, string htmlToRender, Size size)
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

                w.Width = realWidth;
                w.Height = realHeight;

                // Get a Bitmap representation of the webpage as it's rendered in the WebBrowser control
                using (Bitmap bitmap = new Bitmap(w.Width, w.Height))
                {
                    w.DrawToBitmap(bitmap, new Rectangle(0, 0, w.Width, w.Height));
                    return (Image)bitmap.Clone();
                }
            }
        }
    }
}
