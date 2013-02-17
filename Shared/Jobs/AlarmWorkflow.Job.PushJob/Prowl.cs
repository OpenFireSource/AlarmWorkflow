using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class Prowl
    {
        #region ProwlNotificaionPriortiy enum

        public enum ProwlNotificaionPriortiy : sbyte
        {
            VeryLow = -2,
            Moderate = -1,
            Normal = 0,
            High = 1,
            Emergency = 2
        }

        #endregion

        private const string RequestUrl = "https://api.prowlapp.com/publicapi/add";
        private const string RequestContentType = "application/x-www-form-urlencoded";
        private const string RequestMethodType = "POST";
        private const string Apikey = "apikey";
        private const string Application = "application";
        private const string Event = "event";
        private const string Description = "description";
        private const string Developerkey = "providerkey ";
        private const string Priortiy = "priority";
        private const string Url = "url";

        private const string ContentTypeHtml = "text/html";
        private const int ApplicationMaxLength = 256;
        private const int DescriptionMaxLength = 10000;
        private const int EventMaxLength = 1024;
        private const int DeveloperkeyMaxLength = 40;
        private const int UrlMaxLenght = 512;

        #region Sending to one Client

        public void Notify(string apikey, string application, string header, string description)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
                {
                    {Apikey, apikey},
                    {Application, application},
                    {Event, header},
                    {Description, description}
                };
            send(data);
        }

        public void Notify(string apikey, string application, string header, string description,
                           ProwlNotificaionPriortiy priority)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
                {
                    {Apikey, apikey},
                    {Application, application},
                    {Event, header},
                    {Description, description},
                    {Priortiy, ((sbyte) priority).ToString(CultureInfo.InvariantCulture)}
                };
            send(data);
        }

        public void Notify(string apikey, string application, string header, string description, string devKey)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
                {
                    {Apikey, apikey},
                    {Application, application},
                    {Event, header},
                    {Description, description},
                    {Developerkey, devKey}
                };
            send(data);
        }

        public void Notify(string apikey, string application, string header, string description, Uri url)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
                {
                    {Apikey, apikey},
                    {Application, application},
                    {Event, header},
                    {Description, description},
                    {Url, url.OriginalString}
                };
            send(data);
        }


        public void Notify(string apikey, string application, string header, string description,
                           ProwlNotificaionPriortiy priority, string devKey)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
                {
                    {Apikey, apikey},
                    {Application, application},
                    {Event, header},
                    {Description, description},
                    {Priortiy, ((sbyte) priority).ToString()},
                    {Developerkey, devKey}
                };
            send(data);
        }

        public void Notify(string apikey, string application, string header, string description,
                           ProwlNotificaionPriortiy priority, Uri url)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
                {
                    {Apikey, apikey},
                    {Application, application},
                    {Event, header},
                    {Description, description},
                    {Url, url.OriginalString},
                    {Priortiy, ((sbyte) priority).ToString()}
                };
            send(data);
        }

        public void Notify(string apikey, string application, string header, string description, string devKey, Uri url)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
                {
                    {Apikey, apikey},
                    {Application, application},
                    {Event, header},
                    {Description, description},
                    {Developerkey, devKey},
                    {Url, url.OriginalString}
                };
            send(data);
        }


        public void Notify(string apikey, string application, string header, string description,
                           ProwlNotificaionPriortiy priority, string devKey, Uri url)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
                {
                    {Apikey, apikey},
                    {Application, application},
                    {Event, header},
                    {Description, description},
                    {Priortiy, ((sbyte) priority).ToString()},
                    {Url, url.OriginalString},
                    {Developerkey, devKey}
                };
            send(data);
        }

        #endregion

        #region Sending to multiple Clients

        public void Notify(List<string> apiKeys, string application, string header, string description)
        {
            String[] keyArray = apiKeys.ToArray();
            string apikey = String.Join(",", keyArray);
            Dictionary<string, string> data = new Dictionary<string, string>
                {
                    {Apikey, apikey},
                    {Application, application},
                    {Event, header},
                    {Description, description}
                };

            send(data);
        }

        public void Notify(List<string> apiKeys, string application, string header, string description,
                           ProwlNotificaionPriortiy priority)
        {
            String[] keyArray = apiKeys.ToArray();
            string apikey = String.Join(",", keyArray);
            Dictionary<string, string> data = new Dictionary<string, string>
                {
                    {Apikey, apikey},
                    {Application, application},
                    {Event, header},
                    {Description, description},
                    {Priortiy, ((sbyte) priority).ToString()}
                };

            send(data);
        }

        public void Notify(List<string> apiKeys, string application, string header, string description, string devKey)
        {
            String[] keyArray = apiKeys.ToArray();
            string apikey = String.Join(",", keyArray);
            Dictionary<string, string> data = new Dictionary<string, string>
                {
                    {Apikey, apikey},
                    {Application, application},
                    {Event, header},
                    {Description, description},
                    {Developerkey, devKey}
                };

            send(data);
        }

        public void Notify(List<string> apiKeys, string application, string header, string description, Uri url)
        {
            String[] keyArray = apiKeys.ToArray();
            string apikey = String.Join(",", keyArray);
            Dictionary<string, string> data = new Dictionary<string, string>
                {
                    {Apikey, apikey},
                    {Application, application},
                    {Event, header},
                    {Description, description},
                    {Url, url.OriginalString}
                };

            send(data);
        }

        public void Notify(List<string> apiKeys, string application, string header, string description,
                           ProwlNotificaionPriortiy priority, string devKey)
        {
            String[] keyArray = apiKeys.ToArray();
            string apikey = String.Join(",", keyArray);
            Dictionary<string, string> data = new Dictionary<string, string>
                {
                    {Apikey, apikey},
                    {Application, application},
                    {Event, header},
                    {Description, description},
                    {Priortiy, ((sbyte) priority).ToString()},
                    {Developerkey, devKey}
                };

            send(data);
        }

        public void Notify(List<string> apiKeys, string application, string header, string description,
                           ProwlNotificaionPriortiy priority, Uri url)
        {
            String[] keyArray = apiKeys.ToArray();
            string apikey = String.Join(",", keyArray);
            Dictionary<string, string> data = new Dictionary<string, string>
                {
                    {Apikey, apikey},
                    {Application, application},
                    {Event, header},
                    {Description, description},
                    {Url, url.OriginalString},
                    {Priortiy, ((sbyte) priority).ToString()}
                };

            send(data);
        }


        public void Notify(List<string> apiKeys, string application, string header, string description, string devKey,
                           Uri url)
        {
            String[] keyArray = apiKeys.ToArray();
            string apikey = String.Join(",", keyArray);
            Dictionary<string, string> data = new Dictionary<string, string>
                {
                    {Apikey, apikey},
                    {Application, application},
                    {Event, header},
                    {Description, description},
                    {Developerkey, devKey},
                    {Url, url.OriginalString}
                };
            send(data);
        }

        public void Notify(List<string> apiKeys, string application, string header, string description,
                           ProwlNotificaionPriortiy priority, string devKey, Uri url)
        {
            String[] keyArray = apiKeys.ToArray();
            string apikey = String.Join(",", keyArray);
            Dictionary<string, string> data = new Dictionary<string, string>
                {
                    {Apikey, apikey},
                    {Application, application},
                    {Event, header},
                    {Description, description},
                    {Priortiy, ((sbyte) priority).ToString()},
                    {Url, url.OriginalString},
                    {Developerkey, devKey}
                };

            send(data);
        }

        #endregion

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
            HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(RequestUrl);
            webRequest.Method = RequestMethodType;
            webRequest.ContentType = RequestContentType;
            webRequest.ContentLength = data.Length;
            Stream requestStream = webRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            HttpWebResponse webResponse = (HttpWebResponse) webRequest.GetResponse();
            Stream responseStream = webResponse.GetResponseStream();
            string pageContent = null;
            if (responseStream != null)
            {
                StreamReader streamReader = new StreamReader(responseStream, Encoding.Default);
                pageContent = streamReader.ReadToEnd();
                streamReader.Close();
            }
            if (responseStream != null) responseStream.Close();
            webResponse.Close();
            analyseOutput(pageContent);
        }

        private void analyseOutput(string pageContent)
        {
#pragma warning disable 168
            string content = pageContent;
#pragma warning restore 168
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