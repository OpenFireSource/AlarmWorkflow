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
using AlarmWorkflow.Shared.Addressing;
using AlarmWorkflow.Shared.Addressing.EntryObjects;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Engine;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Job.PushJob
{
    [Export("PushJob", typeof(IJob))]
    [Information(DisplayName = "ExportJobDisplayName", Description = "ExportJobDescription")]
    class PushJob : IJob
    {
        #region Constants

        private const string ApplicationName = "Feuerwehr-Alarmierung";
        private const string HeaderText = "Feuerwehralarm";

        #endregion

        #region Fields

        private string _expression;

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion

        #region IJobs Members

        bool IJob.Initialize()
        {
            _expression = SettingsManager.Instance.GetSetting("PushJob", "MessageContent").GetString();
            return true;
        }

        void IJob.Execute(IJobContext context, Operation operation)
        {
            if (context.Phase != JobPhase.AfterOperationStored)
            {
                return;
            }

            NotifyProwl(operation);
            NotifyMyAndroid(operation);
        }

        bool IJob.IsAsync
        {
            get { return true; }
        }

        #endregion

        #region Methods

        private IList<PushEntryObject> GetRecipients(Operation operation)
        {
            var recipients = AddressBookManager.GetInstance().GetCustomObjectsFiltered<PushEntryObject>(PushEntryObject.TypeId, operation);
            return recipients.Select(ri => ri.Item2).ToList();
        }

        private void NotifyMyAndroid(Operation operation)
        {
            string content = operation.ToString(_expression);
            List<String> nmaRecipients = (from pushEntryObject in GetRecipients(operation) where pushEntryObject.Consumer == "NMA" select pushEntryObject.RecipientApiKey).ToList();
            if (nmaRecipients.Count != 0)
            {
                NMA.Notify(nmaRecipients, ApplicationName, HeaderText, content, NMANotificationPriority.Emergency);
            }
        }

        private void NotifyProwl(Operation operation)
        {
            string content = operation.ToString(_expression);
            List<String> prowlRecipients = (from pushEntryObject in GetRecipients(operation) where pushEntryObject.Consumer == "Prowl" select pushEntryObject.RecipientApiKey).ToList();
            if (prowlRecipients.Count != 0)
            {
                Prowl.Notify(prowlRecipients, ApplicationName, HeaderText, content, ProwlNotificationPriority.Emergency);
            }
        }

        #endregion
    }
}