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

using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UIContracts.Extensibility;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.DefaultViewer.Views
{
    /// <summary>
    /// Interaction logic for DefaultOperationView.xaml
    /// </summary>
    [Export("DefaultOperationView", typeof(IOperationViewer))]
    [Information(DisplayName = "ExportDowDisplayName", Description = "ExportDowDescription")]
    public partial class DefaultOperationView : UserControl, IOperationViewer
    {
        #region Fields

        private ViewModel _viewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultOperationView"/> class.
        /// </summary>
        public DefaultOperationView()
        {
            InitializeComponent();

            _viewModel = new ViewModel();
            this.DataContext = _viewModel;
        }

        #endregion

        #region IOperationViewer Members

        System.Windows.FrameworkElement IOperationViewer.Visual
        {
            get { return this; }
        }

        void IOperationViewer.OnNewOperation(Operation operation)
        {
        }

        void IOperationViewer.OnOperationChanged(Shared.Core.Operation operation)
        {
            _viewModel.Operation = operation;
        }

        #endregion

        #region Nested types

        class ViewModel : ViewModelBase
        {
            #region Fields

            private Operation _operation;

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets the operation.
            /// </summary>
            public Operation Operation
            {
                get { return _operation; }
                set
                {
                    if (value == _operation)
                    {
                        return;
                    }

                    _operation = value;

                    OnPropertyChanged("Operation");
                }
            }
            
            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ViewModel"/> class.
            /// </summary>
            public ViewModel()
            {

            }

            #endregion

        }

        #endregion
    }
}