using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using AlarmWorkflow.Shared;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Job.SmsJob
{
    /// <summary>
    /// Implements a Job, that sends SMS with the sms77.de service.
    /// </summary>
    [Export("SmsJob", typeof(IJob))]
    sealed class SmsJob : IJob
    {
        #region Fields

        private List<MobilePhoneEntryObject> _recipients;
        private string _userName;
        private string _password;

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

        void IJob.DoJob(Operation operation)
        {
            // TODO: This string contains CustomData. When actually using this job this should be revised to NOT use any custom data (or make it extensible)!
            string text = "Einsatz:%20" + PrepareString(operation.City.Substring(0, operation.City.IndexOf(" ", StringComparison.Ordinal)))
                + "%20Strasse:%20" + PrepareString(operation.Street)
                + "%20" + PrepareString((string)operation.CustomData["Picture"])
                + "%20" + PrepareString(operation.Comment);
                


            foreach (MobilePhoneEntryObject recipient in _recipients)
            {
                StringBuilder uriBuilder = new StringBuilder();
                uriBuilder.Append("http://gateway.sms77.de/?u=");
                uriBuilder.Append(_userName);
                uriBuilder.Append("&p=");
                uriBuilder.Append(_password);
                uriBuilder.Append("&to=");
                uriBuilder.Append(recipient.PhoneNumber);
                uriBuilder.Append("&text=");
                uriBuilder.Append(HttpUtility.HtmlEncode(text));
                uriBuilder.Append("&type=");
                uriBuilder.Append("basicplus");

                try
                {
                    HttpWebRequest msg = (HttpWebRequest)System.Net.WebRequest.Create(new Uri(uriBuilder.ToString()));
                    HttpWebResponse resp = (HttpWebResponse)msg.GetResponse();
                    Stream resp_steam = resp.GetResponseStream();
                    using (StreamReader streamreader = new StreamReader(resp_steam, Encoding.UTF8))
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
                    Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.SendToRecipientGenericErrorMessage, recipient.PhoneNumber);
                    Logger.Instance.LogException(this, ex);
                }
            }
        }

        bool IJob.Initialize()
        {
            _userName = SettingsManager.Instance.GetSetting("SMSJob", "UserName").GetString();
            _password = SettingsManager.Instance.GetSetting("SMSJob", "Password").GetString();

            var recipients = AlarmWorkflowConfiguration.Instance.AddressBook.GetCustomObjects<MobilePhoneEntryObject>("MobilePhone");
            _recipients.AddRange(recipients.Select(ri => ri.Item2));

            if (_recipients.Count == 0)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.NoRecipientsErrorMessage);
                return false;
            }

            return true;
        }

        /// <summary>
        /// This methode url encodes a given string.
        /// </summary>
        /// <param name="str">The string that must be URL encoded.</param>
        /// <returns>The URL encoded string.</returns>
        private static string PrepareString(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        #endregion

    }
}
