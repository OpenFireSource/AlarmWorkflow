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
using AlarmWorkflow.BackendService.EngineContracts;
using AlarmWorkflow.BackendService.ManagementContracts;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Job.Divera
{ //
    /// <summary>
    /// Implements a Job that send notifications to the Divera Web-API.
    /// </summary>
    [Export(nameof(Divera), typeof(IJob))]
    [Information(DisplayName = "ExportJobDisplayName", Description = "ExportJobDescription")]
    class Divera : IJob
    {
        #region Constants

        private const string ApiUrl = "https://www.divera247.com/api/alarm?accesskey={0}";

        #endregion

        #region Fields

        private ISettingsServiceInternal _settings;
        private IEmkServiceInternal _emk;

        #endregion

        #region IJob Members

        bool IJob.Initialize(IServiceProvider serviceProvider)
        {
            _settings = serviceProvider.GetService<ISettingsServiceInternal>();
            _emk = serviceProvider.GetService<IEmkServiceInternal>();

            return true;
        }

        void IJob.Execute(IJobContext context, Operation operation)
        {
            if (context.Phase != JobPhase.AfterOperationStored)
            {
                return;
            }

            string key = operation.ToString(_settings.GetSetting("Divera", "key").GetValue<string>());
            TimeSpan delay = DateTime.Now - operation.Timestamp;

            Content content = new Content()
            {
                type = operation.ToString(_settings.GetSetting("Divera", "type").GetValue<string>()),
                text = operation.ToString(_settings.GetSetting("Divera", "text").GetValue<string>()),
                address = operation.Einsatzort.ToString(),
                lat = (float?) operation.Einsatzort.GeoLatitude,
                lng = (float?) operation.Einsatzort.GeoLongitude,
                ric = string.Join(",", operation.Loops),
                vehicle = string.Join(",", GetEMKList(operation)),
                delay = delay.Seconds
            };
            
            HttpStatusCode result = 0;
            if (!SendNotification(content, key, ref result))
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.ErrorSendingNotification, result);
            }
            
        }

        private IList<string> GetEMKList(Operation operation)
        {
            return _emk.GetAllResources()
                .Where(r => r.IsActive)
                .Select(r => r.DisplayName)
                .ToList();
        }

        bool IJob.IsAsync => true;

        #endregion

        #region Methods

        private bool SendNotification(Content content, string key, ref HttpStatusCode result)
        {
            string message = Json.Serialize(content, true);
            byte[] byteArray = Encoding.UTF8.GetBytes(message);

            string url = string.Format(ApiUrl, key);

            Logger.Instance.LogFormat(LogType.Debug, this, Properties.Resources.DebugSendMessage, message);
            
            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Post;
            request.KeepAlive = false;
            request.ContentType = "application/json";
            request.ContentLength = byteArray.Length;

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    HttpStatusCode responseCode = ((HttpWebResponse)response).StatusCode;
                    
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    var responseString = reader.ReadToEnd();

                    Logger.Instance.LogFormat(LogType.Debug, this, Properties.Resources.DebugGetResponse, responseCode, responseString);

                    if (responseCode != HttpStatusCode.OK)
                    {
                        result = responseCode;
                        return false;
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.LogException(this, exception);
            }

            return true;
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
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