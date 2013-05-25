using System;
using System.Drawing;
using System.Drawing.Printing;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Properties;

namespace AlarmWorkflow.Shared.Specialized.Printing
{
    /// <summary>
    /// Represents a printer that uses GDI+ to print contents.
    /// </summary>
    public static class GdiPrinter
    {
        #region Methods

        /// <summary>
        /// Executes a printing operation using a specific <see cref="PrintingQueue"/> and action.
        /// </summary>
        /// <param name="queue">The printing queue to use. Must not be null.</param>
        /// <param name="printAction">The printing action. Must not be null.</param>
        public static void Print(PrintingQueue queue, PrintDelegate printAction)
        {
            Assertions.AssertNotNull(queue, "queue");
            Assertions.AssertNotNull(printAction, "printAction");

            PrintDocument doc = new PrintDocument();
            if (!queue.IsDefaultPrinter)
            {
                doc.PrinterSettings.PrinterName = queue.PrinterName;
            }

            int desiredCopyCount = queue.CopyCount;
            int maxSupportedCopyCount = doc.PrinterSettings.MaximumCopies;
            int requiredPrintIterations = 1;

            if (desiredCopyCount <= maxSupportedCopyCount)
            {
                doc.PrinterSettings.Copies = (short)desiredCopyCount;
            }
            else
            {
                // It appears that some printers don't support the CopyCount-feature (notably Microsoft XPS Writer or perhaps PDF-Writers in general?).
                // In this case we simply repeat printing until we have reached our copy count.
                Logger.Instance.LogFormat(LogType.Warning, typeof(GdiPrinter), Resources.UsedPrinterDoesNotSupportThatMuchCopies, maxSupportedCopyCount, desiredCopyCount);

                requiredPrintIterations = desiredCopyCount;
            }

            for (int i = 0; i < requiredPrintIterations; i++)
            {
                Logger.Instance.LogFormat(LogType.Trace, typeof(GdiPrinter), Resources.PrintIterationStart, i + 1, requiredPrintIterations);

                PrintTask task = new PrintTask();
                try
                {
                    task.Print(doc, printAction);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, typeof(GdiPrinter), Resources.GdiPrinterPrintTaskException);
                    Logger.Instance.LogException(typeof(GdiPrinter), ex);
                }

                Logger.Instance.LogFormat(LogType.Trace, typeof(GdiPrinter), Resources.PrintIterationEnd);
            }
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents the delegate that is invoked until the return value returns <c>false</c>.
        /// </summary>
        /// <param name="pageIndex">The index of the page that is printed (one-based).</param>
        /// <param name="graphics">The graphics for the page to print.</param>
        /// <param name="marginBounds">The area between the margins.</param>
        /// <param name="pageBounds">The total area of the paper.</param>
        /// <param name="pageSettings">The <see cref="PageSettings"/> for the page.</param>
        /// <param name="state">A custom user state object. May be used to store correlation information across multiple pages.</param>
        /// <returns>Whether or not there are still pages to be printed. Returns true if more, or false if no more pages are to be printed.</returns>
        public delegate bool PrintDelegate(int pageIndex, Graphics graphics, Rectangle marginBounds, Rectangle pageBounds, PageSettings pageSettings, ref object state);

        /// <summary>
        /// Encapsulates one single GDI print task. Uses a custom <see cref="PrintDelegate"/> to print.
        /// </summary>
        sealed class PrintTask
        {
            #region Fields

            private PrintDelegate _printAction;
            private int _currentPageIndex = 0;
            private object _state;

            #endregion

            #region Methods

            /// <summary>
            /// Executes the print task using the given document and action.
            /// </summary>
            /// <param name="doc"></param>
            /// <param name="printAction"></param>
            internal void Print(PrintDocument doc, PrintDelegate printAction)
            {
                _printAction = printAction;

                doc.PrintPage += doc_PrintPage;
                doc.Print();
            }

            private void doc_PrintPage(object sender, PrintPageEventArgs e)
            {
                _currentPageIndex++;
                bool hasMorePages = false;

                try
                {
                    hasMorePages = _printAction(_currentPageIndex, e.Graphics, e.MarginBounds, e.PageBounds, e.PageSettings, ref _state);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, typeof(PrintTask), Resources.PrintTaskPrintPageException, _currentPageIndex);
                    Logger.Instance.LogException(typeof(PrintTask), ex);
                }

                Logger.Instance.LogFormat(LogType.Trace, this, Resources.PrintingDone, _currentPageIndex);

                e.HasMorePages = hasMorePages;
                if (!e.HasMorePages)
                {
                    ((PrintDocument)sender).PrintPage -= doc_PrintPage;
                }
            }

            #endregion
        }

        #endregion
    }
}
