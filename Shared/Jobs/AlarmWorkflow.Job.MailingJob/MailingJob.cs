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
using AlarmWorkflow.Shared.Addressing;
using AlarmWorkflow.Shared.Addressing.EntryObjects;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Engine;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.ObjectExpressions;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Job.MailingJob
{
    /// <summary>
    /// Implements a Job that send emails with the common alarm information.
    /// </summary>
    [Export("MailingJob", typeof(IJob))]
    [Information(DisplayName = "ExportJobDisplayName", Description = "ExportJobDescription")]
    sealed class MailingJob : IJob
    {
        #region Fields

        private SmtpClient _smptClient;
        private MailAddress _senderEmail;

        private List<MailAddressEntryObject> _recipients;

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

            _smptClient = new SmtpClient(smtpHostName, smtpPort);
            _smptClient.EnableSsl = useSsl;
            if (smtpAuthenticate)
            {
                _smptClient.Credentials = new NetworkCredential(userName, userPassword);
            }

            _mailBodyFormat = SettingsManager.Instance.GetSetting("MailingJob", "EMailBody").GetString();
            _mailSubject = SettingsManager.Instance.GetSetting("MailingJob", "EMailSubject").GetString();
            if (string.IsNullOrWhiteSpace(_mailSubject))
            {
                _mailSubject = AlarmWorkflowConfiguration.Instance.FDInformation.Name + " - Neuer Alarm";
            }

            _attachImage = SettingsManager.Instance.GetSetting("MailingJob", "AttachImage").GetBoolean();

            var recipients = AddressBookManager.GetInstance().GetCustomObjects<MailAddressEntryObject>("Mail");
            _recipients.AddRange(recipients.Select(ri => ri.Item2));

            if (_recipients.Count == 0)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.NoRecipientsMessage);
                return false;
            }

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

                try
                {
                    message.Subject = operation.ToString(_mailSubject, ObjectFormatterOptions.RemoveNewlines, null);
                    message.Body = operation.ToString(_mailBodyFormat);

                    message.BodyEncoding = Encoding.UTF8;
                    message.Priority = MailPriority.High;
                    message.IsBodyHtml = false;

                    if (_attachImage)
                    {
                        Attachment attachment = null;
                        if (context.Parameters.Keys.Contains("ImagePath"))
                        {
                            string imagePath = (string)context.Parameters["ImagePath"];
                            if (!string.IsNullOrWhiteSpace(imagePath))
                            {
                                if (File.Exists(imagePath))
                                {
                                    string[] convertTiffToJpeg = Helpers.ConvertTiffToJpeg(imagePath);
                                    using (Stream stream = Helpers.CombineBitmap(convertTiffToJpeg).ToStream(ImageFormat.Jpeg))
                                    {
                                        attachment = new Attachment(stream, "fax.jpg");
                                    }
                                    foreach (string s in convertTiffToJpeg)
                                    {
                                        File.Delete(s);
                                    }
                                }
                            }
                        }
                        if (attachment != null)
                        {
                            message.Attachments.Add(attachment);
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