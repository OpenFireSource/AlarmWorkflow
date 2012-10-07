using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UI.Extensibility;

namespace AlarmWorkflow.Windows.IlsAnsbachOperationViewer.Views
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

        #region Properties

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        internal UIConfiguration Configuration { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IlsAnsbachOperationView"/> class.
        /// </summary>
        public IlsAnsbachOperationView()
        {
            InitializeComponent();

            Configuration = UIConfiguration.Load();

            _viewModel = new ViewModel(Configuration);
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
