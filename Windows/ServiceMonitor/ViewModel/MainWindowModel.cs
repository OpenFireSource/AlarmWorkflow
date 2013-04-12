using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ServiceMonitor.Helper;
using AlarmWorkflow.Windows.ServiceMonitor.Properties;
using AlarmWorkflow.Windows.UIContracts;
using AlarmWorkflow.Windows.UIContracts.ViewModels;
using Microsoft.Win32;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;

namespace AlarmWorkflow.Windows.ServiceMonitor.ViewModel
{
    internal class MainWindowModel : ViewModelBase
    {
        #region Fields

        private readonly DataGrid _dataGrid;
        private readonly Timer _serviceStatePollingTimer;
        private ObservableCollection<LoggingEvent> _events;
        private LogTypeEnum _logView;
        private bool _showLevelDebug;
        private bool _showLevelError;
        private bool _showLevelFatal;
        private bool _showLevelInfo;
        private bool _showLevelWarn;

        #endregion

        #region Command "RestartServiceCommand"

        /// <summary>
        /// The RestartServiceCommand command.
        /// </summary>
        public ICommand RestartServiceCommand { get; private set; }

        private void RestartServiceCommand_Execute(object parameter)
        {
            if (!ServiceHelper.IsCurrentUserAdministrator())
            {
                UIUtilities.ShowWarning(Resources.AdministratorRequiredMessage);
                return;
            }
            if (!ServiceHelper.IsServiceInstalled())
            {
                UIUtilities.ShowWarning(Resources.ServiceIsNotInstalledError);
                return;
            }

            if (!UIUtilities.ConfirmMessageBox(MessageBoxImage.Warning, Resources.RestartServiceMessage))
            {
                return;
            }

            ServiceHelper.StopService(false);
            ServiceHelper.StartService(false);
        }

        #endregion

        #region Command "InstallServiceCommand"

        /// <summary>
        /// The InstallServiceCommand command.
        /// </summary>
        public ICommand InstallServiceCommand { get; private set; }

        private bool InstallServiceCommand_CanExecute(object parameter)
        {
            return !ServiceHelper.IsServiceInstalled();
        }

        private void InstallServiceCommand_Execute(object parameter)
        {
            if (!ServiceHelper.IsCurrentUserAdministrator())
            {
                UIUtilities.ShowWarning(Resources.AdministratorRequiredMessage);
                return;
            }
            if (!UIUtilities.ConfirmMessageBox(MessageBoxImage.Question, Resources.ServiceInstallConfirmationMessage))
            {
                return;
            }

            try
            {
                ServiceHelper.InstallService();

                MessageBox.Show(Resources.ServiceInstallSuccessMessage);
            }
            catch (Exception ex)
            {
                UIUtilities.ShowWarning(Resources.ServiceInstallFailedMessage, ex.Message);
            }
        }

        #endregion

        #region Command "UninstallServiceCommand"

        /// <summary>
        /// The UninstallServiceCommand command.
        /// </summary>
        public ICommand UninstallServiceCommand { get; private set; }

        private bool UninstallServiceCommand_CanExecute(object parameter)
        {
            return ServiceHelper.IsServiceInstalled();
        }

        private void UninstallServiceCommand_Execute(object parameter)
        {
            if (!ServiceHelper.IsCurrentUserAdministrator())
            {
                UIUtilities.ShowWarning(Resources.AdministratorRequiredMessage);
                return;
            }
            if (ServiceHelper.IsServiceRunning())
            {
                UIUtilities.ShowWarning(Resources.ServiceUninstallErrorServiceIsRunningMessage);
                return;
            }

            if (!UIUtilities.ConfirmMessageBox(MessageBoxImage.Question, Resources.ServiceUninstallConfirmationMessage))
            {
                return;
            }
            try
            {
                ServiceHelper.UninstallService();

                MessageBox.Show(Resources.ServiceUninstallSuccessMessage);
            }
            catch (Exception ex)
            {
                UIUtilities.ShowWarning(Resources.ServiceUninstallFailedMessage, ex.Message);
            }
        }

        #endregion

        #region Command "StartServiceCommand"

        /// <summary>
        /// The StartServiceCommand command.
        /// </summary>
        public ICommand StartServiceCommand { get; private set; }

        private bool StartServiceCommand_CanExecute(object parameter)
        {
            return ServiceHelper.IsServiceInstalled() && !ServiceHelper.IsServiceRunning();
        }

        private void StartServiceCommand_Execute(object parameter)
        {
            if (!ServiceHelper.IsCurrentUserAdministrator())
            {
                UIUtilities.ShowWarning(Resources.AdministratorRequiredMessage);
                return;
            }
            try
            {
                ServiceHelper.StartService(true);
            }
            catch (Exception ex)
            {
                UIUtilities.ShowWarning(Resources.ServiceStartError, ex.Message);
            }
        }

