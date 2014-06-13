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
using System.Linq;
using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.BackendService.ManagementContracts;
using AlarmWorkflow.BackendService.ManagementContracts.Emk;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.Management
{
    class EmkServiceInternal : InternalServiceBase, IEmkServiceInternal
    {
        #region Constants

        private const string EdmxPath = "Data.EmkEntities";

        #endregion

        #region Fields

        private EmkResourceCollection _emkResources;

        #endregion

        #region Properties

        private ISettingsServiceInternal Settings
        {
            get { return this.ServiceProvider.GetService<ISettingsServiceInternal>(); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Overridden to register with services on start.
        /// </summary>
        public override void OnStart()
        {
            base.OnStart();

            ReloadFromSettings();
            Settings.SettingChanged += Settings_SettingChanged;
        }

        /// <summary>
        /// Overridden to clean up on stop.
        /// </summary>
        public override void OnStop()
        {
            base.OnStop();

            Settings.SettingChanged -= Settings_SettingChanged;
        }

        private void ReloadFromSettings()
        {
            lock (SyncRoot)
            {
                _emkResources = Settings.GetSetting(SettingKeys.Emk).GetValue<EmkResourceCollection>();
            }
        }

        private void Settings_SettingChanged(object sender, SettingChangedEventArgs e)
        {
            if (e.Keys.Contains(SettingKeys.Emk))
            {
                ReloadFromSettings();
            }
        }

        private bool HasAnyActiveResourcesConfigured()
        {
            return _emkResources.Any(item => item.IsActive);
        }

        #endregion

        #region IEmkServiceInternal Members

        IEnumerable<EmkResource> IEmkServiceInternal.GetAllResources()
        {
            lock (SyncRoot)
            {
                foreach (EmkResource item in _emkResources)
                {
                    yield return item;
                }
            }
        }

        IEnumerable<OperationResource> IEmkServiceInternal.GetFilteredResources(IEnumerable<OperationResource> resources)
        {
            lock (SyncRoot)
            {
                // We only want to filter the active resources. Yet if the user hasn't configured any active items, deliver everything.
                Func<OperationResource, bool> filter = null;
                if (HasAnyActiveResourcesConfigured())
                {
                    filter = item => _emkResources.ContainsMatch(item);
                }
                else
                {
                    filter = item => true;
                }

                foreach (OperationResource candidate in resources.Where(filter))
                {
                    yield return candidate;
                }
            }
        }

        #endregion
    }
}
