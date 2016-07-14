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
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Web;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Job.PushJob
{
    class Pushover
    {
        #region Constants

        private const string RequestUrl = "https://api.pushover.net/1/messages.json?token=";
        private const string RequestContentType = "application/x-www-form-urlencoded";
        private const string Priority = "1";
        
        private const int BodyMaxLength = 32768;
        private const int TitleMaxLength = 250;

        #endregion

        #region Methods

        internal static void SendNotifications(IEnumerable<string> apiKeys, string application, string header, string message)
        {
            var data = new NameValueCollection();
            data["user"] = "";
            data["priority"] = Priority;
            data["message"] = message;
            data["title"] = header;

            if (!Validate(data) || String.IsNullOrEmpty(application))
            {
                return;
            }

            foreach (var apiKey in apiKeys)
            {
                using (var client = new WebClient())
                {
                    data.Set("user", apiKey);

                    client.Headers[HttpRequestHeader.ContentType] = RequestContentType;
                    client.UploadValuesAsync(new Uri(RequestUrl + application), data);
                }
            }
        }

        private static bool Validate(NameValueCollection postParameters)
        {
            if (!postParameters.AllKeys.Contains("user"))
            {
                return false;
            }
            if (!postParameters.AllKeys.Contains("message") || postParameters.Get("message").Length > BodyMaxLength)
            {
                return false;
            }
            if (!postParameters.AllKeys.Contains("title") || postParameters.Get("title").Length > TitleMaxLength)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}