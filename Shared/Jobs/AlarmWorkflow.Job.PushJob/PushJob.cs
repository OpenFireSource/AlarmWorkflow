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
    [Export("PushJob", typeof (IJob))]
    public class PushJob : IJob
    {
        private string _expression;
        private List<PushEntryObject> _recipients;

        void IDisposable.Dispose()
        {
        }

        bool IJob.Initialize()
        {
            _recipients = new List<PushEntryObject>();
            var recipients = AddressBookManager.GetInstance().GetCustomObjects<PushEntryObject>("Push");
            _recipients.AddRange(recipients.Select(ri => ri.Item2));
            _expression = SettingsManager.Instance.GetSetting("PushJob", "MessageContent").GetString();
            return true;
        }

        void IJob.Execute(IJobContext context, Operation operation)
        {
            Prowl(operation);
            NotifyMyAndroid(operation);
            
        }

        bool IJob.IsAsync
        {
            get { return true; }
        }

       

        private void NotifyMyAndroid(Operation operation)
        {
            NMA sender = new NMA();
            String content = operation.ToString(_expression);
            List<String> nmaRecipients = (from pushEntryObject in _recipients where pushEntryObject.Consumer == "NMA" select pushEntryObject.RecipientApiKey).ToList();
            if (nmaRecipients.Count != 0)
            {
                sender.Notify(nmaRecipients, "Feuerwehr-Allarmierung", "Feuerwehralarm", content, NMA.NMANotificationPriority.Emergency);
            }
        }

        private void Prowl(Operation operation)
        {
            Prowl sender = new Prowl();
            String content = operation.ToString(_expression);
            List<String> prowlRecipients = (from pushEntryObject in _recipients where pushEntryObject.Consumer == "Prowl" select pushEntryObject.RecipientApiKey).ToList();
            if (prowlRecipients.Count != 0)
            {
                sender.Notify(prowlRecipients, "Feuerwehr-Allarmierung", "Feuerwehralarm", content, Job.PushJob.Prowl.ProwlNotificaionPriortiy.Emergency);
            }
        }
    }
}