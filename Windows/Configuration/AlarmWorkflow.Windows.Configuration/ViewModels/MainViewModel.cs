using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Windows.UIContracts;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        #region Constants

        private const string ServiceExecutableName = "AlarmWorkflow.Windows.Service.exe";

        #endregion

        #region Fields

        private SettingsManager _manager;
        private SettingsDisplayConfiguration _displayConfiguration;
        private Dictionary<string, SectionViewModel> _sections;

        private Timer _serviceStatePollingTimer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list of all sections that can be edited.
        /// </summary>
        public IEnumerable<SectionViewModel> Sections
        {
            get { return _sections.Values; }
        }
        /// <summary>
        /// Gets the state of the service.
        /// </summary>
        public string ServiceState { get; private set; }

        #endregion

        #region Commands

        #region Command "SaveChangesCommand"

        /// <summary>
        /// The SaveChangesCommand command.
        /// </summary>
        public ICommand SaveChangesCommand { get; private set; }

        private void SaveChangesCommand_Execute(object parameter)
        {
            // Remember settings that failed to save
            int iFailedSettings = 0;
            // First apply the setting values from the editors back to their setting items.
            foreach (SectionViewModel svm in _sections.Values)
            {
                foreach (CategoryViewModel cvm in svm.CategoryItems)
                {
                    foreach (SettingItemViewModel sivm in cvm.SettingItems)
                    {
                        // Find setting
                        SettingItem item = _manager.GetSetting(sivm.SettingDescriptor.Identifier, sivm.SettingDescriptor.SettingItem.Name);
                        // Try to apply the value
                        object value = null;
                        try
                        {
                            value = sivm.TypeEditor.Value;
                            // If that succeeded, apply the value
                            item.SetValue(value);
                        }
                        catch (Exception ex)
                        {
                            string message = string.Format(Properties.Resources.SettingSaveError + "\n\n{2}", sivm.DisplayText, svm.DisplayText, ex.Message);
                            MessageBox.Show(message, "Fehler beim Speichern einer Einstellung", MessageBoxButton.OK, MessageBoxImage.Error);
                            iFailedSettings++;
                        }
                    }
                }
            }

            // Second, save the settings.
            _manager.SaveSettings();

            string message2 = (iFailedSettings == 0) ? Properties.Resources.SavingSettingsSuccess : Properties.Resources.SavingSettingsWithErrors;
            MessageBox.Show(message2, "Speichern", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region Command "OpenAppDataDirectoryCommand"

        /// <summary>
        /// The OpenAppDataDirectoryCommand command.
        /// </summary>
        public ICommand OpenAppDataDirectoryCommand { get; private set; }

        private void OpenAppDataDirectoryCommand_Execute(object parameter)
        {
            Process.Start(Utilities.GetLocalAppDataFolderPath());
        }

        #endregion

        #region Command "RestartServiceCommand"

        /// <summary>
        /// The RestartServiceCommand command.
        /// </summary>
        public ICommand RestartServiceCommand { get; private set; }

        private void RestartServiceCommand_Execute(object parameter)
        {
            if (!ServiceHelper.IsServiceInstalled())
            {
                UIUtilities.ShowWarning(Properties.Resources.ServiceIsNotInstalledError);
                return;
            }

            if (!UIUtilities.ConfirmMessageBox(MessageBoxImage.Warning, Properties.Resources.RestartServiceMessage))
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
            if (!Helper.IsCurrentUserAdministrator())
            {
                UIUtilities.ShowWarning(Properties.Resources.AdministratorRequiredMessage);
                return;
            }
            if (!UIUtilities.ConfirmMessageBox(MessageBoxImage.Question, Properties.Resources.ServiceInstallConfirmationMessage))
            {
                return;
            }

            try
            {
                ServiceHelper.InstallService();

                MessageBox.Show(Properties.Resources.ServiceInstallSuccessMessage);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(this, ex);
                UIUtilities.ShowWarning(Properties.Resources.ServiceInstallFailedMessage, ex.Message);
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
            if (!Helper.IsCurrentUserAdministrator())
            {
                UIUtilities.ShowWarning(Properties.Resources.AdministratorRequiredMessage);
                return;
            }
            if (ServiceHelper.IsServiceRunning())
            {
                UIUtilities.ShowWarning(Properties.Resources.ServiceUninstallErrorServiceIsRunningMessage);
                return;
            }

            if (!UIUtilities.ConfirmMessageBox(MessageBoxImage.Question, Properties.Resources.ServiceUninstallConfirmationMessage))
            {
                return;
            }

            try
            {
                ServiceHelper.UninstallService();

                MessageBox.Show(Properties.Resources.ServiceUninstallSuccessMessage);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(this, ex);
                UIUtilities.ShowWarning(Properties.Resources.ServiceUninstallFailedMessage, ex.Message);
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
            try
            {
                ServiceHelper.StartService(true);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(this, ex);
                UIUtilities.ShowWarning(Properties.Resources.ServiceStartError, ex.Message);
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
            try
            {
                ServiceHelper.StopService(true);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(this, ex);
                UIUtilities.ShowWarning(Properties.Resources.ServiceStopError, ex.Message);
            }
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            _manager = SettingsManager.Instance;
            _manager.Initialize(SettingsManager.SettingsInitialization.IncludeDisplayConfiguration);

            _displayConfiguration = _manager.GetSettingsDisplayConfiguration();
            BuildSectionsTree();

            _serviceStatePollingTimer = new Timer(2000d);
            _serviceStatePollingTimer.Elapsed += _serviceStatePollingTimer_Elapsed;
            _serviceStatePollingTimer.Start();
        }

        #endregion

        #region Methods

        private void BuildSectionsTree()
        {
            _sections = new Dictionary<string, SectionViewModel>();
            foreach (SettingDescriptor descriptor in _manager)
            {
                SectionViewModel svm = null;
                if (!_sections.TryGetValue(descriptor.Identifier, out svm))
                {
                    svm = new SectionViewModel(_displayConfiguration.GetIdentifier(descriptor.Identifier));
                    svm.Identifier = descriptor.Identifier;
                    _sections.Add(svm.Identifier, svm);
                }

                SettingInfo setting = _displayConfiguration.GetSetting(descriptor.Identifier, descriptor.SettingItem.Name);
                if (setting == null)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.SettingNotFoundInDisplayConfiguration, descriptor.SettingItem.Name, descriptor.Identifier);
                    continue;
                }
                svm.Add(descriptor, setting);
            }

            // Apply sorting
            ICollectionView view = CollectionViewSource.GetDefaultView(this.Sections);
            view.SortDescriptions.Add(new SortDescription("Order", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("DisplayText", ListSortDirection.Ascending));
        }

        private void _serviceStatePollingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                ServiceState = ServiceHelper.GetServiceState().ToString();
            }
            catch (Exception)
            {
                ServiceState = "NotInstalled";
            }
            OnPropertyChanged("ServiceState");
        }

        #endregion
    }
}
