using System.Windows;

namespace AlarmWorkflow.Windows.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Gets the app.
        /// </summary>
        /// <returns></returns>
        internal static App GetApp()
        {
            return (App)App.Current;
        }

        #region Fields

        private bool _isStartedThroughUINotifyable;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// Calling this default constructor will cause an exception, because it is not allowed to start the UI manually
        /// (it must be started through the <see cref="UINotifyable"/> type)!
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">More than one instance of the <see cref="T:System.Windows.Application"/> class is created per <see cref="T:System.AppDomain"/>.</exception>
        public App()
            : this(false)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        /// <param name="isStartedThroughUINotifyable">Whether or not the UI was started manually or through <see cref=""/>.</param>
        internal App(bool isStartedThroughUINotifyable)
            : base()
        {
            _isStartedThroughUINotifyable = isStartedThroughUINotifyable;
            if (!isStartedThroughUINotifyable)
            {
                MessageBox.Show("The UI may not be started manually!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Shutdown();
                return;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            if (!_isStartedThroughUINotifyable)
            {
                return;
            }

            base.OnStartup(e);

            //TaskbarIcon tb = (TaskbarIcon)FindResource("notificationIcon");
        }

        #endregion

    }
}
