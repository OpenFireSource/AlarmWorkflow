using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Jobs
{
    /// <summary>
    /// Implements a Job, that sends SMS with the sms77.de service.
    /// </summary>
    public class SmsJob : IJob
    {
        #region private member
        /// <summary>
        /// The errormsg, if an error occured.
        /// </summary>
        private string errormsg;

        /// <summary>
        /// A list of numbers, which stores all mobile phone numbers to send a SMS to them.
        /// </summary>
        private List<string> numbers;

        /// <summary>
        /// Username for SMS77.
        /// </summary>
        private string username;

        /// <summary>
        /// Username for SMS77.
        /// </summary>
        private string password;
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the SmsJob class.
        /// </summary>
        /// <param name="settings">Xml node that has all the information to connect to the lan-power-adapter.</param>
        /// <param name="debug">If true, sms will only send to debuging numbers.</param>
        public SmsJob(IXPathNavigable settings, bool debug)
        {
            this.numbers = new List<string>();
            XPathNavigator nav = settings.CreateNavigator();
            if (nav.UnderlyingObject is XmlElement)
            {
                this.username = ((XmlElement)nav.UnderlyingObject).Attributes["username"].Value;
                this.password = ((XmlElement)nav.UnderlyingObject).Attributes["password"].Value;
                XmlNodeList list = ((XmlElement)nav.UnderlyingObject).FirstChild.SelectNodes("Nummer"); // HACK: FirstChild hier falsch
                for (int i = 0; i < list.Count; i++)
                {
                    if (debug == true)
                    {
                        XmlAttribute atr = list.Item(i).Attributes["debug"];
                        if (atr != null)
                        {
                            if (atr.InnerText.ToUpperInvariant() == "TRUE")
                            {
                                this.numbers.Add(list.Item(i).InnerText);
                            }
                        }
                    }
                    else
                    {
                        this.numbers.Add(list.Item(i).InnerText);
                    }
                }
            }
            else
            {
                throw new ArgumentException("Settings is not an XmlElement");
            }

            this.errormsg = string.Empty;
        }
        #endregion

        #region iJob Member

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>
        /// Gets the error message.
        /// </value>
        public string ErrorMessage
        {
            get
            {
                return this.errormsg;
            }
        }

        /// <summary>
        /// Inherited by iJob interface. Sends sms with some operation information.
        /// </summary>
        /// <param name="einsatz">Current operation.</param>
        /// <returns>False when an error occured, otherwise true.</returns>
        public bool DoJob(Operation einsatz)
        {
            this.errormsg = string.Empty;
            string text = "Einsatz:%20" + SmsJob.PrepareString(einsatz.Ort.Substring(0, einsatz.Ort.IndexOf(" ", StringComparison.Ordinal))) + "%20" + SmsJob.PrepareString(einsatz.Meldebild) + "%20" + SmsJob.PrepareString(einsatz.Hinweis) + "%20Strasse:%20" + SmsJob.PrepareString(einsatz.Strasse);
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

        /// <summary>
        /// This methode url encodes a given string.
        /// </summary>
        /// <param name="str">The string that must be URL encoded.</param>
        /// <returns>The URL encoded string.</returns>
        private static string PrepareString(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        #endregion
    }
}
