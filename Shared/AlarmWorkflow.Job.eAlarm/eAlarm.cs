using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using AlarmWorkflow.Job.eAlarm.Properties;
using AlarmWorkflow.Shared;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Job.eAlarm
{//
    /// <summary>
    /// Implements a Job that send notifications to the Android App eAlarm.
    /// Author: Florian Ritterhoff (c) 2012
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

        public bool Initialize()
        {
            //Create Webrequest
            webRequest = (HttpWebRequest) WebRequest.Create("https://gymolching-portal.de/gcm/send.php");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            // Get recipients
            var recipients =
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

        public void DoJob(Operation operation)
        {
            //Init StringBuilder for Texts
            var bodyBuilder = new StringBuilder();
            var headerBuilder = new StringBuilder();

            #region bodyBuilder filling
            if (!String.IsNullOrEmpty(operation.Timestamp.ToString(CultureInfo.InvariantCulture)))
                bodyBuilder.AppendLine("Zeitstempel: " + operation.Timestamp.ToString(CultureInfo.InvariantCulture));
            if (!String.IsNullOrEmpty(operation.Keyword))
                bodyBuilder.AppendLine("Stichwort: " + operation.Keyword);
            if (!String.IsNullOrEmpty(operation.EmergencyKeyword))
                bodyBuilder.AppendLine("Einsatzstichwort: " + operation.EmergencyKeyword);
            if (!String.IsNullOrEmpty(operation.Picture)) 
                bodyBuilder.AppendLine("Meldebild: " + operation.Picture);
            if (!String.IsNullOrEmpty(operation.OperationNumber))
                bodyBuilder.AppendLine("Einsatznr: " + operation.OperationNumber);
            if (!String.IsNullOrEmpty(operation.Comment))
                bodyBuilder.AppendLine("Hinweis: " + operation.Comment);
            if (!String.IsNullOrEmpty(operation.Messenger))
                bodyBuilder.AppendLine("Mitteiler: " + operation.Messenger);
            if (!String.IsNullOrEmpty(operation.Location))
                bodyBuilder.AppendLine("Einsatzort: " + operation.Location);
            if (!String.IsNullOrEmpty(operation.Street) && !String.IsNullOrEmpty(operation.StreetNumber))
                bodyBuilder.AppendLine("Straße: " + operation.Street + " " + operation.StreetNumber);
            if (!String.IsNullOrEmpty(operation.GetCustomData<string>("Intersection")))
                bodyBuilder.AppendLine("Kreuzung: " + operation.GetCustomData<string>("Intersection"));
            if (!String.IsNullOrEmpty(operation.ZipCode) && !String.IsNullOrEmpty(operation.City))
                bodyBuilder.AppendLine("Ort: " + operation.ZipCode + " " + operation.City);
            if (!String.IsNullOrEmpty(operation.Property))
                bodyBuilder.AppendLine("Objekt: " + operation.Property);
            if (!String.IsNullOrEmpty(operation.OperationPlan))
                bodyBuilder.AppendLine("Einsatzplan: " + operation.OperationPlan);
            if (!String.IsNullOrEmpty(operation.GetCustomData<string>("Vehicles")))
                bodyBuilder.AppendLine("Fahrzeuge: " + operation.GetCustomData<string>("Vehicles"));
            #endregion

            #region headerBuilder filling

            if (!String.IsNullOrEmpty(operation.Keyword))
                headerBuilder.AppendLine(operation.Keyword);
            else
            {
                if (!String.IsNullOrEmpty(operation.EmergencyKeyword))
                    headerBuilder.AppendLine(operation.EmergencyKeyword);
                else
                {
                    if (!String.IsNullOrEmpty(operation.Picture))
                        headerBuilder.AppendLine(operation.Picture);
                }
            }
            if (headerBuilder.Length == 0)
            {
                headerBuilder.Append("Feuerwehreinsatz");
            }

            #endregion

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
            String longitude = "0";
            String latutude = "0";
            #region postParameters filling and converting for request
            var postParameters = new Dictionary<string, string>
                                     {
                                         {"email", to},
                                         {"header", headerBuilder.ToString()},
                                         {"text", bodyBuilder.ToString()},
                                         {"long", "0"},
                                         {"lat", "0"}
                                     };
            string postData = postParameters.Keys.Aggregate("",
                                                            (current, key) =>
                                                            current +
                                                            (HttpUtility.UrlEncode(key) + "=" +
                                                             HttpUtility.UrlEncode(postParameters[key]) + "&"));

            byte[] data = Encoding.UTF8.GetBytes(postData);
            #endregion
            webRequest.ContentLength = data.Length;

            Stream requestStream = webRequest.GetRequestStream();
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
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