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

using System.Collections.Generic;
using AlarmWorkflow.BackendService.SettingsContracts;

namespace AlarmWorkflow.BackendService.Settings
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