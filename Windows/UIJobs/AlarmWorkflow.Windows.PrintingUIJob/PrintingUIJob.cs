using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
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
            string fileName = System.IO.Path.Combine(Utilities.GetLocalAppDataFolderPath(), "PrintingUIPrintedOperations.txt");

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

            PrintUsingVisualBrush(operationViewer, operation, printQueue);
        }

        private void PrintUsingVisualBrush(IOperationViewer operationViewer, Operation operation, PrintQueue printQueue)
        {
            FrameworkElement frameworkElement = operationViewer.Visual;

            PrintDialog dialog = new PrintDialog();
            dialog.PrintQueue = printQueue;
            dialog.PrintTicket = dialog.PrintQueue.DefaultPrintTicket;
            dialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
            dialog.PrintTicket.CopyCount = _configuration.CopyCount;

            PrintCapabilities printCaps = printQueue.GetPrintCapabilities(dialog.PrintTicket);
            PageImageableArea pia = printCaps.PageImageableArea;

            frameworkElement.Measure(new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight));
            Rect arrangeRect = new Rect(new Point(pia.OriginWidth, pia.OriginHeight), frameworkElement.DesiredSize);
            frameworkElement.Arrange(arrangeRect);

            FixedDocument document = new FixedDocument();

            PageContent pageContent = new PageContent();
            FixedPage page = new FixedPage();
            page.ContentBox = new Rect(pia.OriginWidth, pia.OriginHeight, pia.ExtentWidth, pia.ExtentHeight);

            ((IAddChild)pageContent).AddChild(page);
            document.Pages.Add(pageContent);
            page.Width = dialog.PrintableAreaWidth;
            page.Height = dialog.PrintableAreaHeight;

            Canvas canvas = new Canvas();
            FixedPage.SetTop(canvas, 0d);
            FixedPage.SetLeft(canvas, 0d);
            canvas.Width = page.Width;
            canvas.Height = page.Height;

            VisualBrush brush = new VisualBrush(frameworkElement);
            brush.Stretch = Stretch.Uniform;

            Rectangle brushRect = new Rectangle();
            brushRect.Width = page.Width;
            brushRect.Height = page.Height;
            brushRect.Fill = brush;

            canvas.Children.Add(brushRect);
            page.Children.Add(canvas);
            
            dialog.PrintDocument(document.DocumentPaginator, string.Format("New alarm {0}", operation.OperationNumber));
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