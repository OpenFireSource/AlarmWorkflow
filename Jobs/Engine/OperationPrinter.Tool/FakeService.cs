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
using System.IO;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Shared.Specialized.Printing;
using System.Printing;

namespace AlarmWorkflow.Job.OperationPrinter.Tool
{
    /// <summary>
    /// Fake settings service for the operation printer tool
    /// </summary>
    class FakeSettingsService : ISettingsServiceInternal, IServiceProvider
    {
        private readonly string _defaultPrinter;

        internal FakeSettingsService()
        {
            using (var printServer = new LocalPrintServer())
            {
                _defaultPrinter = printServer.DefaultPrintQueue.FullName;
            }

        }

        #region Implementation of IDisposable

        public void Dispose()
        {
        }

        #endregion

        #region Implementation of IInternalService

        public void Initialize(IServiceProvider serviceProvider)
        {
        }

        public void OnStart()
        {
        }

        public void OnStop()
        {
        }

        #endregion

        #region Implementation of ISettingsServiceInternal

        public SettingsDisplayConfiguration GetDisplayConfiguration()
        {
            return null;
        }

        public SettingItem GetSetting(string identifier, string name)
        {
            return GetSetting(SettingKey.Create(identifier, name));
        }

        public SettingItem GetSetting(SettingKey key)
        {
            Console.WriteLine($"Requested setting {key.Identifier}.{key.Name}.");
            if (key.Name == "PrintingQueueNames")
            {
                return new SettingItem(key.Identifier, key.Name, _defaultPrinter, typeof(string));
            }
            if (key.Name == "PrintingQueuesConfiguration")
            {
                var setting = new PrintingQueuesConfiguration
                {
                    Entries = {new PrintingQueue {Name = _defaultPrinter, IsEnabled = true, CopyCount = 1, PrinterName = _defaultPrinter}}
                };
                
                return new SettingItem(key.Identifier, key.Name, (setting as IStringSettingConvertible).ConvertBack(), typeof(PrintingQueuesConfiguration)) ;
            }
            if (key.Name == "TemplateFile")
            {
                return new SettingItem(key.Identifier, key.Name, Path.GetFullPath(@"Resources\OperationPrintTemplate_NonStatic.htm"), typeof(string));
            }
            if (key.Name == "ScriptTimeout")
            {
                return new SettingItem(key.Identifier, key.Name, "60", typeof(int));
            }
            return null;
        }

        public void SetSetting(string identifier, string name, SettingItem value)
        {
        }

        public void SetSettings(IEnumerable<KeyValuePair<SettingKey, SettingItem>> values)
        {
        }

        public event EventHandler<SettingChangedEventArgs> SettingChanged;

        #endregion

        #region Implementation of IServiceProvider

        public object GetService(Type serviceType)
        {
            return this;
        }

        #endregion
    }
}