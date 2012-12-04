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
    [Export("sms77", typeof(ISmsProvider))]
    class Sms77Provider : ISmsProvider
    {
        #region ISmsProvider Members

        void ISmsProvider.Send(string userName, string password, IEnumerable<string> phoneNumbers, string messageText)
        {
            foreach (string number in phoneNumbers)
            {
                StringBuilder uriBuilder = new StringBuilder();
                uriBuilder.Append("http://gateway.sms77.de/?u=");
                uriBuilder.Append(userName);
                uriBuilder.Append("&p=");
                uriBuilder.Append(password);
                uriBuilder.Append("&to=");
                uriBuilder.Append(number);
                uriBuilder.Append("&text=");
                uriBuilder.Append(HttpUtility.UrlEncode(messageText));
                uriBuilder.Append("&type=");
                uriBuilder.Append("basicplus");

                try
                {
                    WebRequest msg = WebRequest.Create(new Uri(uriBuilder.ToString()));
                    WebResponse resp = msg.GetResponse();
                    using (StreamReader streamreader = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                    {
                        string response = streamreader.ReadToEnd();
                        if (response != "100")
                        {
                            Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.SendToRecipientSMS77ErrorMessage, response);
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
