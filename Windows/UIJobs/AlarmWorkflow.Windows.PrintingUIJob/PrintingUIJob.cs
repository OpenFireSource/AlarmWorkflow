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
using AlarmWorkflow.Windows.UIContracts.Extensibility;

/* Hint:
 * There is information available for WPF printing.
 * Some bugs have been reported that the print output is clipped or cut off.
 * 
 * Examine the tips from these links for cure:
 * http://stackoverflow.com/questions/9674628/print-with-no-margin
 * http://tech.pro/tutorial/881/printing-in-wpf
 * http://stackoverflow.com/questions/12772202/printing-a-wpf-visual-in-landscape-orientation-printer-still-clips-at-portrait
 */

namespace AlarmWorkflow.Windows.PrintingUIJob
{
    /// <summary>
    /// A UI-Job that automatically prints the output of the UI.
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

        bool IUIJob.IsAsync
        {
            // Must be synchronous because of UI-access
            get { return false; }
        }

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

            // Get the print caps for measure and arrange
            PrintCapabilities printCaps = printQueue.GetPrintCapabilities(dialog.PrintTicket);
            PageImageableArea pia = printCaps.PageImageableArea;

            FrameworkElement visual = operationViewer.Visual;
            visual.Margin = new Thickness(pia.OriginWidth, pia.OriginHeight, pia.OriginWidth, pia.OriginHeight);

            // DEBUG
            Logger.Instance.LogFormat(LogType.Debug, this, "PageImageableArea is = {0}", pia);
            Logger.Instance.LogFormat(LogType.Debug, this, "Visual's Margin is = {0}", visual.Margin);

            // Measure and arrange the visual before printing otherwise it looks unpredictably weird and may not fit on the page
            visual.Measure(new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight));
            Rect arrangeRect = new Rect(new Point(pia.OriginWidth, pia.OriginHeight), visual.DesiredSize);
            visual.Arrange(arrangeRect);

            // DEBUG
            Logger.Instance.LogFormat(LogType.Debug, this, "Visual's printing size / rect is = {0} / {1}", visual.DesiredSize, arrangeRect);

            dialog.PrintVisual(visual, "New alarm " + operation.OperationNumber);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (_printQueue.IsValueCreated)
            {
                _printQueue.Value.Dispose();
            }
        }

        #endregion
    }
}