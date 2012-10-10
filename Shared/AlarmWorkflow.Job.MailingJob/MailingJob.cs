using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Config;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Job.MailingJob
{
    /// <summary>
    /// Implements a Job that send emails with the common alarm information.
    /// </summary>
    [Export("MailingJob", typeof(IJob))]
    sealed class MailingJob : IJob
    {
        #region Fields

        private SmtpClient _smptClient;

        private string _server;
        private string _userName;
        private string _userPassword;
        private MailAddress _senderEmail;
        private List<MailAddress> _recipients;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MailingJob class.
        /// </summary>
        public MailingJob()
        {
            _recipients = new List<MailAddress>();
        }

        #endregion

        #region IJob Members

        bool IJob.Initialize()
        {
            string configFile = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\Config\MailingJobConfiguration.xml";
            if (!File.Exists(configFile))
            {
                return false;
            }

            XDocument doc = XDocument.Load(configFile);
            _server = doc.Root.TryGetElementValue("MailServer", "localhost");
            _userName = doc.Root.TryGetElementValue("UserName", "johndoe");
            _userPassword = doc.Root.TryGetElementValue("Password", null);

            _senderEmail = new MailAddress(doc.Root.TryGetElementValue("SenderAddress", "johndoe@domain.com"));

            foreach (XElement recipientE in doc.Root.Element("Recipients").Elements("Recipient"))
            {
                string address = recipientE.TryGetAttributeValue("Address", null);
                try
                {
                    MailAddress ma = new MailAddress(address);
                    _recipients.Add(ma);
                }
                catch (FormatException)
                {
                    // The address failed to parse.
                    Logger.Instance.LogFormat(LogType.Warning, this, "The address '{0}' failed to parse. This is usually an indication that the E-Mail address is invalid formatted.", address);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogException(this, ex);
                }
            }

            // Create new SMTP client for sending mails
            _smptClient = new SmtpClient(_server);
            NetworkCredential credential = new NetworkCredential(this._userName, this._userPassword);
            _smptClient.Credentials = credential;

            return true;
        }

        void IJob.DoJob(Operation operation)
        {
            using (MailMessage message = new MailMessage())
            {
                message.From = _senderEmail;
                foreach (MailAddress ma in _recipients)
                {
                    message.To.Add(ma);
                }

                // TODO: Make this customizable...
                message.Subject = Configuration.Instance.FDInformation.Name + " - New alarm";
                StringBuilder bodyBuilder = new StringBuilder();
                bodyBuilder.AppendLine("Stichwort: " + operation.Keyword);
                bodyBuilder.AppendLine("Einsatznr: " + operation.OperationNumber);
                bodyBuilder.AppendLine("Mitteiler: " + operation.Messenger);
                bodyBuilder.AppendLine("Einsatzort: " + operation.Location);
                bodyBuilder.AppendLine("Strasse: " + operation.Street + " " + operation.StreetNumber);
                bodyBuilder.AppendLine("Ort: " + operation.ZipCode + " " + operation.City);
                bodyBuilder.AppendLine("Objekt: " + operation.Property);
                bodyBuilder.AppendLine("Hinweis: " + operation.Comment);

                message.Body = bodyBuilder.ToString();
                message.BodyEncoding = Encoding.UTF8;
                message.Priority = MailPriority.High;
                // No HTML is needed
                message.IsBodyHtml = false;

                // Send the message asynchronously
                try
                {
                    _smptClient.Send(message);
                }
                catch (Exception ex)
                {
                    SmtpException smtpException = ex as SmtpException;
                    if (smtpException != null)
                    {
                        Logger.Instance.LogFormat(LogType.Error, this, "An SMTP error occurred while sending the mail! Status code: {0}, Message: {1}", smtpException.StatusCode, smtpException.Message);
                    }
                    else
                    {
                        Logger.Instance.LogFormat(LogType.Error, this, "An unknown error occurred while sending the mail! Please check the log for more information.");
                    }

                    Logger.Instance.LogException(this, ex);
                }
            }
        }

        #endregion
    }
}
