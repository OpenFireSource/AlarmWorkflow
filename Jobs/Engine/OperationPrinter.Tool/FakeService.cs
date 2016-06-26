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

        public ISettingItem GetSetting(string identifier, string name)
        {
            return GetSetting(SettingKey.Create(identifier, name));
        }

        public ISettingItem GetSetting(SettingKey key)
        {
            Console.WriteLine($"Requested setting {key.Identifier}.{key.Name}.");
            if (key.Name == "PrintingQueueNames")
            {
                return new FakeItem { Identifier = key.Identifier, Name = key.Name, Value = _defaultPrinter, SettingType = typeof(string) };
            }
            if (key.Name == "PrintingQueuesConfiguration")
            {
                return new FakeItem { Identifier = key.Identifier, Name = key.Name, Value = new PrintingQueuesConfiguration { Entries = { new PrintingQueue { Name = _defaultPrinter, IsEnabled = true, CopyCount = 1, PrinterName = _defaultPrinter } } }, SettingType = typeof(PrintingQueuesConfiguration) };
            }
            if (key.Name == "TemplateFile")
            {
                return new FakeItem { Identifier = key.Identifier, Name = key.Name, Value = Path.GetFullPath(@"Resources\OperationPrintTemplate_NonStatic.htm"), SettingType = typeof(string) };
            }
            if (key.Name == "ScriptTimeout")
            {
                return new FakeItem { Identifier = key.Identifier, Name = key.Name, Value = 60, SettingType = typeof(int) };
            }
            return new FakeItem();
        }

        public void SetSetting(string identifier, string name, ISettingItem value)
        {
        }

        public void SetSettings(IEnumerable<KeyValuePair<SettingKey, ISettingItem>> values)
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