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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlarmWorkflow.Shared.Addressing;
using AlarmWorkflow.Shared.Addressing.EntryObjects;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Engine;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Job.SmsJob
{
    /// <summary>
    /// Implements a Job, that sends SMS with different sms services.
    /// </summary>
    [Export("SmsJob", typeof(IJob))]
    [Information(DisplayName = "ExportJobDisplayName", Description = "ExportJobDescription")]
    sealed class SmsJob : IJob
    {
        #region Fields

        private List<MobilePhoneEntryObject> _recipients;
        private string _userName;
        private string _password;
        private ISmsProvider _provider;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SmsJob"/> class.
        /// </summary>
        public SmsJob()
        {
            _recipients = new List<MobilePhoneEntryObject>();
        }

        #endregion

        #region IJob Members

        void IJob.Execute(IJobContext context, Operation operation)
        {
            if (context.Phase != JobPhase.AfterOperationStored)
            {
                return;
            }
            
            string format = SettingsManager.Instance.GetSetting("SMSJob", "MessageFormat").GetString();
            string text = operation.ToString(format);
            text = text.Replace("Ö", "Oe").Replace("Ä", "Ae").Replace("Ü", "Ue").Replace("ö", "oe").Replace("ä", "ae").Replace("ü", "ue").Replace("ß", "ss");
            // Truncate the string if it is too long
            text = text.Truncate(160, true, true);

            // Invoke the provider-send asynchronous because it is a web request and may take a while
            _provider.Send(_userName, _password, _recipients.Select(r => r.PhoneNumber), text);
        }

        bool IJob.Initialize()
        {
            _userName = SettingsManager.Instance.GetSetting("SMSJob", "UserName").GetString();
            _password = SettingsManager.Instance.GetSetting("SMSJob", "Password").GetString();
            _provider = ExportedTypeLibrary.Import<ISmsProvider>(SettingsManager.Instance.GetSetting("SMSJob", "Provider").GetString());

            var recipients = AddressBookManager.GetInstance().GetCustomObjects<MobilePhoneEntryObject>("MobilePhone");
            _recipients.AddRange(recipients.Select(ri => ri.Item2));

            if (_recipients.Count == 0)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.NoRecipientsErrorMessage);
                return false;
            }

            return true;
        }

        bool IJob.IsAsync
        {
            get { return true; }
        }

        #endregion

        #region IDisposable Members

        void System.IDisposable.Dispose()
        {

        }

        #endregion

    }
}
