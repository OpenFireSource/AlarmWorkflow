using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UI.Extensibility;
using AlarmWorkflow.Windows.UI.ViewModels;

namespace AlarmWorkflow.Windows.UI.Views
{
    /// <summary>
    /// Interaction logic for DefaultOperationView.xaml
    /// </summary>
    [Export("DefaultOperationView", typeof(IOperationViewer))]
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
                    _operation = value;

                    // Set operation itself
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
