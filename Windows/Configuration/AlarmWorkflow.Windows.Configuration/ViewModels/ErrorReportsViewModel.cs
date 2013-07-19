using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using AlarmWorkflow.Shared.Diagnostics.Reports;
using AlarmWorkflow.Windows.UIContracts;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.ViewModels
{
    class ErrorReportsViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets the list that contains all error reports to be shown in the list.
        /// </summary>
        public ObservableCollection<ErrorReportViewModel> ErrorReports { get; private set; }

        #endregion

        #region Commands

        #region Command "RefreshErrorReportsListCommand"

        /// <summary>
        /// The RefreshErrorReportsListCommand command.
        /// </summary>
        public ICommand RefreshErrorReportsListCommand { get; private set; }

        private void RefreshErrorReportsListCommand_Execute(object parameter)
        {
            RefreshErrorReportsList();
        }

        #endregion

        #region Command "GoToErrorReportsDirectoryCommand"

        /// <summary>
        /// The GoToErrorReportsDirectoryCommand command.
        /// </summary>
        public ICommand GoToErrorReportsDirectoryCommand { get; private set; }

        private void GoToErrorReportsDirectoryCommand_Execute(object parameter)
        {
            string directory = ErrorReportManager.ErrorReportPath;
            if (Directory.Exists(directory))
            {
                Process.Start(directory);
            }
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReportsViewModel"/> class.
        /// </summary>
        public ErrorReportsViewModel()
        {
            ErrorReports = new ObservableCollection<ErrorReportViewModel>();
            RefreshErrorReportsList();
        }

        #endregion

        #region Methods

        private void RefreshErrorReportsList()
        {
            ErrorReports.Clear();

            try
            {
                foreach (ErrorReport report in ErrorReportManager.GetNewestReports(null, 0))
                {
                    AddSingleErrorReport(report);
                }
            }
            catch (DirectoryNotFoundException)
            {
                // Nothing - if the directory is not found, there just have been no errors yet.
            }
        }

        internal void AddSingleErrorReport(ErrorReport report)
        {
            ErrorReportViewModel reportVM = new ErrorReportViewModel() { Report = report };
            ErrorReports.Add(reportVM);
        }

        #endregion

        #region Nested types

        internal class ErrorReportViewModel : ViewModelBase
        {
            #region Properties

            public ErrorReport Report { get; set; }

            public string TimestampLocalized
            {
                get { return Report.Timestamp.ToLocalTime().ToString(UIUtilities.DateTimeFormatGermany); }
            }
            public IEnumerable<ExceptionDetail> FlattenedExceptionDetails
            {
                get
                {
                    ExceptionDetail detail = Report.Exception;
                    while (detail != null)
                    {
                        yield return detail;
                        detail = detail.InnerException;
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}
