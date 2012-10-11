using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
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

        private MailAddress _senderEmail;
        private Dictionary<string, MailAddress> _recipients;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MailingJob class.
        /// </summary>
        public MailingJob()
        {
            _recipients = new Dictionary<string, MailAddress>();
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
            _senderEmail = new MailAddress(doc.Root.TryGetElementValue("SenderAddress", "johndoe@domain.com"));

            // Add all recipients with their reception type (To, CC, BCC)
            foreach (XElement recipientE in doc.Root.Element("Recipients").Elements("Recipient"))
            {
                string address = recipientE.TryGetAttributeValue("Address", null);
                string type = recipientE.TryGetAttributeValue("Type", "To").ToUpperInvariant();
                try
                {
                    MailAddress ma = new MailAddress(address);

                    // Allow only To, CC and Bcc
                    if (type != "TO" && type != "CC" && type != "BCC")
                    {
                        type = "TO";
                    }

                    _recipients.Add(type, ma);
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

            XElement serverSettingsE = doc.Root.Element("MailServer");
            string smtpHostName = serverSettingsE.TryGetElementValue("HostName", "localhost");
            string userName = serverSettingsE.TryGetElementValue("UserName", "johndoe");
            string userPassword = serverSettingsE.TryGetElementValue("Password", null);
            int smtpPort = serverSettingsE.TryGetElementValue("Port", 25);
            bool smtpAuthenticate = serverSettingsE.TryGetElementValue("Authenticate", true);

            // Create new SMTP client for sending mails
            _smptClient = new SmtpClient(smtpHostName, smtpPort);
            if (smtpAuthenticate)
            {
                _smptClient.Credentials = new NetworkCredential(userName, userPassword);
            }
            return true;
        }

        void IJob.DoJob(Operation operation)
        {
            using (MailMessage message = new MailMessage())
            {
                message.From = _senderEmail;
                foreach (var addr in _recipients)
                {
                    switch (addr.Key)
                    {
                        case "CC": message.CC.Add(addr.Value); break;
                        case "BCC": message.Bcc.Add(addr.Value); break;
                        default:
                        case "TO": message.To.Add(addr.Value); break;
                    }
                }

                // Construct message subject
                message.Subject = Configuration.Instance.FDInformation.Name + " - ^new alarm";

                // Construct body text
                StringBuilder bodyBuilder = new StringBuilder();
                bodyBuilder.AppendLine("Stichwort: " + operation.Keyword);
                bodyBuilder.AppendLine("Einsatznr: " + operation.OperationNumber);
                bodyBuilder.AppendLine("Hinweis: " + operation.Comment);
                bodyBuilder.AppendLine("Mitteiler: " + operation.Messenger);
                bodyBuilder.AppendLine("Einsatzort: " + operation.Location);
                bodyBuilder.AppendLine("Straße: " + operation.Street + " " + operation.StreetNumber);
                bodyBuilder.AppendLine("Ort: " + operation.ZipCode + " " + operation.City);
                bodyBuilder.AppendLine("Objekt: " + operation.Property);

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
