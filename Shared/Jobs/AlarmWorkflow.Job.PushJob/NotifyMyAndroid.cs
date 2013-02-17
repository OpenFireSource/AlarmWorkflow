using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace AlarmWorkflow.Job.PushJob
{
    /// <summary>
    ///     Description of MyClass.
    /// </summary>
    public class NMA
    {
        #region NMANotificationPriority enum

        public enum NMANotificationPriority : sbyte
        {
            VeryLow = -2,
            Moderate = -1,
            Normal = 0,
            High = 1,
            Emergency = 2
        }

        #endregion

        private const string RequestUrl = "https://www.notifymyandroid.com/publicapi/notify";
        private const string RequestContentType = "application/x-www-form-urlencoded";
        private const string RequestMethodType = "POST";
        private const string Apikey = "apikey";
        private const string Application = "application";
        private const string Event = "event";
        private const string Description = "description";
        private const string Developerkey = "developerkey";
        private const string Priortiy = "priority";
        private const string Url = "url";
        private const int ApplicationMaxLength = 256;
        private const int DescriptionMaxLength = 10000;
        private const int EventMaxLength = 1000;
        private const int DeveloperkeyMaxLength = 48;
        private const int UrlMaxLenght = 2000;

      
       
        public void Notify(List<string> apiKeys, string application, string header, string description,
                           NMANotificationPriority priority)
        {
            String[] keyArray = apiKeys.ToArray();
            string apikey = String.Join(",", keyArray);
            var data = new Dictionary<string, string>
                {
                    {Apikey, apikey},
                    {Application, application},
                    {Event, header},
                    {Description, description},
                    {Priortiy, ((sbyte) priority).ToString()}
                };

            send(data);
        }


        private void send(Dictionary<string, string> postParameters)
        {
            if (!valiade(postParameters))
            {
                return;
            }
            string postData = postParameters.Keys.Aggregate("",
                                                            (current, key) =>
                                                            current +
                                                            (HttpUtility.UrlEncode(key) + "=" +
                                                             HttpUtility.UrlEncode(postParameters[key]) + "&"));
            postData = postData.Substring(0, postData.Length - 1);
            byte[] data = Encoding.UTF8.GetBytes(postData);
            var webRequest = (HttpWebRequest) WebRequest.Create(RequestUrl);
            webRequest.Method = RequestMethodType;
            webRequest.ContentType = RequestContentType;
            webRequest.ContentLength = data.Length;
            Stream requestStream = webRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            var webResponse = (HttpWebResponse) webRequest.GetResponse();
            Stream responseStream = webResponse.GetResponseStream();
            string pageContent = null;
            if (responseStream != null)
            {
                var streamReader = new StreamReader(responseStream, Encoding.Default);
                pageContent = streamReader.ReadToEnd();
                streamReader.Close();
            }
            if (responseStream != null) responseStream.Close();
            webResponse.Close();
            analyseOutput(pageContent);
        }

        private void analyseOutput(string pageContent)
        {
            string content = pageContent;
        }

        private static bool valiade(IDictionary<string, string> postParameters)
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
            if (postParameters.ContainsKey(Developerkey))
            {
                if (postParameters[Developerkey].Length >= DeveloperkeyMaxLength)
                {
                    return false;
                }
            }
            if (postParameters.ContainsKey(Url))
            {
                String url = postParameters[Url];
                if (url.Length > UrlMaxLenght)
                {
                    return false;
                }
            }
            return true;
        }
    }
}