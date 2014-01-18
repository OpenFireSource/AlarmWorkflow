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
using System.ServiceModel;
using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.BackendService.SettingsContracts
{
    /// <summary>
    /// Defines the exposed interface to the settings service.
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ISettingsServiceCallback))]
    public interface ISettingsService : IExposedService
    {
        /// <summary>
        /// Returns the <see cref="SettingsDisplayConfiguration"/>-instance describing all settings.
        /// </summary>
        /// <returns>The <see cref="SettingsDisplayConfiguration"/>-instance describing all settings.</returns>
        [OperationContract()]
        [FaultContract(typeof(AlarmWorkflowFaultDetails))]
        SettingsDisplayConfiguration GetDisplayConfiguration();
        /// <summary>
        /// Returns a specific setting by its parental identifier and name.
        /// </summary>
        /// <param name="key">The <see cref="SettingKey"/>-instance describing the identifier and name of the setting to get.</param>
        /// <returns>The setting by its name.
        /// -or- null, if there was no setting by this name within the configuration defined by <paramref name="key"/>.</returns>
        [OperationContract()]
        [FaultContract(typeof(AlarmWorkflowFaultDetails))]
        SettingItem GetSetting(SettingKey key);
        /// <summary>
        /// Sets the given setting to a new value.
        /// </summary>
        /// <param name="key">The <see cref="SettingKey"/>-instance describing the identifier and name of the setting to set.</param>
        /// <param name="value">The new value of the setting.</param>
        [OperationContract()]
        [FaultContract(typeof(AlarmWorkflowFaultDetails))]
        void SetSetting(SettingKey key, SettingItem value);
        /// <summary>
        /// Sets the values of multiple settings in one batch.
        /// </summary>
        /// <param name="values">The values of the settings to set.</param>
        [OperationContract()]
        [FaultContract(typeof(AlarmWorkflowFaultDetails))]
        void SetSettings(ICollection<KeyValuePair<SettingKey, SettingItem>> values);
    }
}
