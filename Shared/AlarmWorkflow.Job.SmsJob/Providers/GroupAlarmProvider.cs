using System.Collections.Generic;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Job.SmsJob.Providers
{
    [Export("GroupAlarm", typeof(ISmsProvider))]
    class GroupAlarmProvider : ISmsProvider
    {
        #region ISmsProvider Members

        void ISmsProvider.Send(string userName, string password, IEnumerable<string> phoneNumbers, string messageText)
        {
            // TODO: See Sms77Provider.cs (can be copy/pasted mostly)
        }

        #endregion
    }
}