        #endregion

        #region Command "StopServiceCommand"

        /// <summary>
        /// The StopServiceCommand command.
        /// </summary>
        public ICommand StopServiceCommand { get; private set; }

        private bool StopServiceCommand_CanExecute(object parameter)
        {
            return ServiceHelper.IsServiceInstalled() && ServiceHelper.IsServiceRunning();
        }

        private void StopServiceCommand_Execute(object parameter)
        {
            if (!ServiceHelper.IsCurrentUserAdministrator())
            {
                UIUtilities.ShowWarning(Resources.AdministratorRequiredMessage);
                return;
            }
            try
            {
                ServiceHelper.StopService(true);
            }
            catch (Exception ex)
            {
                UIUtilities.ShowWarning(Resources.ServiceStopError, ex.Message);
            }
        }

        #endregion

        #region Command "CopyCommand"

        /// <summary>
        /// The CopyCommand command.
        /// </summary>
        public ICommand CopyCommand { get; private set; }

        private bool CopyCommand_CanExecute(object parameter)
        {
            return _dataGrid.SelectedItems.Count != 0;
        }

        private void CopyCommand_Execute(object parameter)
        {
            StringBuilder selection = new StringBuilder();
            PatternLayout layout = new PatternLayout("%level;%date;%thread;%message");
            layout.ActivateOptions();
            IList selectedItems = _dataGrid.SelectedItems;
            foreach (LoggingEvent selectedItem in selectedItems)
            {
                selection.AppendLine(layout.Format(selectedItem));
            }
            Clipboard.SetText(selection.ToString());
        }

        #endregion

        #region Command "SwitchLogView"

        public ICommand SwitchLogView { get; private set; }

