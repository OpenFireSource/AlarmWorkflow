using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Threading;
using AlarmWorkflow.Job.OperationPrinter.Properties;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Engine;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Shared.Specialized.Printing;

namespace AlarmWorkflow.Job.OperationPrinter
{
    [Export("OperationPrinterJob", typeof(IJob))]
    [Information(DisplayName = "ExportJobDisplayName", Description = "ExportJobDescription")]
    class OperationPrinterJob : IJob
    {
        #region Fields

        private string _templateFile;
        private Operation _operation;

        #endregion

        #region IJob Members

        void IJob.Execute(IJobContext context, Operation operation)
        {
            if (context.Phase == JobPhase.AfterOperationStored)
            {
                PrintOperation(operation);
            }
        }

        private void PrintOperation(Operation operation)
        {
            foreach (string queueName in SettingsManager.Instance.GetSetting("OperationPrinterJob", "PrintingQueueNames").GetStringArray())
            {
                PrintingQueue pq = PrintingQueueManager.GetInstance().GetPrintingQueue(queueName);
                if (pq == null || !pq.IsEnabled)
                {
                    continue;
                }

                _operation = operation;
                PrintWithQueue(pq, operation);
            }
        }

        private void PrintWithQueue(PrintingQueue pq, Operation operation)
        {
            Thread printThread = new Thread(() => GdiPrinter.Print(pq, GdiPrinterPrintAction));
            // STA needed because of the use of WebBrowser (WinForms).
            printThread.SetApartmentState(ApartmentState.STA);
            printThread.Start();
            // Intentionally synchronize thread until its done.
            printThread.Join();
        }

        private bool GdiPrinterPrintAction(int pageIndex, Graphics graphics, Rectangle marginBounds, Rectangle pageBounds, PageSettings pageSettings, ref object state)
        {
            pageSettings.Landscape = false;

            Image renderedImage = state as Image;
            if (renderedImage == null)
            {
                // Store the whole rendered image and share it across multiple pages.
                Size renderBounds = new Size(pageBounds.Width, 0);

                renderedImage = TemplateRenderer.RenderOperation(_operation, _templateFile, renderBounds);
                state = renderedImage;
            }

            // Calculate the source rectangle (the portion of the rendered image) depending on which page we are in.
            int pagesNeeded = (int)Math.Ceiling((double)renderedImage.Height / (double)pageBounds.Size.Height);
            Rectangle destRect = pageBounds;
            Rectangle srcRect = new Rectangle(0, pageBounds.Height * (pageIndex - 1), pageBounds.Width, pageBounds.Height);

            graphics.DrawImage(renderedImage, destRect, srcRect, GraphicsUnit.Pixel);

            return pageIndex < pagesNeeded;
        }

        bool IJob.Initialize()
        {
            _templateFile = SettingsManager.Instance.GetSetting("OperationPrinterJob", "TemplateFile").GetString();
            if (!Path.IsPathRooted(_templateFile))
            {
                _templateFile = Path.Combine(Utilities.GetWorkingDirectory(), _templateFile);
            }

            if (!File.Exists(_templateFile))
            {
                Logger.Instance.LogFormat(LogType.Error, this, Resources.OperationPrintTemplateNotFoundError, _templateFile);
                return false;
            }


            return true;
        }

        bool IJob.IsAsync
        {
            get { return true; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}
