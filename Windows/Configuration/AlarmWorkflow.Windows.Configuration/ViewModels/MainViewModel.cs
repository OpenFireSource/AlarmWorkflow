// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.Configuration.Views;
using AlarmWorkflow.Windows.UIContracts;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        #region Constants

        private const string ServiceExecutableName = "AlarmWorkflow.Windows.Service.exe";
        private const string SectionNameShared = "Shared";

        #endregion

        #region Fields

        private SettingsDisplayConfiguration _displayConfiguration;
        private List<GroupedSectionViewModel> _sections;

        private Timer _serviceStatePollingTimer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether or not the connection to the service is established.
        /// </summary>
        public bool IsConnected { get; private set; }
        /// <summary>
        /// Gets a list of all sections that can be edited.
        /// </summary>
        public IEnumerable<GroupedSectionViewModel> Sections
        {
            get { return _sections; }
        }
        /// <summary>
        /// Gets the state of the service.
        /// </summary>
        public string ServiceState { get; private set; }
        /// <summary>
        /// Gets whether or not the configuration editor running from the current location is configured
        /// as a server; that is, the backend configuration has an address specified like "localhost" or any starting with "127".
        /// </summary>
        public bool IsConfiguredAsServer { get; private set; }
        /// <summary>
        /// Gets whether or not the configuration editor running from the current location is configured
        /// as a client. This is the negated value from <see cref="IsConfiguredAsServer"/> and exists for convenience.
        /// </summary>
        public bool IsConfiguredAsClient
        {
            get { return !IsConfiguredAsServer; }
        }

        #endregion

        #region Commands

        #region Command "SaveChangesCommand"

        /// <summary>
        /// The SaveChangesCommand command.
        /// </summary>
        public ICommand SaveChangesCommand { get; private set; }

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

        private bool RestartServiceCommand_CanExecute(object parameter)
        {
            return IsConfiguredAsServer && ServiceHelper.IsServiceInstalled();
        }

        private void RestartServiceCommand_Execute(object parameter)
        {
            if (!Helper.IsCurrentUserAdministrator())
            {
                UIUtilities.ShowWarning(Properties.Resources.AdministratorRequiredMessage);
                return;
            }
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
            return IsConfiguredAsServer && !ServiceHelper.IsServiceInstalled();
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
            return IsConfiguredAsServer && ServiceHelper.IsServiceInstalled();
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
            return IsConfiguredAsServer && ServiceHelper.IsServiceInstalled() && !ServiceHelper.IsServiceRunning();
        }

        private void StartServiceCommand_Execute(object parameter)
        {
            if (!Helper.IsCurrentUserAdministrator())
            {
                UIUtilities.ShowWarning(Properties.Resources.AdministratorRequiredMessage);
                return;
            }

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
            return IsConfiguredAsServer && ServiceHelper.IsServiceInstalled() && ServiceHelper.IsServiceRunning();
        }

        private void StopServiceCommand_Execute(object parameter)
        {
            if (!Helper.IsCurrentUserAdministrator())
            {
                UIUtilities.ShowWarning(Properties.Resources.AdministratorRequiredMessage);
                return;
            }

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

        #region Command "ShowAboutWindowCommand"

        /// <summary>
        /// The ShowAboutWindowCommand command.
        /// </summary>
        public ICommand ShowAboutWindowCommand { get; private set; }

        private void ShowAboutWindowCommand_Execute(object parameter)
        {
            AboutWindow window = new AboutWindow();
            window.ShowDialog();
        }

        #endregion

        #region Command "OpenObjectExpressionTesterCommand"

        /// <summary>
        /// The OpenObjectExpressionTesterCommand command.
        /// </summary>
        public ICommand OpenObjectExpressionTesterCommand { get; private set; }

        private void OpenObjectExpressionTesterCommand_Execute(object parameter)
        {
            ObjectExpressionTesterWindow window = new ObjectExpressionTesterWindow();
            window.Show();
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            IsConfiguredAsServer = CheckIsConfiguredAsServer();

            InitializeSettings();

            _serviceStatePollingTimer = new Timer(2000d);
            _serviceStatePollingTimer.Elapsed += _serviceStatePollingTimer_Elapsed;
            _serviceStatePollingTimer.Start();

            SaveChangesCommand = new SaveSettingsTaskCommand(this);
        }

        #endregion

        #region Methods

        private bool CheckIsConfiguredAsServer()
        {
            try
            {
                string address = ServiceFactory.BackendConfigurator.Get("ServerHostAddress");
                if (!string.IsNullOrWhiteSpace(address))
                {
                    address = address.Trim().ToLowerInvariant();
                    return (address == "localhost") || address.StartsWith("127");
                }
                return false;
            }
            catch (FileNotFoundException ex)
            {
                Logger.Instance.LogException(this, ex);
                UIUtilities.ShowWarning(ex.Message);

                return false;
            }
        }

        private void InitializeSettings()
        {
            try
            {
                using (var service = ServiceFactory.GetCallbackServiceWrapper<ISettingsService>(new SettingsServiceCallback()))
                {
                    _displayConfiguration = service.Instance.GetDisplayConfiguration();
                    BuildSectionsTree(service.Instance);

                    IsConnected = true;
                }
            }
            catch (EndpointNotFoundException ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.EndpointNotFoundOnStart);
                Logger.Instance.LogException(this, ex);

                if (IsConfiguredAsClient)
                {
                    UIUtilities.ShowWarning(Properties.Resources.EndpointNotFoundOnStart);
                }

            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.SettingsInitializationError);
                Logger.Instance.LogException(this, ex);

                UIUtilities.ShowWarning(Properties.Resources.SettingsInitializationError);
            }
        }

        private void BuildSectionsTree(ISettingsService settings)
        {
            Dictionary<string, SectionViewModel> sectionsTemp = new Dictionary<string, SectionViewModel>();
            Dictionary<string, string> sectionParents = new Dictionary<string, string>();

            foreach (IdentifierInfo identifier in _displayConfiguration.Identifiers)
            {
                SectionViewModel svm = null;
                if (!sectionsTemp.TryGetValue(identifier.Name, out svm))
                {
                    sectionParents[identifier.Name] = identifier.Parent;

                    svm = new SectionViewModel(identifier);
                    sectionsTemp.Add(svm.Identifier, svm);
                }

                foreach (SettingInfo info in identifier.Settings)
                {
                    SettingItem setting = settings.GetSetting(info.CreateSettingKey());
                    svm.Add(info, setting);
                }
            }

            _sections = new List<GroupedSectionViewModel>();
            // Create hierarchy, starting with the sections that don't have parents
            foreach (var kvp in sectionParents.OrderBy(k => k.Value))
            {
                SectionViewModel section = sectionsTemp.FirstOrDefault(s => s.Key == kvp.Key).Value;
                string parentName = kvp.Value;
                bool hasParent = (parentName != null);

                // Create group for this item
                GroupedSectionViewModel group = new GroupedSectionViewModel(section);

                if (hasParent)
                {
                    // Find parent group
                    // TODO: If there is no parent group, create a dummy group
                    GroupedSectionViewModel parentGroup = GetAllSections().FirstOrDefault(s => s.Identifier == parentName);
                    if (parentGroup == null)
                    {
                        // Create a dummy group
                        parentGroup = new GroupedSectionViewModel(null);
                        parentGroup.Identifier = parentName;
                        parentGroup.Header = parentName;
                        _sections.Add(parentGroup);
                    }

                    parentGroup.Children.Add(group);
                }
                else
                {
                    _sections.Add(group);
                }
            }

            // Always select the "Shared" section
            _sections.First(s => s.Identifier == SectionNameShared).IsSelected = true;

            ICollectionView view = CollectionViewSource.GetDefaultView(_sections);
            view.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Order", ListSortDirection.Ascending));
        }

        internal IEnumerable<GroupedSectionViewModel> GetAllSections()
        {
            List<GroupedSectionViewModel> all = new List<GroupedSectionViewModel>();
            all.AddRange(_sections);
            all.AddRange(_sections.SelectMany(s => s.Children));
            return all;
        }

        private void _serviceStatePollingTimer_Elapsed(object sender, ElapsedEventArgs e)
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
    }
}