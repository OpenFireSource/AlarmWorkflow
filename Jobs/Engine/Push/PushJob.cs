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
using System.Threading.Tasks;
using AlarmWorkflow.BackendService.AddressingContracts;
using AlarmWorkflow.BackendService.AddressingContracts.EntryObjects;
using AlarmWorkflow.BackendService.EngineContracts;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Job.PushJob
{
    [Export("PushJob", typeof(IJob))]
    [Information(DisplayName = "ExportJobDisplayName", Description = "ExportJobDescription")]
    class PushJob : IJob
    {
        #region Constants

        private const string ApplicationName = "Feuerwehr-Alarmierung";

        #endregion

        #region Fields

        private ISettingsServiceInternal _settings;
        private IAddressingServiceInternal _addressing;

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion

        #region IJobs Members

        bool IJob.Initialize(IServiceProvider serviceProvider)
        {
            _settings = serviceProvider.GetService<ISettingsServiceInternal>();
            _addressing = serviceProvider.GetService<IAddressingServiceInternal>();

            return true;
        }

        void IJob.Execute(IJobContext context, Operation operation)
        {
            if (context.Phase != JobPhase.AfterOperationStored)
            {
                return;
            }

            string header = _settings.GetSetting(SettingKeysJob.Header).GetValue<string>();

            string expression = _settings.GetSetting(SettingKeysJob.MessageContent).GetValue<string>();
            string message = operation.ToString(expression);

            Task.Factory.StartNew(() => SendToProwl(operation, message, header));
            Task.Factory.StartNew(() => SendToNotifyMyAndroid(operation, message, header));
        }

        bool IJob.IsAsync
        {
            get { return false; }
        }

        #endregion

        #region Methods

        private IEnumerable<string> GetRecipientApiKeysFor(Operation operation, string consumer)
        {
            var recipients = _addressing.GetCustomObjectsFiltered<PushEntryObject>(PushEntryObject.TypeId, operation);

            return recipients
                .Select(ri => ri.Item2)
                .Where(item => item.Consumer == consumer)
                .Select(item => item.RecipientApiKey);
        }

        private void SendToNotifyMyAndroid(Operation operation, string message, string header)
        {
            try
            {
                IEnumerable<string> recipients = GetRecipientApiKeysFor(operation, "NMA");

                if (recipients.Any())
                {
                    NotifyMyAndroid.SendNotifications(recipients, ApplicationName, header, message);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.ErrorNMA, ex.Message);
                Logger.Instance.LogException(this, ex);
            }
        }

        private void SendToProwl(Operation operation, string message, string header)
        {
            try
            {
                IEnumerable<string> recipients = GetRecipientApiKeysFor(operation, "Prowl");

                if (recipients.Any())
                {
                    Prowl.SendNotifications(recipients, ApplicationName, header, message);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.ErrorProwl, ex.Message);
                Logger.Instance.LogException(this, ex);
            }
        }

        #endregion
    }
}