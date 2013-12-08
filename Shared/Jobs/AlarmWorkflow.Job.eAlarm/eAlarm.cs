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
using System.Web.Script.Serialization;
using AlarmWorkflow.BackendService.AddressingContracts;
using AlarmWorkflow.BackendService.AddressingContracts.EntryObjects;
using AlarmWorkflow.BackendService.EngineContracts;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Job.eAlarm
{ //
    /// <summary>
    /// Implements a Job that send notifications to the Android App eAlarm.     
    /// </summary>
    [Export("eAlarm", typeof(IJob))]
    [Information(DisplayName = "ExportJobDisplayName", Description = "ExportJobDescription")]
    public class eAlarm : IJob
    {
        #region Fields

        private ISettingsServiceInternal _settings;
        private IAddressingServiceInternal _addressing;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the eAlarm class.
        /// </summary>
        public eAlarm()
        {
        }

        #endregion Constructor

        #region IJob Members

        bool IJob.Initialize(IServiceProvider serviceProvider)
        {
            _settings = serviceProvider.GetService<ISettingsServiceInternal>();
            _addressing = serviceProvider.GetService<IAddressingServiceInternal>();
            return true;
        }

        void IJob.Execute(IJobContext context, Operation operation)
        {
            if (context.Phase != JobPhase.AfterOperationStored)
            {
                return;
            }

            string body = operation.ToString(_settings.GetSetting("eAlarm", "text").GetValue<string>());
            string header = operation.ToString(_settings.GetSetting("eAlarm", "header").GetValue<string>());
            string location = operation.Einsatzort.ToString();
            bool encryption = _settings.GetSetting("eAlarm", "Encryption").GetValue<bool>();
            string encryptionKey = _settings.GetSetting("eAlarm", "EncryptionKey").GetValue<string>();
            if (encryption)
            {
                body = Helper.Encrypt(body, encryptionKey);
                header = Helper.Encrypt(header, encryptionKey);
                location = Helper.Encrypt(location, encryptionKey);
            }

            string[] to = GetRecipients(operation).Where(pushEntryObject => pushEntryObject.Consumer == "eAlarm").Select(pushEntryObject => pushEntryObject.RecipientApiKey).ToArray();

            Content content = new Content()
            {
                registration_ids = to,
                data = new Content.Data()
                    {
                        awf_title = header,
                        awf_message = body,
                        awf_location = location
                    }
            };
            string message = new JavaScriptSerializer().Serialize(content);

            HttpStatusCode result = 0;
            if (SendGCMNotification("AIzaSyA5hhPTlYxJsEDniEoW8OgfxWyiUBEPiS0", message, ref result))
            {
                Logger.Instance.LogFormat(LogType.Info, this, "Succesfully sent eAlarm notification");
            }
            else
            {
                Logger.Instance.LogFormat(LogType.Error, this, "Error while sending eAlarm notification Errorcode: '{0}'", (int)result);
            }
        }

        private IList<PushEntryObject> GetRecipients(Operation operation)
        {
            var recipients = _addressing.GetCustomObjectsFiltered<PushEntryObject>(PushEntryObject.TypeId, operation);
            return recipients.Select(ri => ri.Item2).ToList();
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