        private void SwitchLogView_Execute(object parameter)
        {
            _events = new ObservableCollection<LoggingEvent>();
            OnPropertyChanged("Events");
            switch (LogView)
            {
                case LogTypeEnum.Live:
                    // Create OpenFileDialog
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.InitialDirectory = Path.Combine(Utilities.GetLocalAppDataFolderPath(), "Logs");
                    dlg.DefaultExt = ".log";
                    dlg.Filter = "Log documents (.log)|*.log";
                    bool? result = dlg.ShowDialog();

                    if (result == true)
                    {
                        LogView = LogTypeEnum.File;
                        string filename = dlg.FileName;
                        try
                        {
                            IEnumerable<LoggingEvent> eventlist = XmlEntriesProvider.GetEntries(filename);
                            foreach (LoggingEvent loggingEvent in eventlist)
                            {
                                AddNewLoggingEvent(loggingEvent, this);
                            }
                        }
                        catch (Exception ex)
                        {
                            UIUtilities.ShowWarning(Resources.ReadFileError, filename, ex.Message);
                            LogView = LogTypeEnum.Live;
                        }
                    }
                    break;
                case LogTypeEnum.File:
                    LogView = LogTypeEnum.Live;
                    break;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the state of the service.
        /// </summary>
        public string ServiceState { get; private set; }

        public LogTypeEnum LogView
        {
            get { return _logView; }
            set
            {
                if (value != _logView)
                {
                    _logView = value;
                    OnPropertyChanged("LogView");
                }
            }
        }

        public ObservableCollection<LoggingEvent> Events
        {
            get { return _events; }
        }

        /// <summary>
        /// ShowLevelDebug Property
        /// </summary>
        public bool ShowLevelDebug
        {
            get { return _showLevelDebug; }
            set
            {
                if (value != _showLevelDebug)
                {
                    _showLevelDebug = value;
                    OnPropertyChanged("ShowLevelDebug");
                    Filter();
                }
            }
        }

        /// <summary>
        /// ShowLevelInfo Property
        /// </summary>
        public bool ShowLevelInfo
        {
            get { return _showLevelInfo; }
            set
            {
                if (value != _showLevelInfo)
                {
                    _showLevelInfo = value;
                    OnPropertyChanged("ShowLevelInfo");
                    Filter();
                }
            }
        }

        /// <summary>
        /// ShowLevelWarn Property
        /// </summary>
        public bool ShowLevelWarn
        {
            get { return _showLevelWarn; }
            set
            {
                if (value != _showLevelWarn)
                {
                    _showLevelWarn = value;
                    OnPropertyChanged("ShowLevelWarn");
                    Filter();
                }
            }
        }

        /// <summary>
        /// ShowLevelError Property
        /// </summary>
        public bool ShowLevelError
        {
            get { return _showLevelError; }
            set
            {
                if (value != _showLevelError)
                {
                    _showLevelError = value;
                    OnPropertyChanged("ShowLevelError");
                    Filter();
                }
            }
        }

        /// <summary>
        /// ShowLevelFatal Property
        /// </summary>
        public bool ShowLevelFatal
        {
            get { return _showLevelFatal; }
            set
            {
                if (value != _showLevelFatal)
                {
                    _showLevelFatal = value;
                    OnPropertyChanged("ShowLevelFatal");
                    Filter();
                }
            }
        }

        #endregion

        #region Methods

        private void Filter()
        {
            ICollectionView logView = CollectionViewSource.GetDefaultView(_dataGrid.ItemsSource);
            logView.Filter = LogFilter;
            logView.Refresh();
            OnPropertyChanged("Events");
        }

        private bool LogFilter(object item)
        {
            LoggingEvent loggingEvent = item as LoggingEvent;
            if (loggingEvent == null)
            {
                return false;
            }
            if (loggingEvent.Level == Level.Fatal && _showLevelFatal)
            {
                return true;
            }
            if (loggingEvent.Level == Level.Error && _showLevelError)
            {
                return true;
            }
            else if (loggingEvent.Level == Level.Warn && _showLevelWarn)
            {
                return true;
            }
            else if (loggingEvent.Level == Level.Debug && _showLevelDebug)
            {
                return true;
            }
            else if (loggingEvent.Level == Level.Info && _showLevelInfo)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal void AddNewLoggingEvent(LoggingEvent loggingEvent, object listAppender)
        {
            if (listAppender.GetType() == typeof (ListAppender) && LogView == LogTypeEnum.File)
            {
                return;
            }
            _events.Add(loggingEvent);
            ICollectionView view = CollectionViewSource.GetDefaultView(_dataGrid.ItemsSource);
            view.SortDescriptions.Clear();
            ListSortDirection? direction = _dataGrid.Columns[1].SortDirection;
            if (direction == null)
            {
                _dataGrid.Columns[1].SortDirection = ListSortDirection.Descending;
                direction = ListSortDirection.Descending;
            }
            view.SortDescriptions.Add(new SortDescription("TimeStamp", (ListSortDirection) direction));
            view.Filter = LogFilter;
            view.Refresh();
            switch (direction)
            {
                case ListSortDirection.Descending:
                    _dataGrid.ScrollIntoView(_dataGrid.Items[0]);
                    break;
                case ListSortDirection.Ascending:
                    _dataGrid.ScrollIntoView(_dataGrid.Items[_dataGrid.Items.Count - 1]);
                    break;
            }
            OnPropertyChanged("Events");
        }

        private void UpdateServiceState()
        {
            ServiceState = "NotInstalled";
            try
            {
                if (ServiceHelper.IsServiceInstalled())
                {
                    ServiceState = ServiceHelper.GetServiceState().ToString();
                }
            }
            catch (Exception)
            {
            }
            OnPropertyChanged("ServiceState");
        }

        #endregion

        #region Constructor

        public MainWindowModel(MainWindow mainWindow)
        {
            _dataGrid = mainWindow.DataGrid;
            _events = new ObservableCollection<LoggingEvent>();
            IAppender appender = new ListAppender(this);
            _serviceStatePollingTimer = new Timer(2000d);
            _serviceStatePollingTimer.Elapsed += _serviceStatePollingTimer_Elapsed;
            _serviceStatePollingTimer.Start();
            _showLevelDebug = _showLevelInfo = _showLevelWarn = _showLevelError = _showLevelFatal = true;
            BasicConfigurator.Configure(appender);
            ChannelServices.RegisterChannel(new TcpChannel(9090), false);
            RemotingConfiguration.RegisterWellKnownServiceType(new WellKnownServiceTypeEntry(typeof (RemoteSink), "LoggingSink", WellKnownObjectMode.SingleCall));
            UpdateServiceState();
            LogView = LogTypeEnum.Live;
        }

        #endregion

        #region Events

        private void _serviceStatePollingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateServiceState();
        }

        #endregion
    }

    #region Nested types

    internal enum LogTypeEnum
    {
        Live,
        File
    }


    internal class ListAppender : IAppender
    {
        private readonly object _lockObj = new object();
        private readonly MainWindowModel _mainWindowModel;

        public ListAppender(MainWindowModel mainWindowModel)
        {
            _mainWindowModel = mainWindowModel;
            ((IAppender) this).Name = "ListAppender";
        }

        void IAppender.Close()
        {
            //Nothing to do
        }

        void IAppender.DoAppend(LoggingEvent loggingEvent)
        {
            try
            {
                if (_mainWindowModel == null)
                {
                    return;
                }
                lock (_lockObj)
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => _mainWindowModel.AddNewLoggingEvent(loggingEvent, this)));
                }
            }
            catch (Exception ex)
            {
                UIUtilities.ShowWarning(Resources.LogAddError, ex.Message);
            }
        }

        string IAppender.Name { get; set; }
    }

    #endregion
}