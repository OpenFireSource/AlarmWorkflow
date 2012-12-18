using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        #region Fields

        private SettingsManager _manager;
        private SettingsDisplayConfiguration _displayConfiguration;
        private Dictionary<string, SectionViewModel> _sections;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list of all sections that can be edited.
        /// </summary>
        public IEnumerable<SectionViewModel> Sections
        {
            get { return _sections.Values; }
        }

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
        /// The OpenAppDataDirectoryCommand command.
        /// </summary>
        public ICommand RestartServiceCommand { get; private set; }

        private void RestartServiceCommand_Execute(object parameter)
        {
            ServiceController service = new ServiceController("AlarmworkflowService");
            try
            {
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(100));
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(this, ex);
            }
            try
            {
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(100));
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(this,ex);
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
            view.SortDescriptions.Add(new SortDescription("DisplayText", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Order", ListSortDirection.Ascending));
        }

        #endregion
    }
}
