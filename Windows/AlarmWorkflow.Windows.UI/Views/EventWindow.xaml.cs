using System.Windows;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UI.ViewModels;

namespace AlarmWorkflow.Windows.UI.Views
{
    /// <summary>
    /// Interaction logic for EventWindow.xaml
    /// </summary>
    public partial class EventWindow : Window
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
        }

        #endregion

        #region Methods

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
        /// Pushes a new event to the window, either causing it to spawn, or to extend its list box by this event if already shown.
        /// </summary>
        /// <param name="operation">The event to push.</param>
        public void PushEvent(Operation operation)
        {
            _viewModel.PushEvent(operation);
        }

        /// <summary>
        /// Clears all events.
        /// </summary>
        public void ClearEvents()
        {
            _viewModel.ClearEvents();
        }

        #endregion

        #region Event handlers

        private void Window_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            // TODO
            uiScaleSlider.Value += 0.01d * e.Delta;
        }

        #endregion
    }
}
