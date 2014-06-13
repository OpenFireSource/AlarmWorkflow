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
using System.Data;
using System.Linq;
using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.Backend.ServiceContracts.Data;
using AlarmWorkflow.BackendService.Settings.Data;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
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

                IEnumerable<SettingKey> savedSettings = null;

                lock (SyncRoot)
                {
                    SettingKey key = SettingKey.Create(identifier, name);

                    List<KeyValuePair<SettingKey, SettingItem>> settings = new List<KeyValuePair<SettingKey, SettingItem>>();
                    settings.Add(new KeyValuePair<SettingKey, SettingItem>(key, value));

                    savedSettings = SaveSettings(settings);
                }

                if (savedSettings != null && savedSettings.Any())
                {
                    OnSettingChanged(new SettingChangedEventArgs(savedSettings));
                }
            }
            catch (Exception ex)
            {
                throw AlarmWorkflowFaultDetails.CreateFault(ex);
            }
        }

        private IList<SettingKey> SaveSettings(IEnumerable<KeyValuePair<SettingKey, SettingItem>> values)
        {
            List<SettingKey> keys = new List<SettingKey>();

            using (SettingsEntities entities = EntityFrameworkHelper.CreateContext<SettingsEntities>(EdmxPath))
            {
                foreach (var pair in values)
                {
                    SettingKey key = pair.Key;
                    IProxyType<string> value = pair.Value;

                    try
                    {
                        if (AddOrUpdateSetting(entities, key, value))
                        {
                            keys.Add(key);
                        }
                    }
                    catch (ConstraintException ex)
                    {
                        Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.SettingAddOrUpdateConstraintError, key);
                        Logger.Instance.LogException(this, ex);
                    }
                    catch (DataException ex)
                    {
                        Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.SettingAddOrUpdateError, key);
                        Logger.Instance.LogException(this, ex);
                    }
                }

                if (keys.Count > 0)
                {
                    entities.SaveChanges();
                }
            }

            return keys;
        }

        private static bool AddOrUpdateSetting(SettingsEntities entities, SettingKey key, IProxyType<string> value)
        {
            UserSettingData userSetting = entities.GetUserSettingData(key.Identifier, key.Name);
            if (userSetting == null)
            {
                userSetting = new UserSettingData();
                userSetting.Identifier = key.Identifier;
                userSetting.Name = key.Name;
                entities.UserSettings.AddObject(userSetting);
            }

            string valueToPersist = value.ProxiedValue;
            if (!string.Equals(userSetting.Value, valueToPersist, StringComparison.Ordinal))
            {
                userSetting.Value = valueToPersist;

                return true;
            }

            return false;
        }

        void ISettingsServiceInternal.SetSettings(IEnumerable<KeyValuePair<SettingKey, SettingItem>> values)
        {
            try
            {
                Assertions.AssertNotNull(values, "values");

                IEnumerable<SettingKey> savedSettings = null;

                lock (SyncRoot)
                {
                    savedSettings = SaveSettings(values);
                }

                if (savedSettings != null && savedSettings.Any())
                {
                    OnSettingChanged(new SettingChangedEventArgs(savedSettings));
                }
            }
            catch (Exception ex)
            {
                throw AlarmWorkflowFaultDetails.CreateFault(ex);
            }
        }

        #endregion
    }
}
