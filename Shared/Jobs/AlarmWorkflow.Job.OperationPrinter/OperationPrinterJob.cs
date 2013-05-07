using System;
using System.Drawing;
using System.Drawing.Printing;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Engine;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Shared.Specialized.Printing;

namespace AlarmWorkflow.Job.OperationPrinter
{
    [Export("OperationPrinterJob", typeof(IJob))]
    class OperationPrinterJob : IJob
    {
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

                PrintWithQueue(pq, operation);
            }
        }

        private void PrintWithQueue(PrintingQueue pq, Operation operation)
        {
            GdiPrinter.Print(pq, GdiPrinterPrintAction);
        }

        private bool GdiPrinterPrintAction(Graphics graphics, Rectangle marginBounds, Rectangle pageBounds, PageSettings pageSettings)
        {
            
            return false;
        }

        bool IJob.Initialize()
        {
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
