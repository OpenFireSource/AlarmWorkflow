//**********************//
//Philipp von Kirschbaum//
//**********************//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.XPath;
using System.IO;

using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

using Prowl;
using System.Configuration;

namespace AlarmWorkflow.Job.Prowl
{
    [Export("ProwlJob", typeof(IJob))]
    public class ProwlJob : IJob
    {
        #region private members

        /// <summary>
        /// The Prowl Configuration
        /// </summary>
        private ProwlClientConfiguration pcconfig;

        /// <summary>
        /// The Prowl Client
        /// </summary>
        private ProwlClient pclient;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ProwlJob class.
        /// </summary>
        public ProwlJob()
        {
        }

        #endregion

        #region IJob Members

        bool IJob.Initialize()
        {
            pcconfig = new ProwlClientConfiguration();

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\Config\Prowl.xml");

                IXPathNavigable settings = doc.CreateNavigator().SelectSingleNode("Prowl");

                XPathNavigator nav = settings.CreateNavigator();
                if (nav.UnderlyingObject is XmlElement)
                {
                    this.pcconfig.ApplicationName = ((XmlElement)nav.UnderlyingObject).Attributes["ApplicationName"].Value;
                    this.pcconfig.ProviderKey = ((XmlElement)nav.UnderlyingObject).Attributes["ProviderKey"].Value;
                    bool first = true;
                    foreach (XmlNode node in doc.GetElementsByTagName("API"))
                    {
                        if (first)
                        {
                            pcconfig.ApiKeychain = node.InnerText;
                            first = false;
                        }
                        else
                            pcconfig.ApiKeychain = "," + node.InnerText;
                    }

                    pclient = new ProwlClient(pcconfig);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, "An error occurred while configuration the Prowl Job.");
                Logger.Instance.LogException(this, ex);
            }
            return false;
        }

        void IJob.DoJob(Operation operation)
        {
            // Construct Notification text
            string body = "Einsatz:\r\n";
            body += "Zeitstempel: " + operation.Timestamp.ToString() + "\r\n";
            body += "Stichwort: " + operation.Keyword + "\r\n";
            body += "Einsatznr: " + operation.OperationNumber + "\r\n";
            body += "Hinweis: " + operation.Comment + "\r\n";
            body += "Mitteiler: " + operation.Messenger + "\r\n";
            body += "Einsatzort: " + operation.Location + "\r\n";
            body += "Straße: " + operation.Street + " " + operation.StreetNumber + "\r\n";
            body += "Ort: " + operation.ZipCode + " " + operation.City + "\r\n";
            body += "Objekt: " + operation.Property + "\r\n";

            ProwlNotification notifi = new ProwlNotification();
            notifi.Priority = ProwlNotificationPriority.Emergency;
            notifi.Event = "Feuerwehr Einsatz";
            notifi.Description = body;

            //Send the Message
            try
            {
                pclient.PostNotification(notifi);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, "An error occurred while sending the Prowl Messages.");
                Logger.Instance.LogException(this, ex);
            }
        }

        #endregion
    }
}
