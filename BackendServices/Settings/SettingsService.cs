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
using System.Linq;
using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.BackendService.Settings
{
    class SettingsService : ExposedCallbackServiceBase<ISettingsServiceCallback>, ISettingsService
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsService"/> class.
        /// </summary>
        public SettingsService()
        {
            this.ServiceProvider.GetService<ISettingsServiceInternal>().SettingChanged += SettingsService_SettingChanged;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Overridden to dispose of used resources.
        /// </summary>
        protected override void DisposeCore()
        {
            base.DisposeCore();

            this.ServiceProvider.GetService<ISettingsServiceInternal>().SettingChanged -= SettingsService_SettingChanged;
        }

        private void SettingsService_SettingChanged(object sender, SettingChangedEventArgs e)
        {
            this.Callback.OnSettingChanged(e.Keys.ToList());
        }

        #endregion

        #region ISettingsService Members

        SettingsDisplayConfiguration ISettingsService.GetDisplayConfiguration()
        {
            return this.ServiceProvider.GetService<ISettingsServiceInternal>().GetDisplayConfiguration();
        }

        SettingItem ISettingsService.GetSetting(SettingKey key)
        {
            return this.ServiceProvider.GetService<ISettingsServiceInternal>().GetSetting(key);
        }

        void ISettingsService.SetSetting(SettingKey key, SettingItem value)
        {
            this.ServiceProvider.GetService<ISettingsServiceInternal>().SetSetting(key.Identifier, key.Name, value);
        }

        void ISettingsService.SetSettings(ICollection<KeyValuePair<SettingKey, SettingItem>> values)
        {
            this.ServiceProvider.GetService<ISettingsServiceInternal>().SetSettings(values);
        }

        #endregion
    }
}
