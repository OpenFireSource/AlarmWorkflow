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
using System.Net;
using System.Text;
using System.Web;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Job.SmsJob.Providers
{
    [Export("GroupAlarm", typeof(ISmsProvider))]
    [Information(DisplayName = "ExportGroupAlarmDisplayName", Description = "ExportGroupAlarmDescription")]
    class GroupAlarmProvider : ISmsProvider
    {
        #region ISmsProvider Members

        void ISmsProvider.Send(string userName, string password, IEnumerable<string> phoneNumbers, string messageText)
        {
            foreach (string number in phoneNumbers)
            {
                StringBuilder uriBuilder = new StringBuilder();
                uriBuilder.Append("http://www.groupalarm.de/webin.php?log_user=");
                uriBuilder.Append(userName);
                uriBuilder.Append("&log_epass=");
                uriBuilder.Append(password);
                uriBuilder.Append("&listcode=");
                uriBuilder.Append(number);
                uriBuilder.Append("&free=");
                uriBuilder.Append(HttpUtility.UrlEncode(messageText));
                uriBuilder.Append("&flash=0");
                uriBuilder.Append("&fb=1");

                try
                {
                    WebRequest msg = WebRequest.Create(new Uri(uriBuilder.ToString()));
                    using (WebResponse resp = msg.GetResponse())
                    {
                        using (StreamReader streamreader = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                        {
                            string response = streamreader.ReadToEnd();
                            if (response != "OK")
                            {
                                Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.SendToRecipientSMSProviderErrorMessage, response);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.SendToRecipientGenericErrorMessage, number);
                    Logger.Instance.LogException(this, ex);
                }
            }
        }

        #endregion
    }
}