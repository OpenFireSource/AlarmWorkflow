using System.Linq;
using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UI.Extensibility;

namespace AlarmWorkflow.Windows.IlsAnsbachOperationViewer.Views
{
    /// <summary>
    /// Interaction logic for IlsAnsbachNeaOperationView.xaml. This is the same as the IlsAnsbachOperationView, but is adjusted for FFW-NEA needs.
    /// </summary>
    [Export("IlsAnsbachNeaOperationViewer", typeof(IOperationViewer))]
    public partial class IlsAnsbachNeaOperationView : UserControl, IOperationViewer
    {
        #region Fields

        private UIConfigurationNea _configuration;
        private IlsAnsbachNeaViewModel _viewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IlsAnsbachNeaOperationView"/> class.
        /// </summary>
        public IlsAnsbachNeaOperationView()
        {
            InitializeComponent();

            _configuration = UIConfigurationNea.Load();

            _viewModel = new IlsAnsbachNeaViewModel(_configuration);
            this.DataContext = _viewModel;

            this.Loaded += IlsAnsbachNeaOperationView_Loaded;
        }

        #endregion

        #region Event handlers

        private void IlsAnsbachNeaOperationView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Loaded -= IlsAnsbachNeaOperationView_Loaded;

            // Set focus on the user control to true to allow us to handle (keyboard) events
            this.Focusable = true;
            this.Focus();
        }

        private void UserControl_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Let's see which vehicle is associated with the shortkey
            var vehicle = _configuration.Vehicles.FirstOrDefault(v => v.Shortkey == e.Key);
            if (vehicle == null)
            {
                return;
            }

            _viewModel.AddManuallyDeployedVehicles(vehicle.Name);
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
