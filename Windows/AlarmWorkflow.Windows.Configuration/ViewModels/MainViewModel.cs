using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Windows.Configuration.Config;
using AlarmWorkflow.Windows.UI.ViewModels;

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
            // First apply the setting values from the editors back to their setting items.
            foreach (SectionViewModel svm in _sections.Values)
            {
                foreach (SettingItemViewModel sivm in svm.SettingItems)
                {
                    SettingItem item = _manager.GetSetting(sivm.SettingDescriptor.Identifier, sivm.SettingDescriptor.SettingItem.Name);
                    item.SetValue(sivm.TypeEditor.Value);
                }
            }

            // Second, save the settings.
            _manager.SaveSettings();

            MessageBox.Show("Die Einstellungen wurden erfolgreich gespeichert!", "Speichern", MessageBoxButton.OK, MessageBoxImage.Information);
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
            _manager.Initialize();

            _displayConfiguration = SettingsDisplayConfiguration.Load();

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
                svm.SettingItems.Add(new SettingItemViewModel(descriptor, setting));
            }

            // TODO: Sort the list afterwards
        }

        #endregion

        #region Methods

        #endregion
    }
}
