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
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using AlarmWorkflow.Job.eAlarm.Properties;
using AlarmWorkflow.Shared.Addressing;
using AlarmWorkflow.Shared.Addressing.EntryObjects;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Engine;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Job.eAlarm
{ //
    /// <summary>
    ///     Implements a Job that send notifications to the Android App eAlarm.     
    /// </summary>
    [Export("eAlarm", typeof(IJob))]
    [Information(DisplayName = "ExportJobDisplayName", Description = "ExportJobDescription")]
    public class eAlarm : IJob
    {
        #region Fields

        private readonly List<PushEntryObject> _recipients;

        #endregion Fields

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the eAlarm class.
        /// </summary>
        public eAlarm()
        {
            _recipients = new List<PushEntryObject>();
        }

        #endregion Constructor

        #region IJob Members

        bool IJob.Initialize()
        {
            // Get recipients
            IEnumerable<Tuple<AddressBookEntry, PushEntryObject>> recipients = AddressBookManager.GetInstance().GetCustomObjects<PushEntryObject>("Push");
            _recipients.AddRange(recipients.Select(ri => ri.Item2));

            // Require at least one recipient for initialization to succeed
            if (_recipients.Count == 0)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Resources.NoRecipientsMessage);
                return false;
            }

            return true;
        }

        void IJob.Execute(IJobContext context, Operation operation)
        {
            if (context.Phase != JobPhase.AfterOperationStored)
            {
                return;
            }

            Dictionary<String, String> geoCode =
                Helpers.GetGeocodes(operation.Einsatzort.Street + " " + operation.Einsatzort.StreetNumber + " " + operation.Einsatzort.ZipCode + " " + operation.Einsatzort.City);
            String longitude = "0";
            String latitude = "0";
            if (geoCode != null)
            {
                longitude = geoCode[Resources.LONGITUDE];
                latitude = geoCode[Resources.LATITUDE];
            }
          
            String body = operation.ToString(SettingsManager.Instance.GetSetting("eAlarm", "text").GetString());
            String header = operation.ToString(SettingsManager.Instance.GetSetting("eAlarm", "header").GetString());
            Dictionary<string, string> postParameters = new Dictionary<string, string>
                {
                    {"header", header},
                    {"text", body},
                    {"lat", latitude},
                    {"long", longitude}
                };
            String to = _recipients.Where(pushEntryObject => pushEntryObject.Consumer == "eAlarm").Aggregate("[", (current, pushEntryObject) => current + ("\"" + pushEntryObject.RecipientApiKey + "\","));
            to = to.Substring(0, to.Length - 1) + "]";
            string postData = postParameters.Keys.Aggregate("", (current, key) => current + ("\"" + HttpUtility.UrlEncode(key) + "\":\"" + HttpUtility.UrlEncode(postParameters[key], Encoding.UTF8) + "\","));
            postData = "{" + postData.Substring(0, postData.Length - 1) + "}";
            string message = "{\"registration_ids\":" + to + ",\"data\":" + postData + "}";
            HttpStatusCode result = (HttpStatusCode)0;
            if (SendGCMNotification("AIzaSyA5hhPTlYxJsEDniEoW8OgfxWyiUBEPiS0", message, ref result))
            {
                Logger.Instance.LogFormat(LogType.Info, this, "Succesfully sent eAlarm notification");
            }
            else
            {
                Logger.Instance.LogFormat(LogType.Error, this, "Error while sending eAlarm notification Errorcode: '{0}'", (int)result);
            }
        }

        bool IJob.IsAsync
        {
            get { return true; }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Send a Google Cloud Message. Uses the GCM service and your provided api key.
        /// </summary>
        /// <param name="apiKey">Server API Key</param>
        /// <param name="postData">JSON-formated Data</param>
        /// <param name="result">Errorresult</param>
        /// <param name="postDataContentType">Possible change of dataformat </param>
        /// <returns>The response string from the google servers</returns>
        private bool SendGCMNotification(string apiKey, string postData, ref HttpStatusCode result, string postDataContentType = "application/json")
        {
            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://android.googleapis.com/gcm/send");
            request.Method = "POST";
            request.KeepAlive = false;
            request.ContentType = postDataContentType;
            request.Headers.Add(string.Format("Authorization: key={0}", apiKey));
            request.ContentLength = byteArray.Length;

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            try
            {
                WebResponse response = request.GetResponse();
                HttpStatusCode responseCode = ((HttpWebResponse)response).StatusCode;
                if (!responseCode.Equals(HttpStatusCode.OK))
                {
                    result = responseCode;
                    return false;
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.LogFormat(LogType.Error, this, "No Respone by Googleserver.");
                Logger.Instance.LogException(this, exception);
            }
            return true;
        }

        /// <summary>
        /// Returns allways true for a server certrificate validation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}