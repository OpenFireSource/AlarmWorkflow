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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Job.PushJob
{
    static class Prowl
    {
        #region Constants

        private const string RequestUrl = "https://api.prowlapp.com/publicapi/add";
        private const string RequestContentType = "application/x-www-form-urlencoded";
        private const string Apikey = "apikey";
        private const string Application = "application";
        private const string Event = "event";
        private const string Description = "description";
        private const string Priority = "priority";
        private const int ApplicationMaxLength = 256;
        private const int DescriptionMaxLength = 10000;
        private const int EventMaxLength = 1024;
        private const int EmergencyPriority = 2;

        #endregion

        #region Methods

        internal static void SendNotifications(IEnumerable<string> apiKeys, string application, string header, string message)
        {
            string apikey = string.Join(",", apiKeys);

            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                {Apikey, apikey},
                {Application, application},
                {Event, header},
                {Description, message},
                {Priority, EmergencyPriority.ToString()}
            };

            Send(data);
        }

        private static void Send(IDictionary<string, string> postParameters)
        {
            if (!Validate(postParameters))
            {
                return;
            }

            string postData = postParameters.Keys.Aggregate("", (current, key) => current + (HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode(postParameters[key]) + "&"));
            postData = postData.Substring(0, postData.Length - 1);

            byte[] data = Encoding.UTF8.GetBytes(postData);

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(RequestUrl);
            webRequest.Method = WebRequestMethods.Http.Post;
            webRequest.ContentType = RequestContentType;
            webRequest.ContentLength = data.Length;

            using (Stream requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
            }

            using (WebResponse webResponse = webRequest.GetResponse())
            {
                using (Stream responseStream = webResponse.GetResponseStream())
                {
                    using (StreamReader streamReader = new StreamReader(responseStream, Encoding.Default))
                    {
                        string respone = streamReader.ReadToEnd();

                        Logger.Instance.LogFormat(LogType.Info, typeof(Prowl), Properties.Resources.Prowl, respone);
                    }
                }
            }
        }

        private static bool Validate(IDictionary<string, string> postParameters)
        {
            if (!postParameters.ContainsKey(Apikey))
            {
                return false;
            }
            if (!postParameters.ContainsKey(Application) || postParameters[Application].Length >= ApplicationMaxLength)
            {
                return false;
            }
            if (!postParameters.ContainsKey(Event) || postParameters[Event].Length >= EventMaxLength)
            {
                return false;
            }
            if (!postParameters.ContainsKey(Description) || postParameters[Description].Length >= DescriptionMaxLength)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}