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
using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.BackendService.SettingsContracts
{
    /// <summary>
    /// Defines the service interface to the settings service.
    /// </summary>
    public interface ISettingsServiceInternal : IInternalService
    {
        /// <summary>
        /// Occurs when the value of a setting has changed.
        /// </summary>
        event EventHandler<SettingChangedEventArgs> SettingChanged;

        /// <summary>
        /// Returns the <see cref="SettingsDisplayConfiguration"/>-instance describing all settings.
        /// </summary>
        /// <returns>The <see cref="SettingsDisplayConfiguration"/>-instance describing all settings.</returns>
        SettingsDisplayConfiguration GetDisplayConfiguration();
        /// <summary>
        /// Returns a specific setting by its parental identifier and name.
        /// </summary>
        /// <param name="identifier">The identifier of the setting. This is used to distinguish between the different setting configurations available.</param>
        /// <param name="name">The name of the setting within the configuration defined by <paramref name="identifier"/>.</param>
        /// <returns>The setting by its name.
        /// -or- null, if there was no setting by this name within the configuration defined by <paramref name="identifier"/>.</returns>
        SettingItem GetSetting(string identifier, string name);
        /// <summary>
        /// Returns a specific setting by its parental identifier and name.
        /// </summary>
        /// <param name="key">The <see cref="SettingKey"/>-instance describing the identifier and name of the setting to get.</param>
        /// <returns>The setting by its name.
        /// -or- null, if there was no setting by this name within the configuration defined by <paramref name="key"/>.</returns>
        SettingItem GetSetting(SettingKey key);
        /// <summary>
        /// Sets the given setting to a new value.
        /// </summary>
        /// <param name="identifier">The identifier of the setting. This is used to distinguish between the different setting configurations available.</param>
        /// <param name="name">The name of the setting within the configuration defined by <paramref name="identifier"/>.</param>
        /// <param name="value">The new value of the setting.</param>
        void SetSetting(string identifier, string name, SettingItem value);
        /// <summary>
        /// Sets the values of multiple settings in one batch.
        /// </summary>
        /// <param name="values">The values of the settings to set.</param>
        void SetSettings(IEnumerable<KeyValuePair<SettingKey, SettingItem>> values);
    }
}
