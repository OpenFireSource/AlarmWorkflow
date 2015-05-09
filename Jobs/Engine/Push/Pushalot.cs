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
    static class Pushalot
    {
        #region Constants

        private const string RequestUrl = "https://pushalot.com/api/sendmessage";
        private const string RequestContentType = "application/x-www-form-urlencoded";
        private const string IsImportant = "True";
        private const string IsSilent = "False";
        private const string TimeToLive = "43200";
        private const int BodyMaxLength = 32768;
        private const int TitleMaxLength = 250;
        private const int SourceMaxLength = 25;

        #endregion

        #region Methods

        internal static void SendNotifications(IEnumerable<string> apiKeys, string application, string header, string message)
        {
            var data = new NameValueCollection();
            data["AuthorizationToken"] = "";
            data["Body"] = message;
            data["IsImportant"] = IsImportant;
            data["IsSilent"] = IsSilent;
            data["Source"] = application;
            data["TimeToLive"] = TimeToLive;
            data["Title"] = header;
            if (!Validate(data))
            {
                return;
            }

            foreach (var apiKey in apiKeys)
            {
                using (var client = new WebClient())
                {
                    data.Set("AuthorizationToken", apiKey);

                    client.Headers[HttpRequestHeader.ContentType] = RequestContentType;
                    client.UploadValuesAsync(new Uri(RequestUrl), data);
                }
            }
        }

        private static bool Validate(NameValueCollection postParameters)
        {
            if (!postParameters.AllKeys.Contains("AuthorizationToken"))
            {
                return false;
            }
            if (!postParameters.AllKeys.Contains("Body") || postParameters.Get("Body").Length > BodyMaxLength)
            {
                return false;
            }
            if (!postParameters.AllKeys.Contains("Title") || postParameters.Get("Title").Length > TitleMaxLength)
            {
                return false;
            }
            if (!postParameters.AllKeys.Contains("Source") || postParameters.Get("Source").Length > SourceMaxLength)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
