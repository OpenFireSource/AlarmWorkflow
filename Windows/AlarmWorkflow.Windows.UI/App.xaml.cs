using System;
using System.ServiceModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.Service.WcfServices;
using AlarmWorkflow.Windows.UI.Models;
using AlarmWorkflow.Windows.UI.ViewModels;
using AlarmWorkflow.Windows.UI.Views;
using Hardcodet.Wpf.TaskbarNotification;

namespace AlarmWorkflow.Windows.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields

        private TaskbarIcon _taskbarIcon;
        private DateTime _startedDate;

        private readonly object Lock = new object();
        private EventWindow _eventWindow;
        private Timer _timer;

        private bool _isMessageBoxShown;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the configuration for the UI.
        /// </summary>
        public UIConfiguration Configuration { get; private set; }

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
            LoadConfiguration();
        }

        #endregion

        #region Methods

        private void LoadConfiguration()
        {
            try
            {
                Configuration = UIConfiguration.Load();
                Logger.Instance.LogFormat(LogType.Info, this, "The UI-configuration was successfully loaded.");
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, "The UI-configuration could not be loaded!");
                Logger.Instance.LogException(this, ex);
            }
        }

        /// <summary>
        /// Gets the app.
        /// </summary>
        /// <returns></returns>
        internal static App GetApp()
        {
            return (App)App.Current;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

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

            // Create timer with an interval of 2 seconds
            _timer = new Timer(2000d);
            _timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            _timer.Start();
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
                    int maxAge = (_startedDate - DateTime.UtcNow).Minutes;
                    using (var service = ServiceFactory.GetServiceWrapper<IAlarmWorkflowService>())
                    {
                        // TODO: Make max entries customizable!
                        var operations = service.Instance.GetOperationIds(maxAge.ToString(), true.ToString(), 9.ToString());
                        if (operations.Count == 0)
                        {
                            return;
                        }

                        foreach (int operationId in operations)
                        {
                            // TODO: Make this better!
                            OperationItem operation = service.Instance.GetOperationById(operationId.ToString());

                            // If the event is too old, do display it this time, but acknowledge it so it won't show up
                            if (ShouldAutomaticallyAcknowledgeOperation(operation))
                            {
                                service.Instance.AcknowledgeOperation(operation.Id.ToString());
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
            // We need to disable Topmost otherwise the user can't see the 
            MainWindow.Topmost = false;

            if (MessageBox.Show(AlarmWorkflow.Windows.UI.Properties.Resources.UIServiceExitWarning, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                this.Shutdown();
            }
            else
            {
                // Then re-enable topmost again
                MainWindow.Topmost = true;
            }

            _isMessageBoxShown = false;
        }

        #endregion

    }
}
