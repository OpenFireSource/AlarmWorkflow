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
    /// NMA-Sender
    /// </summary>
    static class NMA
    {
        #region Constants

        private const string RequestUrl = "https://www.notifymyandroid.com/publicapi/notify";
        private const string RequestContentType = "application/x-www-form-urlencoded";
        private const string RequestMethodType = "POST";
        private const string Apikey = "apikey";
        private const string Application = "application";
        private const string Event = "event";
        private const string Description = "message";
        private const string Priortiy = "priority";
        private const int ApplicationMaxLength = 256;
        private const int DescriptionMaxLength = 10000;
        private const int EventMaxLength = 1000;

        #endregion

        #region Methods
  
        /// <summary>
        /// Sends a prowlnotification with given data
        /// </summary>
        /// <param name="apiKeys">List of API-Keys</param>
        /// <param name="application">Title of the Application</param>
        /// <param name="header">Title of the Event</param>
        /// <param name="message">Message</param>
        /// <param name="priority">Priority of the message</param>
        public static void Notify(List<string> apiKeys, string application, string header, string message, NMANotificationPriority priority)
        {
            String[] keyArray = apiKeys.ToArray();
            string apikey = String.Join(",", keyArray);
            Dictionary<string, string> data = new Dictionary<string, string>
                {
                    {Apikey, apikey},
                    {Application, application},
                    {Event, header},
                    {Description, message},
                    {Priortiy, ((sbyte) priority).ToString()}
                };

            Send(data);
        }


        private static void Send(Dictionary<string, string> postParameters)
        {
            if (!Validate(postParameters))
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

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(RequestUrl);
            webRequest.Method = RequestMethodType;
            webRequest.ContentType = RequestContentType;
            webRequest.ContentLength = data.Length;

            using (Stream requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
            }

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            using (Stream responseStream = webResponse.GetResponseStream())
            {
                using (StreamReader streamReader = new StreamReader(responseStream, Encoding.Default))
                {
                    streamReader.ReadToEnd();
                }
            }

            webResponse.Close();
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

    #region Nested Types

    internal enum NMANotificationPriority : sbyte
    {
        VeryLow = -2,
        Moderate = -1,
        Normal = 0,
        High = 1,
        Emergency = 2
    }

    #endregion
}