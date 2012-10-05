using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Job.MailingJob
{
    /// <summary>
    /// Implements a Job, that send emails with all the operation information.
    /// </summary>
    [Export("MailingJob", typeof(IJob))]
    sealed class MailingJob : IJob
    {
        #region Fields

        private SmtpClient _smptClient;

        private string server;
        private string fromEmail;
        private string user;
        private string pwd;
        private List<string> emaillist;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MailingJob class.
        /// </summary>
        public MailingJob()
        {

        }

        #endregion

        #region Event handlers

        private void SmptClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Logger.Instance.LogFormat(LogType.Error, this, "An error occurred while sending the mail!");
                Logger.Instance.LogException(this, e.Error);
            }
        }

        #endregion

        #region IJob Members

        void IJob.Initialize()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\Config\MailingJobConfiguration.xml");

            IXPathNavigable settings = doc.CreateNavigator().SelectSingleNode("Mailing");

            this.emaillist = new List<string>();
            XPathNavigator nav = settings.CreateNavigator();
            if (nav.UnderlyingObject is XmlElement)
            {
                this.server = nav.SelectSingleNode("MailServer").InnerXml;
                this.fromEmail = nav.SelectSingleNode("FromMail").InnerXml;
                this.user = nav.SelectSingleNode("User").InnerXml;
                this.pwd = nav.SelectSingleNode("Pwd").InnerXml;
                XmlNode emailnode = ((XmlElement)nav.UnderlyingObject).SelectSingleNode("MailAdresses");
                XmlNodeList emails = emailnode.SelectNodes("MailAddress");
                for (int i = 0; i < emails.Count; i++)
                {
                    this.emaillist.Add(emails.Item(i).InnerText);
                }
            }
            else
            {
                throw new ArgumentException("Settings is not an XmlElement");
            }

            // Create new SMTP client for sending mails
            _smptClient = new SmtpClient(server);
            _smptClient.SendCompleted += new SendCompletedEventHandler(SmptClient_SendCompleted);
            NetworkCredential credential = new NetworkCredential(this.user, this.pwd);
            _smptClient.Credentials = credential;
        }

        void IJob.DoJob(Operation operation)
        {
            // create the Mail
            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(this.fromEmail);
                foreach (string ma in this.emaillist)
                {
                    message.To.Add(ma);
                }

                // TODO: Make this customizable...
                message.Subject = "FFWPlanegg Einsatz";
                message.Body += "Einsatznr: " + operation.OperationNumber + "\n";
                message.Body += "Mitteiler: " + operation.Messenger + "\n";
                message.Body += "Einsatzort: " + operation.Location + "\n";
                message.Body += "Strasse: " + operation.Street + "\n";
                message.Body += "Kreuzung: " + operation.CustomData["Intersection"] + "\n";
                message.Body += "Ort: " + operation.City + "\n";
                message.Body += "Objekt: " + operation.Property + "\n";
                message.Body += "Meldebild: " + operation.CustomData["Picture"] + "\n";
                message.Body += "Hinweis: " + operation.Comment + "\n";
                message.Body += "Einsatzplan: " + operation.CustomData["PlanOfAction"] + "\n";

                message.BodyEncoding = Encoding.UTF8;

                // Send the message asynchronously
                _smptClient.SendAsync(message, null);
            }
        }

        #endregion
    }
}
