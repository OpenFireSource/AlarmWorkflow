using System;
using System.Linq;
using System.Timers;
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

        private Timer _focusTimer;

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

        private void FocusTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() => this.Focus()));
        }

        private void IlsAnsbachNeaOperationView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Loaded -= IlsAnsbachNeaOperationView_Loaded;

            // Set focus on the user control to true to allow us to handle (keyboard) events
            this.Focusable = true;
            this.Focus();

            // Cheat: Due to the (good) way WPF handles focussing, we need to manually force the focus on this control,
            // otherwise the focus may go anywhere in a leaf node of the visual/logical tree and we cannot bring it back here
            // again to use (Preview)KeyDown events (like if the focus gets stuck in the operation list).
            _focusTimer = new Timer(1000d);
            _focusTimer.Elapsed += FocusTimer_Elapsed;
            _focusTimer.Start();
        }

        private void UserControl_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Let's see which vehicle is associated with the shortkey
            var vehicle = _configuration.Vehicles.FirstOrDefault(v => v.Shortkey == e.Key);
            if (vehicle == null)
            {
                return;
            }

            _viewModel.ToggleManuallyDeployedVehicles(vehicle.Name);
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
