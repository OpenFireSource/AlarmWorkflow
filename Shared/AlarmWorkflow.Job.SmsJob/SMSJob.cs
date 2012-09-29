using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Job.SmsJob
{
    /// <summary>
    /// Implements a Job, that sends SMS with the sms77.de service.
    /// </summary>
    public class SmsJob : IJob
    {
        #region private member

        private string errormsg;
        private List<string> numbers;
        private string username;
        private string password;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SmsJob"/> class.
        /// </summary>
        public SmsJob()
        {
            this.numbers = new List<string>();
        }

        #endregion

        #region IJob Members

        string IJob.ErrorMessage
        {
            get
            {
                return this.errormsg;
            }
        }

        bool IJob.DoJob(Operation einsatz)
        {
            this.errormsg = string.Empty;
            string text = "Einsatz:%20" + SmsJob.PrepareString(einsatz.City.Substring(0, einsatz.City.IndexOf(" ", StringComparison.Ordinal))) + "%20" + SmsJob.PrepareString(einsatz.Picture) + "%20" + SmsJob.PrepareString(einsatz.Hint) + "%20Strasse:%20" + SmsJob.PrepareString(einsatz.Street);
            foreach (string number in this.numbers)
            {
                try
                {
                    HttpWebRequest msg = (HttpWebRequest)System.Net.WebRequest.Create(new Uri("http://gateway.sms77.de/?u=" + this.username + "&p=" + this.password + "&to=" + number + "&text=" + text + "&type=basicplus"));
                    HttpWebResponse resp = (HttpWebResponse)msg.GetResponse();
                    Stream resp_steam = resp.GetResponseStream();
                    StreamReader streamreader = new StreamReader(resp_steam, Encoding.UTF8);
                    string response = streamreader.ReadToEnd();
                    streamreader.Close();
                    if (response != "100")
                    {
                        this.errormsg += "Fehler von sms77: " + response;
                    }
                }
                catch (ArgumentNullException ex)
                {
                    this.errormsg = ex.ToString();
                }
                catch (WebException ex)
                {
                    this.errormsg = ex.ToString();
                }
                catch (NotSupportedException ex)
                {
                    this.errormsg = ex.ToString();
                }
                catch (ProtocolViolationException ex)
                {
                    this.errormsg = ex.ToString();
                }
                catch (InvalidOperationException ex)
                {
                    this.errormsg = ex.ToString();
                }
                catch (SecurityException ex)
                {
                    this.errormsg = ex.ToString();
                }
                catch (OutOfMemoryException ex)
                {
                    this.errormsg = ex.ToString();
                }
                catch (IOException ex)
                {
                    this.errormsg = ex.ToString();
                }
            }

            if (string.IsNullOrEmpty(this.errormsg) == false)
            {
                return false;
            }

            return true;
        }

        void IJob.Initialize(IXPathNavigable settings)
        {
            XPathNavigator nav = settings.CreateNavigator();
            if (nav.UnderlyingObject is XmlElement)
            {
                this.username = ((XmlElement)nav.UnderlyingObject).Attributes["username"].Value;
                this.password = ((XmlElement)nav.UnderlyingObject).Attributes["password"].Value;
                XmlNodeList list = ((XmlElement)nav.UnderlyingObject).FirstChild.SelectNodes("Nummer"); // HACK: FirstChild hier falsch
                for (int i = 0; i < list.Count; i++)
                {
                    //if (debug == true)
                    //{
                    //    XmlAttribute atr = list.Item(i).Attributes["debug"];
                    //    if (atr != null)
                    //    {
                    //        if (atr.InnerText.ToUpperInvariant() == "TRUE")
                    //        {
                    //            this.numbers.Add(list.Item(i).InnerText);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    this.numbers.Add(list.Item(i).InnerText);
                    //}
                }
            }
            else
            {
                throw new ArgumentException("Settings is not an XmlElement");
            }
        }

        /// <summary>
        /// This methode url encodes a given string.
        /// </summary>
        /// <param name="str">The string that must be URL encoded.</param>
        /// <returns>The URL encoded string.</returns>
        private static string PrepareString(string str)
        {
            //return HttpUtility.UrlEncode(str);
            // TODO: .Net 2.0-equivalent to UrlEncode??
            return str;
        }

        #endregion

    }
}
