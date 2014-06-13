// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using AlarmWorkflow.BackendService.AddressingContracts;
using AlarmWorkflow.BackendService.AddressingContracts.EntryObjects;
using AlarmWorkflow.BackendService.EngineContracts;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.ObjectExpressions;

namespace AlarmWorkflow.Job.MailingJob
{
    /// <summary>
    /// Implements a Job that send emails with the common alarm information.
    /// </summary>
    [Export("MailingJob", typeof(IJob))]
    [Information(DisplayName = "ExportJobDisplayName", Description = "ExportJobDescription")]
    sealed class MailingJob : IJob
    {
        #region Constants

        private const string ImageAttachmentFileName = "fax.jpg";

        #endregion

        #region Fields

        private ISettingsServiceInternal _settings;
        private IAddressingServiceInternal _addressing;

        private SmtpClient _smptClient;
        private MailAddress _senderEmail;

        private string _mailSubject;
        private string _mailBodyFormat;
        private bool _attachImage;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MailingJob class.
        /// </summary>
        public MailingJob()
        {

        }

        #endregion

        #region IJob Members

        bool IJob.Initialize(IServiceProvider serviceProvider)
        {
            _settings = serviceProvider.GetService<ISettingsServiceInternal>();
            _addressing = serviceProvider.GetService<IAddressingServiceInternal>();

            string smtpHostName = _settings.GetSetting("MailingJob", "HostName").GetValue<string>();
            string userName = _settings.GetSetting("MailingJob", "UserName").GetValue<string>();
            string userPassword = _settings.GetSetting("MailingJob", "Password").GetValue<string>();
            int smtpPort = _settings.GetSetting("MailingJob", "Port").GetValue<int>();
            bool smtpAuthenticate = _settings.GetSetting("MailingJob", "Authenticate").GetValue<bool>();
            bool useSsl = _settings.GetSetting("MailingJob", "UseSsl").GetValue<bool>();

            _senderEmail = Helpers.ParseAddress(_settings.GetSetting("MailingJob", "SenderAddress").GetValue<string>());
            if (_senderEmail == null)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.NoSenderAddressMessage);
                return false;
            }

            _smptClient = new SmtpClient(smtpHostName, smtpPort);
            _smptClient.EnableSsl = useSsl;
            if (smtpAuthenticate)
            {
                _smptClient.Credentials = new NetworkCredential(userName, userPassword);
            }

            _mailBodyFormat = _settings.GetSetting("MailingJob", "EMailBody").GetValue<string>();
            _mailSubject = _settings.GetSetting("MailingJob", "EMailSubject").GetValue<string>();
            if (string.IsNullOrWhiteSpace(_mailSubject))
            {
                _mailSubject = _settings.GetSetting("Shared", "FD.Name").GetValue<string>() + " - Neuer Alarm";
            }

            _attachImage = _settings.GetSetting("MailingJob", "AttachImage").GetValue<bool>();

            return true;
        }

        void IJob.Execute(IJobContext context, Operation operation)
        {
            if (context.Phase != JobPhase.AfterOperationStored)
            {
                return;
            }

            SendMail(operation, context);
        }

        private void SendMail(Operation operation, IJobContext context)
        {
            using (MailMessage message = new MailMessage())
            {
                IList<MailAddressEntryObject> recipients = GetMailRecipients(operation);
                if (recipients.Count == 0)
                {
                    Logger.Instance.LogFormat(LogType.Info, this, Properties.Resources.NoRecipientsMessage);
                    return;
                }

                message.From = _senderEmail;
                foreach (MailAddressEntryObject recipient in recipients)
                {
                    switch (recipient.Type)
                    {
                        case MailAddressEntryObject.ReceiptType.CC: message.CC.Add(recipient.Address); break;
                        case MailAddressEntryObject.ReceiptType.Bcc: message.Bcc.Add(recipient.Address); break;
                        default:
                        case MailAddressEntryObject.ReceiptType.To: message.To.Add(recipient.Address); break;
                    }
                }

                try
                {
                    message.Subject = operation.ToString(_mailSubject, ObjectFormatterOptions.RemoveNewlines, null);
                    message.Body = operation.ToString(_mailBodyFormat);

                    message.BodyEncoding = Encoding.UTF8;
                    message.Priority = MailPriority.High;
                    message.IsBodyHtml = false;

                    if (_attachImage)
                    {
                        try
                        {
                            AttachImage(context, message);
                        }
                        catch (Exception ex)
                        {
                            Logger.Instance.LogException(this, ex);
                            Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.AttachImageFailed);
                        }
                    }

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

        private void AttachImage(IJobContext context, MailMessage message)
        {
            if (context.Parameters.Keys.Contains("ImagePath"))
            {
                string imagePath = (string)context.Parameters["ImagePath"];
                if (!string.IsNullOrWhiteSpace(imagePath))
                {
                    if (File.Exists(imagePath))
                    {
                        string[] splitFiles = Helpers.ConvertTiffToJpegAndSplit(imagePath);
                        Stream stream = Helpers.CombineBitmap(splitFiles).ToStream(ImageFormat.Jpeg);

                        message.Attachments.Add(new Attachment(stream, ImageAttachmentFileName));

                        foreach (string s in splitFiles)
                        {
                            File.Delete(s);
                        }
                    }
                }
            }
        }

        private IList<MailAddressEntryObject> GetMailRecipients(Operation operation)
        {
            var recipients = _addressing.GetCustomObjectsFiltered<MailAddressEntryObject>(MailAddressEntryObject.TypeId, operation);
            return recipients.Select(ri => ri.Item2).ToList();
        }

        bool IJob.IsAsync
        {
            get { return true; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {

        }

        #endregion
    }
}