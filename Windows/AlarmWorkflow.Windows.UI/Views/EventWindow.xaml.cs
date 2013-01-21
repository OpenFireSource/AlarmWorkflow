using System.Linq;
using System.Windows;
using System.Windows.Forms;
using AlarmWorkflow.Windows.UI.ViewModels;
using AlarmWorkflow.Windows.UIContracts;

namespace AlarmWorkflow.Windows.UI.Views
{
    /// <summary>
    /// Interaction logic for EventWindow.xaml
    /// </summary>
    internal partial class EventWindow : Window
    {
        #region Fields

        private EventWindowViewModel _viewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EventWindow"/> class.
        /// </summary>
        public EventWindow()
        {
            InitializeComponent();

            this.uiScaleSlider.Value = App.GetApp().Configuration.ScaleFactor;

            _viewModel = new EventWindowViewModel();
            this.DataContext = _viewModel;

            SetLocationToScreen();
        }

        #endregion

        #region Methods

        private void SetLocationToScreen()
        {
            // Get all screens on this system
            // Nasty: We need System.Windows.Forms for this. Unfortunately there is no cure. WPF doesn't expose such API, unfortunately...
            Screen screenToShowOn = null;

            int desiredScreenId = App.GetApp().Configuration.ScreenId;
            if (desiredScreenId > 0 && desiredScreenId < Screen.AllScreens.Length)
            {
                // Pick the desired screen
                screenToShowOn = Screen.AllScreens[desiredScreenId];
            }
            else
            {
                // Pick the primary screen
                screenToShowOn = Screen.AllScreens.SingleOrDefault(s => s.Primary);
            }

            // Show the form on exactly this screen
            this.Width = screenToShowOn.WorkingArea.Width;
            this.Height = screenToShowOn.WorkingArea.Height;
            this.Left = screenToShowOn.WorkingArea.X;
            this.Top = screenToShowOn.WorkingArea.Y;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the event data.</param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!UIUtilities.ConfirmMessageBox(MessageBoxImage.Warning, AlarmWorkflow.Windows.UI.Properties.Resources.UIServiceExitWarning))
            {
                e.Cancel = true;
                return;
            }

            base.OnClosing(e);
        }

        #endregion

        #region Event handlers

        private void Window_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            uiScaleSlider.Value += 0.001d * e.Delta;
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // If this is the "acknowledge operation" key
            if (e.Key == App.GetApp().Configuration.AcknowledgeOperationKey)
            {
                _viewModel.AcknowledgeCurrentOperation(true);

                e.Handled = true;
            }
        }

        #endregion
    }
}
