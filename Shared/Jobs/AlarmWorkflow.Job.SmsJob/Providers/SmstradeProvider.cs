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
using System.Net;
using AlarmWorkflow.Job.SmsJob.Properties;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Job.SmsJob.Providers
{
    [Export("Smstrade", typeof(ISmsProvider))]
    [Information(DisplayName = "ExportSmstradeDisplayName", Description = "ExportSmstradeDescription")]
    class SmstradeProvider : ISmsProvider
    {
        #region ISmsProvider Members

        void ISmsProvider.Send(string userName, string password, IEnumerable<string> phoneNumbers, string messageText)
        {
            using (WebClient client = new WebClient())
            {
                // This is required for preventing some errors. (Taken from example .net - client from smstrade)

                Uri uri = new Uri("https://gateway.smstrade.de/bulk/");
                ServicePoint servicePoint = ServicePointManager.FindServicePoint(uri);
                if (servicePoint.Expect100Continue)
                {
                    servicePoint.Expect100Continue = false;
                }

                string url = string.Format("https://gateway.smstrade.de/bulk/?key={0}&message={1}&to={2}&route=basic", password, messageText, string.Join(";", phoneNumbers));
                string status = client.DownloadString(url);

                Logger.Instance.LogFormat(LogType.Info, this, Resources.SendToRecipientSuccessMessage, status);
            }
        }

        #endregion
    }
}
