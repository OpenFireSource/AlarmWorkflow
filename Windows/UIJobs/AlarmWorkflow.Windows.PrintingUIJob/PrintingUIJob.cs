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
using AlarmWorkflow.Windows.PrintingUIJob.Properties;
using AlarmWorkflow.Windows.UIContracts.Extensibility;

namespace AlarmWorkflow.Windows.PrintingUIJob
{
    /// <summary>
    /// A UI-Job that automatically prints the output of the UI.
    /// </summary>
    [Export("PrintingUIJob", typeof(IUIJob))]
    class PrintingUIJob : IUIJob
    {
        #region Constants

        private static readonly string PrintedOperationsCacheFileName = System.IO.Path.Combine(Utilities.GetLocalAppDataFolderPath(), "PrintingUIPrintedOperations.txt");

        #endregion

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
            if (!_configuration.RememberPrintedOperations)
            {
                return false;
            }

            List<string> alreadyPrintedOperations = new List<string>();

            if (File.Exists(PrintedOperationsCacheFileName))
            {
                alreadyPrintedOperations = new List<string>(File.ReadAllLines(PrintedOperationsCacheFileName));
                if (alreadyPrintedOperations.Contains(operation.OperationNumber))
                {
                    return true;
                }
            }

            if (addIfNot)
            {
                alreadyPrintedOperations.Add(operation.OperationNumber);
                File.WriteAllLines(PrintedOperationsCacheFileName, alreadyPrintedOperations.ToArray());
            }

            return false;
        }

        private PrintQueue GetPrintQueue()
        {
            if (_configuration == null)
            {
                return null;
            }

            PrintServer printServer = GetPrintServer();
            if (printServer == null)
            {
                return null;
            }

            // Pick the desired printer (even if none is selected, for convenience)
            EnumeratedPrintQueueTypes[] enpqt = new[] { EnumeratedPrintQueueTypes.Connections, EnumeratedPrintQueueTypes.Local };
            PrintQueue queue = printServer.GetPrintQueues(enpqt).FirstOrDefault(pq => pq.FullName.Equals(_configuration.PrinterName));

            if (queue != null)
            {
                return queue;
            }

            // Otherwise see if we are supposed to return a custom named printer ...
            if (!string.IsNullOrWhiteSpace(_configuration.PrinterName))
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Resources.PrintServerByNameNotFound, _configuration.PrinterName);
            }

            // Return the default, local printer (there is no default print queue for a print server other than the local server!).
            return LocalPrintServer.GetDefaultPrintQueue();
        }

        private PrintServer GetPrintServer()
        {
            try
            {
                return new PrintServer(_configuration.PrintServer);
            }
            catch (PrintServerException ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Resources.InvalidPrintServerName, ex.ServerName);
            }
            return null;
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
            if (CheckIsOperationAlreadyPrinted(operation, true))
            {
                return;
            }

            PrintQueue printQueue = _printQueue.Value;
            if (printQueue == null)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Resources.CannotPrintNoPrintQueue);
                return;
            }

            if (_configuration.UseAlternativeAlgorithm)
            {
                /* This algorithm uses a VisualBrush to print the operation viewer.
                 * This has proven functional in all cases.
                 * 
                 * The only "drawback" of this approach is that the layout seems to change,
                 * in that the height is never the same, if contents in the Content-control change.
                 * This may be fixed by a different layouting algorithm,
                 * but could also be seen as a "feature" - it reduces the printed content.
                 * 
                 */
                PrintUsingVisualBrush(operationViewer, operation, printQueue);
            }
            else
            {
                /* "Old" algorithm - shows certain problems where the layout is cut off on the right side.
                 * Find out who has problems with this algorithm. If there are too many people having problems,
                 * then remove this algorithm altogether.
                 */
                PrintUsingPrintVisual(operationViewer, operation, printQueue);
            }
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

            dialog.PrintDocument(document.DocumentPaginator, string.Format(Resources.PrintDocumentNameTemplate, operation.OperationNumber));
        }

        private void PrintUsingPrintVisual(IOperationViewer operationViewer, Operation operation, PrintQueue printQueue)
        {
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