using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text.RegularExpressions;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Engine;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Specialized.Printing;

namespace AlarmWorkflow.Job.AlarmSourcePrinterJob
{
    [Export("AlarmSourcePrinterJob", typeof(IJob))]
    class AlarmSourcePrinterJob : IJob
    {
        #region Fields

        private PrintJobConfiguration _printingConfiguration;

        #endregion

        #region Methods

        private void PrintFaxes(IJobContext context, Operation operation)
        {
            if (!context.Parameters.ContainsKey("ArchivedFilePath") || !context.Parameters.ContainsKey("SplittedTiffFileNames"))
            {
                return;
            }

            System.IO.FileInfo sourceImageFile = new System.IO.FileInfo((string)context.Parameters["ArchivedFilePath"]);
            if (!sourceImageFile.Exists)
            {
                // It was removed in the meanwhile or so
                return;
            }

            // Grab all created files to print
            string[] splittedTiffFileNames = (string[])context.Parameters["SplittedTiffFileNames"];

            PrintDocument doc = new PrintDocument();
            doc.DocumentName = operation.OperationNumber + Properties.Resources.DocumentNameAppendix;
            if (!_printingConfiguration.IsDefaultPrinter)
            {
                doc.PrinterSettings.PrinterName = _printingConfiguration.PrinterName;
            }

            // Create dedicated task to wrap the events of the PrintDocument-class
            PrintFaxTask task = new PrintFaxTask();
            task.SplittedTiffFileNames = splittedTiffFileNames;
            task.Print(doc);
        }

        #endregion

        #region IJob Members

        void IJob.Execute(IJobContext context, Operation operation)
        {
            // TODO: May be made modular using a plug-in system and/or delegates in the future!
            switch (context.AlarmSourceName)
            {
                case "FaxAlarmSource": PrintFaxes(context, operation); break;
                default:
                    break;
            }
        }

        bool IJob.Initialize()
        {
            _printingConfiguration = PrintJobConfiguration.FromSettings("AlarmSourcePrinterJob");

            AssertPrintJobConfigurationIsComplete();
            return true;
        }

        private void AssertPrintJobConfigurationIsComplete()
        {
            if (!_printingConfiguration.IsDefaultPrinter)
            {
                IEnumerable<string> printerNames = PrinterSettings.InstalledPrinters.Cast<string>();
                if (!printerNames.Any(p => p == _printingConfiguration.PrinterName))
                {
                    string message = string.Format(Properties.Resources.NoSuchPrinterFoundError, _printingConfiguration.PrinterName, string.Join(", ", printerNames));
                    throw new InvalidOperationException(message);
                }
            }
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

        #region Nested types

        class PrintFaxTask
        {
            private int _currentPageIndex = -1;

            internal string[] SplittedTiffFileNames { get; set; }

            internal void Print(PrintDocument doc)
            {
                doc.PrintPage += doc_PrintPage;
                doc.Print();
            }

            private void doc_PrintPage(object sender, PrintPageEventArgs e)
            {
                _currentPageIndex++;

                bool hasMorePages = _currentPageIndex < SplittedTiffFileNames.Length - 1;
                e.HasMorePages = hasMorePages;

                if (!hasMorePages)
                {
                    ((PrintDocument)sender).PrintPage -= doc_PrintPage;
                }

                using (Image image = Image.FromFile(SplittedTiffFileNames[_currentPageIndex]))
                {
                    Point point = Point.Empty;
                    e.Graphics.DrawImage(image, point);
                }
            }
        }

        #endregion
    }
}
