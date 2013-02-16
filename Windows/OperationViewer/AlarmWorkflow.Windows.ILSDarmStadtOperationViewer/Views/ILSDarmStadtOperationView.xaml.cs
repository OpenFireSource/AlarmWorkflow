using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UIContracts.Extensibility;

namespace AlarmWorkflow.Windows.ILSDarmStadtOperationViewer.Views
{
    /// <summary>
    /// Interaction logic for IlsAnsbachNeaOperationView.xaml. This is the same as the IlsAnsbachOperationView, but is adjusted for FFW-NEA needs.
    /// </summary>
    [Export("ILSDarmStadtOperationViewer", typeof(IOperationViewer))]
    public partial class ILSDarmStadtOperationView : UserControl, IOperationViewer
    {
        #region Fields

        private ILSDarmStadtOperationViewModel _viewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ILSDarmStadtOperationView"/> class.
        /// </summary>
        public ILSDarmStadtOperationView()
        {
            InitializeComponent();

            _viewModel = new ILSDarmStadtOperationViewModel();
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
            _viewModel.Operation = operation;
        }

        void IOperationViewer.OnOperationChanged(Operation operation)
        {
            _viewModel.Operation = operation;
        }

        #endregion
    }
}
