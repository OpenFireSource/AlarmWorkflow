using System.Drawing;
using System.Drawing.Printing;
using AlarmWorkflow.Shared.Core;

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
            int alternativeCopyPrintingAmount = 1;

            if (desiredCopyCount <= maxSupportedCopyCount)
            {
                doc.PrinterSettings.Copies = (short)desiredCopyCount;
            }
            else
            {
                //// It appears that some printers don't support the CopyCount-feature (notably Microsoft XPS Writer or perhaps PDF-Writers in general?).
                //// In this case we simply repeat printing until we have reached our copy count.
                //Logger.Instance.LogFormat(LogType.Warning, this, Resources.UsedPrinterDoesNotSupportThatMuchCopies, maxSupportedCopyCount, desiredCopyCount);

                // Setting this variable causes the loop below to execute the Print() method the specified number of times.
                alternativeCopyPrintingAmount = desiredCopyCount;
            }

            for (int i = 0; i < alternativeCopyPrintingAmount; i++)
            {
                //Logger.Instance.LogFormat(LogType.Trace, this, Resources.PrintIterationStart, i + 1, alternativeCopyPrintingAmount);

                PrintTask task = new PrintTask();
                task.Print(doc, printAction);

                //Logger.Instance.LogFormat(LogType.Trace, this, Resources.PrintIterationEnd);
            }
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents the delegate that is invoked as long until the return value returns <c>false</c>.
        /// </summary>
        /// <param name="graphics">The graphics for the page to print.</param>
        /// <param name="marginBounds"></param>
        /// <param name="pageBounds"></param>
        /// <param name="pageSettings"></param>
        /// <returns>Whether or not there are still pages to be printed. Returns true if more, or false if no more pages are to be printed.</returns>
        public delegate bool PrintDelegate(Graphics graphics, Rectangle marginBounds, Rectangle pageBounds, PageSettings pageSettings);

        class PrintTask
        {
            private PrintDelegate _printAction;

            internal void Print(PrintDocument doc, PrintDelegate printAction)
            {
                _printAction = printAction;

                doc.PrintPage += doc_PrintPage;
                doc.Print();
            }

            private void doc_PrintPage(object sender, PrintPageEventArgs e)
            {

                bool hasMorePages = false;
                try
                {
                    hasMorePages = _printAction(e.Graphics, e.MarginBounds, e.PageBounds, e.PageSettings);
                }
                catch (System.Exception)
                {
                    // TODO
                }
                e.HasMorePages = hasMorePages;

                //Logger.Instance.LogFormat(LogType.Trace, this, Resources.PrintingDone, _currentPageIndex);

                if (!hasMorePages)
                {
                    ((PrintDocument)sender).PrintPage -= doc_PrintPage;
                }
            }
        }

        #endregion
    }
}
