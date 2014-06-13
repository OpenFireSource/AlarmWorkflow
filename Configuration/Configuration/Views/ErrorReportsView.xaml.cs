// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

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