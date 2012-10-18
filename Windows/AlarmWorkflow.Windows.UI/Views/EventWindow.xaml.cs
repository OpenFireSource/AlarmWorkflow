using System.Linq;
using System.Windows;
using System.Windows.Forms;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UI.ViewModels;

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
            _viewModel.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ViewModel_PropertyChanged);
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
            // Silently avoid being closed when there are events
            if (_viewModel.AvailableEvents.Count > 0)
            {
                e.Cancel = true;
                return;
            }

            base.OnClosing(e);
        }

        /// <summary>
        /// Returns whether or not the window contains an operation with the given id.
        /// </summary>
        /// <param name="operationId"></param>
        /// <returns></returns>
        public bool ContainsEvent(int operationId)
        {
            return _viewModel.AvailableEvents.Any(o => o.Operation.Id == operationId);
        }

        /// <summary>
        /// Pushes a new event to the window, either causing it to spawn, or to extend its list box by this event if already shown.
        /// </summary>
        /// <param name="operation">The event to push.</param>
        public void PushEvent(Operation operation)
        {
            if (_viewModel.PushEvent(operation))
            {
                // If the event was new, bring the window to front (sanity)
                this.Activate();
            }
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

        // Don't try this at home!
        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AvailableEvents" && _viewModel.AvailableEvents.Count == 0)
            {
                this.Close();
            }
        }

        #endregion
    }
}
