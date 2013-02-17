using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using AlarmWorkflow.Shared.Addressing;
using AlarmWorkflow.Shared.Addressing.EntryObjects;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Engine;
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

        private List<MailAddressEntryObject> _recipients;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MailingJob class.
        /// </summary>
        public MailingJob()
        {
            _recipients = new List<MailAddressEntryObject>();
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
            bool useSsl = SettingsManager.Instance.GetSetting("MailingJob", "UseSsl").GetBoolean();

            _senderEmail = Helpers.ParseAddress(SettingsManager.Instance.GetSetting("MailingJob", "SenderAddress").GetString());
            if (_senderEmail == null)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.NoSenderAddressMessage);
                return false;
            }

            // Create new SMTP client for sending mails
            _smptClient = new SmtpClient(smtpHostName, smtpPort);
            _smptClient.EnableSsl = useSsl;
            if (smtpAuthenticate)
            {
                _smptClient.Credentials = new NetworkCredential(userName, userPassword);
            }

            // Get recipients
            var recipients = AddressBookManager.GetInstance().GetCustomObjects<MailAddressEntryObject>("Mail");
            _recipients.AddRange(recipients.Select(ri => ri.Item2));

            // Require at least one recipient for initialization to succeed
            if (_recipients.Count == 0)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.NoRecipientsMessage);
                return false;
            }

            return true;
        }

        void IJob.Execute(IJobContext context, Operation operation)
        {
            SendMail(operation);
        }

        private void SendMail(Operation operation)
        {
            using (MailMessage message = new MailMessage())
            {
                message.From = _senderEmail;
                foreach (var recipient in _recipients)
                {
                    switch (recipient.Type)
                    {
                        case MailAddressEntryObject.ReceiptType.CC: message.CC.Add(recipient.Address); break;
                        case MailAddressEntryObject.ReceiptType.Bcc: message.Bcc.Add(recipient.Address); break;
                        default:
                        case MailAddressEntryObject.ReceiptType.To: message.To.Add(recipient.Address); break;
                    }
                }

                // Construct message subject
                message.Subject = AlarmWorkflowConfiguration.Instance.FDInformation.Name + " - new alarm";

                // Construct body text
                StringBuilder bodyBuilder = new StringBuilder();
                bodyBuilder.AppendLine("Zeitstempel: " + operation.Timestamp.ToString());
                bodyBuilder.AppendLine("Stichwort: " + operation.Keywords.Keyword);
                bodyBuilder.AppendLine("Einsatzstichwort: " + operation.Keywords.EmergencyKeyword);
                bodyBuilder.AppendLine("Meldebild: " + operation.Picture);
                bodyBuilder.AppendLine("Einsatznr: " + operation.OperationNumber);
                bodyBuilder.AppendLine("Hinweis: " + operation.Comment);
                bodyBuilder.AppendLine("Mitteiler: " + operation.Messenger);
                bodyBuilder.AppendLine("Einsatzort: " + operation.Einsatzort.Location);
                bodyBuilder.AppendLine("Straße: " + operation.Einsatzort.Street + " " + operation.Einsatzort.StreetNumber);
                bodyBuilder.AppendLine("Kreuzung: " + operation.Einsatzort.Intersection);
                bodyBuilder.AppendLine("Ort: " + operation.Einsatzort.ZipCode + " " + operation.Einsatzort.City);
                bodyBuilder.AppendLine("Objekt: " + operation.Einsatzort.Property);
                bodyBuilder.AppendLine("Einsatzplan: " + operation.OperationPlan);
                bodyBuilder.AppendLine("Fahrzeuge: " + operation.Resources.ToString("{FullName} {RequestedEquipment} | ", null));


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

        bool IJob.IsAsync
        {
            get { return true; }
        }

        #endregion

        #region IDisposable Members

        void System.IDisposable.Dispose()
        {

        }

        #endregion
    }
}
