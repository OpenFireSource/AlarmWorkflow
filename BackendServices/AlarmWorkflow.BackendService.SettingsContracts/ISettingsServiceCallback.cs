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

using System.ServiceModel;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.BackendService.SettingsContracts
{
    /// <summary>
    /// Defines the methods for the settings service callback.
    /// </summary>
    public interface ISettingsServiceCallback
    {
        /// <summary>
        /// Called when the value of a setting has changed.
        /// </summary>
        /// <param name="key">The <see cref="SettingKey"/>-instance describing the identifier and name of the changed setting.</param>
        [OperationContract(IsOneWay = true)]
        void OnSettingChanged(SettingKey key);
    }
}
