using System.Windows;
using AlarmWorkflow.Windows.UI.ViewModels;
using AlarmWorkflow.Windows.UIContracts;

namespace AlarmWorkflow.Windows.UI.Views
{
    /// <summary>
    /// Interaction logic for EventWindow.xaml
    /// </summary>
    internal partial class MainWindow : Window
    {
        #region Fields

        private MainWindowViewModel _viewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainWindowViewModel();
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
            _viewModel.UiScaleFactor += 0.001d * e.Delta;
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
