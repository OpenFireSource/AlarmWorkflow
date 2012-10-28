using System.Collections.Generic;

namespace AlarmWorkflow.Shared.Settings
{
    /// <summary>
    /// Represents the "settings.xml" file.
    /// </summary>
    sealed class SettingsConfigurationFile : IEnumerable<SettingItem>
    {
        #region Fields

        private Dictionary<string, SettingItem> _settings;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier of this settings configuration file.
        /// </summary>
        internal string Identifier { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsConfigurationFile"/> class.
        /// </summary>
        /// <param name="identifier">The identifier of this settings configuration file.</param>
        /// <param name="settings">The settings.</param>
        internal SettingsConfigurationFile(string identifier, IEnumerable<SettingItem> settings)
        {
            this.Identifier = identifier;

            _settings = new Dictionary<string, SettingItem>();
            foreach (SettingItem item in settings)
            {
                _settings[item.Name] = item;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the setting by its name, or null, if the setting was not found.
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        internal SettingItem GetSetting(string settingName)
        {
            if (_settings.ContainsKey(settingName))
            {
                return _settings[settingName];
            }
            return null;
        }

        #endregion

        #region IEnumerable<SettingItem> Members

        IEnumerator<SettingItem> IEnumerable<SettingItem>.GetEnumerator()
        {
            return _settings.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _settings.Values.GetEnumerator();
        }

        #endregion
    }
}
