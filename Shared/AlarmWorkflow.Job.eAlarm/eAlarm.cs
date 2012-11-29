using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using AlarmWorkflow.Job.MailingJob;
using AlarmWorkflow.Job.eAlarm.Properties;
using AlarmWorkflow.Shared;
using AlarmWorkflow.Shared.Addressing;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Job.eAlarm
{ //
    /// <summary>
    ///     Implements a Job that send notifications to the Android App eAlarm.
    ///     Author: Florian Ritterhoff (c) 2012
    /// </summary>
    [Export("eAlarm", typeof(IJob))]
    public class eAlarm : IJob
    {
        #region Fields

        private readonly List<String> _recipients;
        private readonly List<MailingEntryObject> _recipientsEntry;
        private HttpWebRequest webRequest;

        #endregion Fields

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the eAlarm class.
        /// </summary>
        public eAlarm()
        {
            _recipientsEntry = new List<MailingEntryObject>();
            _recipients = new List<string>();
        }

        #endregion Constructor

        #region IJob Members

        bool IJob.Initialize()
        {
            //Create Webrequest
            webRequest = (HttpWebRequest)WebRequest.Create("https://gymolching-portal.de/gcm/send.php");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            // Get recipients
            IEnumerable<Tuple<AddressBookEntry, MailingEntryObject>> recipients =
                AlarmWorkflowConfiguration.Instance.AddressBook.GetCustomObjects<MailingEntryObject>("Mail");

            _recipientsEntry.AddRange(recipients.Select(ri => ri.Item2));

            // Require at least one recipient for initialization to succeed
            if (_recipientsEntry.Count == 0)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Resources.NoRecipientsMessage);
                return false;
            }

            return true;
        }

        void IJob.DoJob(Operation operation)
        {
            //Gets the mail-addresses with googlemail.com or gmail.com
            foreach (MailingEntryObject recipient in _recipientsEntry)
            {
                if (recipient.Address.Address.EndsWith("@gmail.com") ||
                    recipient.Address.Address.EndsWith("@googlemail.com"))
                {
                    _recipients.Add(recipient.Address.Address);
                }
            }
            string to = String.Join(",", _recipients.ToArray());

            //TODO Fetching Longitude and Latitude!
            Dictionary<String, String> geoCode =
                Helpers.GetGeocodes(operation.Location + " " + operation.Street + " " + operation.StreetNumber);
            String longitude = "0";
            String latitude = "0";
            if (geoCode != null)
            {
                longitude = geoCode[Resources.LONGITUDE];
                latitude = geoCode[Resources.LATITUDE];
            }
            String body = operation.ToString(SettingsManager.Instance.GetSetting("eAlarm", "text").GetString());
            String header = operation.ToString(SettingsManager.Instance.GetSetting("eAlarm", "header").GetString());
            var postParameters = new Dictionary<string, string>
                                     {
                                         {"email", to},
                                         {"header", header},
                                         {"text", body},
                                         {"long", longitude},
                                         {"lat", latitude}
                                     };
            string postData = postParameters.Keys.Aggregate("",
                                                            (current, key) =>
                                                            current +
                                                            (HttpUtility.UrlEncode(key) + "=" +
                                                             HttpUtility.UrlEncode(postParameters[key]) + "&"));

            byte[] data = Encoding.UTF8.GetBytes(postData);
            webRequest.ContentLength = data.Length;

            Stream requestStream = webRequest.GetRequestStream();
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            Stream responseStream = webResponse.GetResponseStream();

            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            if (responseStream != null)
            {
                var reader = new StreamReader(responseStream, Encoding.Default);
                string pageContent = reader.ReadToEnd();
                reader.Close();
                responseStream.Close();
                webResponse.Close();

                //TODO Analyzing Response
            }
        }

        #endregion IJob Members

    }
}