using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.ServiceContracts;
using AlarmWorkflow.Windows.UI.Extensibility;
using AlarmWorkflow.Windows.UI.Models;
using AlarmWorkflow.Windows.UI.Views;
using AlarmWorkflow.Windows.UIContracts;
using AlarmWorkflow.Windows.UIContracts.ViewModels;
using Hardcodet.Wpf.TaskbarNotification;
using AlarmWorkflow.Windows.UIContracts.Security;

namespace AlarmWorkflow.Windows.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Constants

        private const string MutexName = "$AlarmWorkflow.Windows.UI";

        #endregion

        #region Fields

        private System.Threading.Mutex _mutex;

        private TaskbarIcon _taskbarIcon;
        private DateTime _startedDate;

        private readonly object Lock = new object();
        private EventWindow _eventWindow;
        private System.Timers.Timer _timer;

        private bool _isMessageBoxShown;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the configuration for the UI.
        /// </summary>
        internal UIConfiguration Configuration { get; private set; }
        /// <summary>
        /// Gets the extension manager instance.
        /// </summary>
        internal ExtensionManager ExtensionManager { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// Calling this default constructor will cause an exception, because it is not allowed to start the UI manually
        /// (it must be started through the <see cref="UINotifyable"/> type)!
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">More than one instance of the <see cref="T:System.Windows.Application"/> class is created per <see cref="T:System.AppDomain"/>.</exception>
        public App()
        {
            if (Debugger.IsAttached)
            {
                Logger.Instance.RegisterListener(new DiagnosticsLoggingListener());
            }

            // Check mutex for existence (in which case we quit --> only one instance allowed!)
            try
            {
                Mutex.OpenExisting(MutexName);

                // error: since the mutex could be openend, this means another instance is already open!
                MessageBox.Show("Anwendung kann nicht zweimal gestartet werden!");
                App.Current.Shutdown();
                return;
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                // yay! let's create a mutex now! oh yeah, and start the app too.
                _mutex = new Mutex(false, MutexName);
            }

            AlarmWorkflow.Shared.Settings.SettingsManager.Instance.Initialize();
            LoadConfiguration();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the app.
        /// </summary>
        /// <returns></returns>
        internal static App GetApp()
        {
            return (App)App.Current;
        }

        private void LoadConfiguration()
        {
            try
            {
                Configuration = UIConfiguration.Load();
                Logger.Instance.LogFormat(LogType.Info, this, "The UI-configuration was successfully loaded.");
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, "The UI-configuration could not be loaded! Using default configuration.");
                Logger.Instance.LogException(this, ex);

                // Use default configuration
                Configuration = new UIConfiguration();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            if (_mutex == null)
            {
                return;
            }

            base.OnStartup(e);

            // Set shutdown mode to explicit to allow our application to run even if there are no windows open anymore.
            ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;

            _startedDate = DateTime.UtcNow;

            // Create taskbar icon
            _taskbarIcon = new TaskbarIcon();
            _taskbarIcon.IconSource = new BitmapImage(this.GetPackUri("Images/FaxHS.ico"));
            _taskbarIcon.ToolTipText = "AlarmWorkflow-UI Application is running...";

            _taskbarIcon.ContextMenu = new System.Windows.Controls.ContextMenu();
            _taskbarIcon.ContextMenu.Items.Add(new MenuItem()
            {
                Header = "Anwendung schließen",
                Command = new RelayCommand(LeftClickCommand_Execute),
            });

            // Create timer with a custom interval from configuration
            _timer = new System.Timers.Timer(Configuration.OperationFetchingArguments.Interval);
            _timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            _timer.Start();

            InitializeServices();
            ExtensionManager = new ExtensionManager();
        }

        /// <summary>
        /// Called when the application shuts down.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            if (_mutex != null)
            {
                _mutex.Dispose();
            }
        }

        /// <summary>
        /// Initializes the services and registers them at the ServiceProvider.
        /// </summary>
        private void InitializeServices()
        {
            // Credentials-confirmation dialog service
            ServiceProvider.Instance.AddService(typeof(ICredentialConfirmationDialogService), new Security.CredentialConfirmationDialogService());
        }

        private bool ContainsEvent(int operationId)
        {
            lock (Lock)
            {
                return _eventWindow != null && _eventWindow.ContainsEvent(operationId);
            }
        }

        private void PushEvent(OperationItem operation)
        {
            lock (Lock)
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    if (_eventWindow == null)
                    {
                        _eventWindow = new EventWindow();
                        _eventWindow.Closed += EventWindow_Closed;
                        _eventWindow.Show();
                    }

                    // Call the event window on this operation
                    _eventWindow.PushEvent(operation.ToOperation());

                }));
            }
        }

        private bool ShouldAutomaticallyAcknowledgeOperation(OperationItem operationItem)
        {
            if (!Configuration.AutomaticOperationAcknowledgement.IsEnabled)
            {
                return false;
            }

            int daage = Configuration.AutomaticOperationAcknowledgement.MaxAge;
            TimeSpan dat = daage > 0 ? TimeSpan.FromMinutes(daage) : Operation.DefaultAcknowledgingTimespan;
            return !operationItem.IsAcknowledged && (DateTime.UtcNow - operationItem.Timestamp) > dat;
        }

        #endregion

        #region Event handlers

        private void EventWindow_Closed(object sender, System.EventArgs e)
        {
            _eventWindow = null;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (Lock)
            {
                try
                {
                    using (var service = InternalServiceProxy.GetServiceInstance())
                    {
                        int maxAge = Configuration.OperationFetchingArguments.MaxAge;
                        bool onlyNonAcknowledged = Configuration.OperationFetchingArguments.OnlyNonAcknowledged;
                        int limitAmount = Configuration.OperationFetchingArguments.LimitAmount;

                        var operations = service.Instance.GetOperationIds(maxAge, onlyNonAcknowledged, limitAmount);
                        if (operations.Count == 0)
                        {
                            return;
                        }

                        foreach (int operationId in operations)
                        {
                            // Check if we already have this event (in this case don't retrieve it all over again)
                            if (ContainsEvent(operationId))
                            {
                                continue;
                            }

                            // Second parameter determines the detail level. Here, we can use "1" (full detail).
                            OperationItem operation = service.Instance.GetOperationById(operationId, OperationItemDetailLevel.Full);

                            // If the event is too old, do display it this time, but acknowledge it so it won't show up
                            if (ShouldAutomaticallyAcknowledgeOperation(operation))
                            {
                                service.Instance.AcknowledgeOperation(operation.Id);
                            }
                            else
                            {
                                // Push the event to the queue
                                PushEvent(operation);
                            }
                        }
                    }
                }
                catch (EndpointNotFoundException)
                {
                    // This is ok, since it also occurs when the service is starting up.
                }
                catch (Exception ex)
                {
                    // This could be interesting though...
                    Logger.Instance.LogException(this, ex);
                }

            }
        }

        private void LeftClickCommand_Execute(object parameter)
        {
            if (_isMessageBoxShown)
            {
                return;
            }

            _isMessageBoxShown = true;

            if (MessageBox.Show(AlarmWorkflow.Windows.UI.Properties.Resources.UIServiceExitWarning, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                this.Shutdown();
            }
            else
            {
                // Then re-enable topmost again... or not
                if (MainWindow != null)
                {
                    MainWindow.Activate();
                }
            }

            _isMessageBoxShown = false;
        }

        #endregion

    }
}
