using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Jobs
{
    /// <summary>
    /// Implements a Job, that send emails with all the operation information.
    /// </summary>
    [Export("MailingJob", typeof(IJob))]
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

        void IJob.Initialize(IXPathNavigable settings)
        {
            // NOTE: TENTATIVE CODE until settings are stored more dynamical!
            settings = settings.CreateNavigator().SelectSingleNode("Mailing");

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
            System.Net.Mail.MailMessage message = new MailMessage();
            message.From = new MailAddress(this.fromEmail);
            foreach (string ma in this.emaillist)
            {
                message.To.Add(ma);
            }

            message.Subject = "FFWPlanegg Einsatz";
            message.Body += "Einsatznr: " + einsatz.Einsatznr + "\n";
            message.Body += "Mitteiler: " + einsatz.Mitteiler + "\n";
            message.Body += "Einsatzort: " + einsatz.Einsatzort + "\n";
            message.Body += "Strasse: " + einsatz.Strasse + "\n";
            message.Body += "Kreuzung: " + einsatz.Kreuzung + "\n";
            message.Body += "Ort: " + einsatz.Ort + "\n";
            message.Body += "Objekt: " + einsatz.Objekt + "\n";
            message.Body += "Meldebild: " + einsatz.Meldebild + "\n";
            message.Body += "Hinweis: " + einsatz.Hinweis + "\n";
            message.Body += "Einsatzplan: " + einsatz.Einsatzplan + "\n";

            message.BodyEncoding = Encoding.UTF8;

            // Authentifizierung
            NetworkCredential credential = new NetworkCredential(this.user, this.pwd);
            client.Credentials = credential;

            // send
            try
            {
                client.Send(message);
            }
            catch (ArgumentNullException e)
            {
                this.errormsg = e.ToString();
                return false;
            }
            catch (ArgumentOutOfRangeException e)
            {
                this.errormsg = e.ToString();
                return false;
            }
            catch (InvalidOperationException e)
            {
                this.errormsg = e.ToString();
                return false;
            }
            catch (SmtpException e)
            {
                this.errormsg = e.ToString();
                return false;
            }

            return true;
        }

        #endregion
    }
}
