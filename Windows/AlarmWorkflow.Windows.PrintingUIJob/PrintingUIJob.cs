using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.UI.Extensibility;

namespace AlarmWorkflow.Windows.PrintingUIJob
{
    /// <summary>
    /// A UI-Job that automatically prints 
    /// </summary>
    [Export("PrintingUIJob", typeof(IUIJob))]
    class PrintingUIJob : IUIJob
    {
        #region Fields

        private Configuration _configuration;
        private Lazy<PrintQueue> _printQueue;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintingUIJob"/> class.
        /// </summary>
        public PrintingUIJob()
        {
            _printQueue = new Lazy<PrintQueue>(GetPrintQueue);
        }

        #endregion

        #region Methods

        private bool CheckIsOperationAlreadyPrinted(Operation operation, bool addIfNot)
        {
            // If we shall always print any operation (not recommended)
            if (!_configuration.RememberPrintedOperations)
            {
                return false;
            }

            // Load the file that stores the printed operations
            string fileName = Path.Combine(Utilities.GetWorkingDirectory(), "Config\\PrintingUIPrintedOperations.lst");

            List<string> alreadyPrintedOperations = new List<string>();

            if (File.Exists(fileName))
            {
                alreadyPrintedOperations = new List<string>(File.ReadAllLines(fileName));
                if (alreadyPrintedOperations.Contains(operation.OperationNumber))
                {
                    // Already printed --> do nothing.
                    return true;
                }
            }

            if (addIfNot)
            {
                alreadyPrintedOperations.Add(operation.OperationNumber);
                File.WriteAllLines(fileName, alreadyPrintedOperations.ToArray());
            }

            return false;
        }

        private PrintQueue GetPrintQueue()
        {
            if (_configuration == null)
            {
                return null;
            }

            // Connect to the print server
            PrintServer printServer = null;
            try
            {
                printServer = new PrintServer(_configuration.PrintServer);
            }
            catch (PrintServerException ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, "Invalid print server name! Make sure that the print server under '{0}' is running or correct the print server name (leave blank to use local computer's print server).", ex.ServerName);
                return null;
            }

            // Pick the desired printer (even if none is selected, for convenience)
            var enpqt = new[] { EnumeratedPrintQueueTypes.Connections, EnumeratedPrintQueueTypes.Local };
            PrintQueue queue = printServer.GetPrintQueues(enpqt).FirstOrDefault(pq => pq.FullName.Equals(_configuration.PrinterName));

            // If there was a printer found, return that one
            if (queue != null)
            {
                return queue;
            }

            // Otherwise see if we are supposed to return a custom named printer ...
            if (!string.IsNullOrWhiteSpace(_configuration.PrinterName))
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "Did not find a printer with name '{0}'. Using the default, local printer.", _configuration.PrinterName);
            }

            // Return the default, local printer (there is no default print queue for a print server other than the local server!).
            return LocalPrintServer.GetDefaultPrintQueue();
        }

        #endregion

        #region IUIJob Members

        bool IUIJob.Initialize()
        {
            _configuration = Configuration.Load();
            return true;
        }

        void IUIJob.OnNewOperation(IOperationViewer operationViewer, Operation operation)
        {
            // Only print if we don't have already (verrrrrrrrrrrry helpful during debugging, but also a sanity-check)
            if (CheckIsOperationAlreadyPrinted(operation, true))
            {
                return;
            }

            PrintQueue printQueue = _printQueue.Value;
            // If printing is not possible (an error occurred because the print server is not available etc.).
            if (printQueue == null)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "Cannot print job because the configured printer seems not available! Check log entries.");
                return;
            }

            // We need to wait for a bit to let the UI "catch a breath".
            // Otherwise, if printing immediately, it may have side-effects that parts of the visual aren't visible (bindings not updated etc.).
            Thread.Sleep(_configuration.WaitInterval);

            PrintDialog dialog = new PrintDialog();
            dialog.PrintQueue = printQueue;
            dialog.PrintTicket = dialog.PrintQueue.DefaultPrintTicket;
            dialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
            dialog.PrintTicket.CopyCount = _configuration.CopyCount;

            FrameworkElement visual = operationViewer.Visual;
            // Measure and arrange the visual before printing otherwise it looks unpredictably weird and may not fit on the page
            visual.Measure(new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight));
            visual.Arrange(new Rect(new Point(0, 0), visual.DesiredSize));

            dialog.PrintVisual(visual, "New alarm " + operation.OperationNumber);
        }

        #endregion
    }
}