using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Job.MailingJob
{
    /// <summary>
    /// Implements a Job, that send emails with all the operation information.
    /// </summary>
    public class MailingJob : IJob
    {
        #region private member
        /// <summary>
        /// The errormsg, if an error occured.
        /// </summary>
        private string errormsg;

        /// <summary>
        /// URL of the SMTP server.
        /// </summary>
        private string server;

        /// <summary>
        /// Sender email address.
        /// </summary>
        private string fromEmail;

        /// <summary>
        /// Username of the SMTP server.
        /// </summary>
        private string user;

        /// <summary>
        /// Password of the SMTP server.
        /// </summary>
        private string pwd;

        /// <summary>
        /// Stores all the Emails.
        /// </summary>
        private List<string> emaillist;
        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the MailingJob class.
        /// </summary>
        public MailingJob()
        {

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
                    //if (debug == true)
                    //{
                    //    XmlAttribute atr = emails.Item(i).Attributes["debug"];
                    //    if (atr != null)
                    //    {
                    //        if (atr.InnerText.ToUpperInvariant() == "TRUE")
                    //        {
                    //            this.emaillist.Add(emails.Item(i).InnerText);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    this.emaillist.Add(emails.Item(i).InnerText);
                    //}
                }
            }
            else
            {
                throw new ArgumentException("Settings is not an XmlElement");
            }

            this.errormsg = string.Empty;
        }

        bool IJob.DoJob(Operation einsatz)
        {
            this.errormsg = string.Empty;
            SmtpClient client = new SmtpClient(this.server);

            // create the Mail
            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(this.fromEmail);
                foreach (string ma in this.emaillist)
                {
                    message.To.Add(ma);
                }

                message.Subject = "FFWPlanegg Einsatz";
                message.Body += "Einsatznr: " + einsatz.OperationNumber + "\n";
                message.Body += "Mitteiler: " + einsatz.Messenger + "\n";
                message.Body += "Einsatzort: " + einsatz.Location + "\n";
                message.Body += "Strasse: " + einsatz.Street + "\n";
                message.Body += "Kreuzung: " + einsatz.Intersection + "\n";
                message.Body += "Ort: " + einsatz.City + "\n";
                message.Body += "Objekt: " + einsatz.Property + "\n";
                message.Body += "Meldebild: " + einsatz.Picture + "\n";
                message.Body += "Hinweis: " + einsatz.Hint + "\n";
                message.Body += "Einsatzplan: " + einsatz.PlanOfAction + "\n";

                message.BodyEncoding = Encoding.UTF8;

                // Authentifizierung
                NetworkCredential credential = new NetworkCredential(this.user, this.pwd);
                client.Credentials = credential;

                // send
                try
                {
                    client.Send(message);
                }
                catch (Exception e)
                {
                    this.errormsg = e.ToString();
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
