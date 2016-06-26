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
using AlarmWorkflow.Backend.Data;
using AlarmWorkflow.Backend.Data.Types;
using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.BackendService.Settings
{
    class SettingsServiceInternal : InternalServiceBase, ISettingsServiceInternal
    {
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
        /// </summary>
        protected override void InitializeOverride()
        {
            base.InitializeOverride();

            _settings = new SettingsCollection();
        }

        private void OnSettingChanged(SettingChangedEventArgs e)
        {
            SettingChanged?.Invoke(this, e);
        }

        #endregion

        #region ISettingsServiceInternal Members

        /// <summary>
        /// Occurs when the value of a setting has changed.
        /// </summary>
        public event EventHandler<SettingChangedEventArgs> SettingChanged;

        SettingsDisplayConfiguration ISettingsServiceInternal.GetDisplayConfiguration()
        {
            return _settings.SettingsDisplayConfiguration;
        }

        ISettingItem ISettingsServiceInternal.GetSetting(string identifier, string name)
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

        private void ApplySettingValue(string identifier, string name, SettingItem item)
        {
            using (IUnitOfWork work = ServiceProvider.GetService<IDataContextFactory>().Get().Create())
            {
                SettingData setting = work.For<SettingData>().Query.FirstOrDefault(_ => _.Identifier == identifier && _.Name == name);

                if (setting != null)
                {
                    item.ApplyUserValue(setting.Value);
                }
            }
        }

        ISettingItem ISettingsServiceInternal.GetSetting(SettingKey key)
        {
            Assertions.AssertNotNull(key, "key");

            SettingItem item = _settings.GetSetting(key.Identifier, key.Name);

            lock (SyncRoot)
            {
                ApplySettingValue(key.Identifier, key.Name, item);
            }

            return item;
        }

        void ISettingsServiceInternal.SetSetting(string identifier, string name, ISettingItem value)
        {
            Assertions.AssertNotEmpty(identifier, "identifier");
            Assertions.AssertNotEmpty(name, "name");

            IEnumerable<SettingKey> savedSettings = null;

            lock (SyncRoot)
            {
                SettingKey key = SettingKey.Create(identifier, name);

                Dictionary<SettingKey, ISettingItem> settings = new Dictionary<SettingKey, ISettingItem>();
                settings.Add(key, value);

                savedSettings = SaveSettings(settings);
            }

            if (savedSettings != null && savedSettings.Any())
            {
                OnSettingChanged(new SettingChangedEventArgs(savedSettings));
            }
        }

        private IList<SettingKey> SaveSettings(IEnumerable<KeyValuePair<SettingKey, ISettingItem>> values)
        {
            List<SettingKey> keys = new List<SettingKey>();

            using (IUnitOfWork work = ServiceProvider.GetService<IDataContextFactory>().Get().Create())
            {
                IRepository<SettingData> repository = work.For<SettingData>();

                foreach (var pair in values)
                {
                    SettingKey key = pair.Key;
                    IProxyType<string> value = pair.Value;

                    try
                    {
                        if (AddOrUpdateSetting(repository, key, value))
                        {
                            keys.Add(key);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.SettingAddOrUpdateError, key);
                        Logger.Instance.LogException(this, ex);
                    }
                }

                work.Commit();
            }

            return keys;
        }

        private static bool AddOrUpdateSetting(IRepository<SettingData> repository, SettingKey key, IProxyType<string> value)
        {
            SettingData setting = repository.Query.FirstOrDefault(_ => _.Identifier == key.Identifier && _.Name == key.Name);

            if (setting == null)
            {
                setting = new SettingData();
                setting.Identifier = key.Identifier;
                setting.Name = key.Name;

                repository.Insert(setting);
            }

            string valueToPersist = value.ProxiedValue;
            if (!string.Equals(setting.Value, valueToPersist, StringComparison.Ordinal))
            {
                setting.Value = valueToPersist;

                return true;
            }

            return false;
        }

        void ISettingsServiceInternal.SetSettings(IEnumerable<KeyValuePair<SettingKey, ISettingItem>> values)
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

        #endregion
    }
}
