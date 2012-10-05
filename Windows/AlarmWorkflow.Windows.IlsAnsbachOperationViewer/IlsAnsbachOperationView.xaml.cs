using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UI.Extensibility;

namespace AlarmWorkflow.Windows.IlsAnsbachOperationViewer
{
    /// <summary>
    /// Interaction logic for IlsAnsbachOperationView.xaml
    /// </summary>
    [Export("IlsAnsbachOperationViewer", typeof(IOperationViewer))]
    public partial class IlsAnsbachOperationView : UserControl, IOperationViewer
    {
        #region Fields

        private ViewModel _viewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IlsAnsbachOperationView"/> class.
        /// </summary>
        public IlsAnsbachOperationView()
        {
            InitializeComponent();

            _viewModel = new ViewModel();
            this.DataContext = _viewModel;
        }

        #endregion

        #region IOperationViewer Members

        System.Windows.FrameworkElement IOperationViewer.Create()
        {
            return this;
        }

        void IOperationViewer.OnOperationChanged(Operation operation)
        {
            _viewModel.Operation = operation;
        }

        #endregion
    }
}
