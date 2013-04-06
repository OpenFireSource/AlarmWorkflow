using System.IO;
using System.Windows;
using System.Windows.Controls;
using AlarmWorkflow.Shared.Diagnostics.Reports;
using AlarmWorkflow.Windows.Configuration.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.Views
{
    /// <summary>
    /// Interaction logic for ErrorReportsView.xaml
    /// </summary>
    public partial class ErrorReportsView : UserControl
    {
        #region Fields

        private ErrorReportsViewModel _viewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReportsView"/> class.
        /// </summary>
        public ErrorReportsView()
        {
            InitializeComponent();

            _viewModel = new ErrorReportsViewModel();
            this.DataContext = _viewModel;
        }

        #endregion

        #region Methods

        private void lsvReports_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                HandleDroppedFile(e.Data.GetData(DataFormats.FileDrop));
            }
        }

        private void HandleDroppedFile(object data)
        {
            _viewModel.ErrorReports.Clear();
            foreach (string fileName in (string[])data)
            {
                try
                {
                    ErrorReport report = ErrorReport.Deserialize(File.ReadAllText(fileName));

                    _viewModel.AddSingleErrorReport(report);
                }
                catch (System.Exception)
                {
                    // TODO: Handling (message box).
                }
            }
        }

        #endregion

    }
}
