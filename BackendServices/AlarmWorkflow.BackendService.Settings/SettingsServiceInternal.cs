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
using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.Backend.ServiceContracts.Data;
using AlarmWorkflow.BackendService.Settings.Data;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.BackendService.Settings
{
    class SettingsServiceInternal : InternalServiceBase, ISettingsServiceInternal
    {
        #region Constants

        private const string EdmxPath = "Data.Entities";

        #endregion

        #region Fields

        private SettingsCollection _settings;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsServiceInternal"/> class.
        /// </summary>
        public SettingsServiceInternal()
            : base()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// When overridden in a derived class, allows for a custom initialization procedure.
        /// At this point, the <see cref="P:AlarmWorkflow.Backend.ServiceContracts.Core.BackendServiceBase.ServiceProvider"/> has already been set.
        /// </summary>
        protected override void InitializeOverride()
        {
            base.InitializeOverride();

            _settings = new SettingsCollection();
        }

        private void OnSettingChanged(SettingChangedEventArgs e)
        {
            var copy = SettingChanged;
            if (copy != null)
            {
                copy(this, e);
            }
        }

        #endregion

        #region ISettingsServiceInternal Members

        /// <summary>
        /// Occurs when the value of a setting has changed.
        /// </summary>
        public event EventHandler<SettingChangedEventArgs> SettingChanged;

        SettingsDisplayConfiguration ISettingsServiceInternal.GetDisplayConfiguration()
        {
            try
            {
                return _settings.SettingsDisplayConfiguration;
            }
            catch (Exception ex)
            {
                throw AlarmWorkflowFaultDetails.CreateFault(ex);
            }
        }

        SettingItem ISettingsServiceInternal.GetSetting(string identifier, string name)
        {
            try
            {
                Assertions.AssertNotEmpty(identifier, "identifier");
                Assertions.AssertNotEmpty(name, "name");

                SettingItem item = _settings.GetSetting(identifier, name);

                lock (SyncRoot)
                {
                    ApplySettingValue(identifier, name, item);
                }

                return item;
            }
            catch (Exception ex)
            {
                throw AlarmWorkflowFaultDetails.CreateFault(ex);
            }
        }

        private static void ApplySettingValue(string identifier, string name, SettingItem item)
        {
            using (SettingsEntities entities = EntityFrameworkHelper.CreateContext<SettingsEntities>(EdmxPath))
            {
                UserSettingData userData = entities.GetUserSettingData(identifier, name);

                if (userData != null)
                {
                    item.ApplyUserValue(userData.Value);
                }
            }
        }

        SettingItem ISettingsServiceInternal.GetSetting(SettingKey key)
        {
            try
            {
                Assertions.AssertNotNull(key, "key");

                SettingItem item = _settings.GetSetting(key.Identifier, key.Name);

                lock (SyncRoot)
                {
                    ApplySettingValue(key.Identifier, key.Name, item);
                }

                return item;
            }
            catch (Exception ex)
            {
                throw AlarmWorkflowFaultDetails.CreateFault(ex);
            }
        }

        void ISettingsServiceInternal.SetSetting(string identifier, string name, SettingItem value)
        {
            try
            {
                Assertions.AssertNotEmpty(identifier, "identifier");
                Assertions.AssertNotEmpty(name, "name");

                lock (SyncRoot)
                {
                    using (SettingsEntities entities = EntityFrameworkHelper.CreateContext<SettingsEntities>(EdmxPath))
                    {
                        UserSettingData userSetting = entities.GetUserSettingData(identifier, name);
                        if (userSetting == null)
                        {
                            userSetting = new UserSettingData();
                            userSetting.Identifier = identifier;
                            userSetting.Name = name;
                            entities.UserSettings.AddObject(userSetting);
                        }

                        string valueToPersist = ((IProxyType<string>)value).ProxiedValue;
                        if (string.Equals(userSetting.Value, valueToPersist, StringComparison.Ordinal))
                        {
                            return;
                        }

                        userSetting.Value = valueToPersist;
                        entities.SaveChanges();
                    }
                }

                SettingKey key = SettingKey.Create(identifier, name);
                OnSettingChanged(new SettingChangedEventArgs(key));
            }
            catch (Exception ex)
            {
                throw AlarmWorkflowFaultDetails.CreateFault(ex);
            }
        }

        #endregion
    }
}
