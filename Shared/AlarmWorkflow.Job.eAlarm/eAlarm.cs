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
            String body = GenerateText(SettingsManager.Instance.GetSetting("eAlarm", "text").GetString(), operation);
            String header = GenerateText(SettingsManager.Instance.GetSetting("eAlarm", "header").GetString(), operation);
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

        #region Methods

        private string GenerateText(String input, Operation operation)
        {
            String text = input;
            text = text.Replace("{Zeitstempel}", !String.IsNullOrEmpty(operation.Timestamp.ToString(CultureInfo.InvariantCulture)) ? operation.Timestamp.ToString(CultureInfo.InvariantCulture) : "n/a");
            text = text.Replace("{Stichwort}", !String.IsNullOrEmpty(operation.Keyword) ? operation.Keyword : "n/a");
            text = text.Replace("{Einsatzstichwort}", !String.IsNullOrEmpty(operation.EmergencyKeyword) ? operation.EmergencyKeyword : "n/a");
            text = text.Replace("{Meldebild}", !String.IsNullOrEmpty(operation.Picture) ? operation.Picture : "n/a");
            text = text.Replace("{Einsatznr}", !String.IsNullOrEmpty(operation.OperationNumber) ? operation.OperationNumber : "n/a");
            text = text.Replace("{Hinweis}", !String.IsNullOrEmpty(operation.Comment) ? operation.Comment : "n/a");
            text = text.Replace("{Mitteiler}", !String.IsNullOrEmpty(operation.Messenger) ? operation.Messenger : "n/a");
            text = text.Replace("{Einsatzort}", !String.IsNullOrEmpty(operation.Location) ? operation.Location : "n/a");
            text = !String.IsNullOrEmpty(operation.Street) && !String.IsNullOrEmpty(operation.StreetNumber)
                       ? text.Replace("{Straße}", operation.Street + " " + operation.StreetNumber)
                       : text.Replace("{Straße}", operation.Keyword);
            text = text.Replace("{Kreuzung}", !String.IsNullOrEmpty(operation.GetCustomData<string>("Intersection")) ? operation.GetCustomData<string>("Intersection") : operation.Keyword);
            text = !String.IsNullOrEmpty(operation.ZipCode) && !String.IsNullOrEmpty(operation.City)
                       ? text.Replace("{Ort}", operation.ZipCode + " " + operation.City)
                       : text.Replace("{Ort}", "n/a");
            text = text.Replace("{Objekt}", !String.IsNullOrEmpty(operation.Property) ? operation.Property : "n/a");
            text = text.Replace("{Einsatzplan}", !String.IsNullOrEmpty(operation.OperationPlan) ? operation.OperationPlan : "n/a");
            text = text.Replace("{Fahrzeuge}", !String.IsNullOrEmpty(operation.GetCustomData<string>("Vehicles")) ? operation.GetCustomData<string>("Vehicles") : "n/a");
            return text;
        }

        #endregion Methods
    }
}