using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using AlarmWorkflow.Shared;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Settings;

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

        private List<MailingEntryObject> _recipients;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MailingJob class.
        /// </summary>
        public MailingJob()
        {
            _recipients = new List<MailingEntryObject>();
        }

        #endregion

        #region IJob Members

        bool IJob.Initialize()
        {
            string smtpHostName = SettingsManager.Instance.GetSetting("MailingJob", "HostName").GetString();
            string userName = SettingsManager.Instance.GetSetting("MailingJob", "UserName").GetString();
            string userPassword = SettingsManager.Instance.GetSetting("MailingJob", "Password").GetString();
            int smtpPort = SettingsManager.Instance.GetSetting("MailingJob", "Port").GetInt32();
            bool smtpAuthenticate = SettingsManager.Instance.GetSetting("MailingJob", "Authenticate").GetBoolean();

            _senderEmail = Helpers.ParseAddress(SettingsManager.Instance.GetSetting("MailingJob", "SenderAddress").GetString());
            if (_senderEmail == null)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.NoSenderAddressMessage);
                return false;
            }

            // Create new SMTP client for sending mails
            _smptClient = new SmtpClient(smtpHostName, smtpPort);
            if (smtpAuthenticate)
            {
                _smptClient.Credentials = new NetworkCredential(userName, userPassword);
            }

            // Get recipients
            var recipients = AlarmWorkflowConfiguration.Instance.AddressBook.GetCustomObjects<MailingEntryObject>("Mail");
            _recipients.AddRange(recipients.Select(ri => ri.Item2));

            // Require at least one recipient for initialization to succeed
            if (_recipients.Count == 0)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.NoRecipientsMessage);
                return false;
            }

            return true;
        }

        void IJob.DoJob(Operation operation)
        {
            // Send it asynchronously because it may take a while!
            ThreadPool.QueueUserWorkItem(o => SendMailThread(operation));
        }

        private void SendMailThread(Operation operation)
        {
            using (MailMessage message = new MailMessage())
            {
                message.From = _senderEmail;
                foreach (var recipient in _recipients)
                {
                    switch (recipient.Type)
                    {
                        case MailingEntryObject.ReceiptType.CC: message.CC.Add(recipient.Address); break;
                        case MailingEntryObject.ReceiptType.Bcc: message.Bcc.Add(recipient.Address); break;
                        default:
                        case MailingEntryObject.ReceiptType.To: message.To.Add(recipient.Address); break;
                    }
                }

                // Construct message subject
                message.Subject = AlarmWorkflowConfiguration.Instance.FDInformation.Name + " - new alarm";

                // Construct body text
                StringBuilder bodyBuilder = new StringBuilder();
                bodyBuilder.AppendLine("Zeitstempel: " + operation.Timestamp.ToString());
                bodyBuilder.AppendLine("Stichwort: " + operation.Keyword);
                bodyBuilder.AppendLine("Einsatzstichwort: " + operation.EmergencyKeyword);
                bodyBuilder.AppendLine("Meldebild: " + operation.Picture);
                bodyBuilder.AppendLine("Einsatznr: " + operation.OperationNumber);
                bodyBuilder.AppendLine("Hinweis: " + operation.Comment);
                bodyBuilder.AppendLine("Mitteiler: " + operation.Messenger);
                bodyBuilder.AppendLine("Einsatzort: " + operation.Location);
                bodyBuilder.AppendLine("Straße: " + operation.Street + " " + operation.StreetNumber);
                bodyBuilder.AppendLine("Kreuzung: " + operation.GetCustomData<string>("Intersection"));
                bodyBuilder.AppendLine("Ort: " + operation.ZipCode + " " + operation.City);
                bodyBuilder.AppendLine("Objekt: " + operation.Property);
                bodyBuilder.AppendLine("Einsatzplan: " + operation.OperationPlan);
                bodyBuilder.AppendLine("Fahrzeuge: " + operation.Resources.ToString("{FullName} | {RequestedEquipment}", null));


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
                        Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.SendExceptionSMTPMessage, smtpException.StatusCode, smtpException.Message);
                    }
                    else
                    {
                        Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.SendExceptionMessage);
                    }
                    Logger.Instance.LogException(this, ex);
                }
            }
        }

        #endregion
    }
}
