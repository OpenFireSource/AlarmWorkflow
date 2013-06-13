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
            _viewModel.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_viewModel_PropertyChanged);
            this.DataContext = _viewModel;

            this.Loaded += Window_Loaded;

            SetWindowPosition();
            SetContent(false);
        }

        #endregion

        #region Methods

        private void SetWindowPosition()
        {
            var pos = Properties.Settings.Default.WindowPosition;
            this.Top = (double)pos.Top;
            this.Left = (double)pos.Left;
            this.Width = (double)pos.Width;
            this.Height = (double)pos.Height;
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

            SaveWindowPosition();
            base.OnClosing(e);
        }

        private void SaveWindowPosition()
        {
            var rectangle = Helper.GetWindowRect(this);
            Properties.Settings.Default.WindowPosition = rectangle;
            Properties.Settings.Default.WindowMaximized = this.WindowState == System.Windows.WindowState.Maximized;
        }

        private void SetContent(bool alarmsAvailable)
        {
            if (alarmsAvailable)
            {
                this.content.Content = new ContentAlarmsAvailableControl();
            }
            else
            {
                this.content.Content = new ContentNoAlarmsControl();
            }
        }

        #endregion

        #region Event handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = Properties.Settings.Default.WindowMaximized ? WindowState.Maximized : WindowState.Normal;
        }

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

        private void _viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HasDisplayableEvents")
            {
                SetContent(_viewModel.HasDisplayableEvents);
            }
        }

        #endregion
    }
}
