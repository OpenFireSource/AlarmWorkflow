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

        private bool GdiPrinterPrintAction(int pageIndex, Graphics graphics, Rectangle marginBounds, Rectangle pageBounds, PageSettings pageSettings)
        {
            pageSettings.Landscape = false;

            Image rendered = TemplateRenderer.RenderOperation(_operation, _templateFile, pageBounds.Size);
            graphics.DrawImage(rendered, pageBounds.Location);

            // TODO: Support printing more than one page. Clip contents at page boundaries and continue from there on the next page.
            // No more pages --> false.
            return false;
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
