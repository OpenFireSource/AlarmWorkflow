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
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using AlarmWorkflow.AlarmSource.Mail.Properties;
using AlarmWorkflow.BackendService.EngineContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;
using S22.Imap;

namespace AlarmWorkflow.AlarmSource.Mail
{
    [Export("MailAlarmSource", typeof(IAlarmSource))]
    [Information(DisplayName = "ExportAlarmSourceDisplayName", Description = "ExportAlarmSourceDescription")]
    class MailAlarmSource : IAlarmSource
    {
        #region Constants

        private const int PollIntervalMs = 10000;

        #endregion

        #region Fields

        private MailConfiguration _configuration;

        private IParser _mailParser;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MailAlarmSource" /> class.
        /// </summary>
        public MailAlarmSource()
        {
        }

        #endregion

        #region IAlarmSource Members

        /// <summary>
        /// Raised when a new alarm has surfaced by successfully processing an incoming mail.
        /// </summary>
        public event EventHandler<AlarmSourceEventArgs> NewAlarm;

        void IAlarmSource.Initialize(IServiceProvider serviceProvider)
        {
            _configuration = new MailConfiguration(serviceProvider);

            _mailParser = ExportedTypeLibrary.Import<IParser>(_configuration.ParserAlias);
        }

        void IAlarmSource.RunThread()
        {
            while (true)
            {
                CheckImapMail();

                Thread.Sleep(PollIntervalMs);
            }
        }

        private void OnNewAlarm(Operation operation)
        {
            var copy = NewAlarm;
            if (copy != null)
            {
                copy(this, new AlarmSourceEventArgs(operation));
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (_configuration != null)
            {
                _configuration.Dispose();
                _configuration = null;
            }
        }

        #endregion

        #region Methods

        private void CheckImapMail()
        {
            try
            {
                using (ImapClient imapClient = new ImapClient(_configuration.ServerName, _configuration.Port, _configuration.SSL, null, _configuration.Encoding))
                {
                    imapClient.Login(_configuration.UserName, _configuration.Password, AuthMethod.Login);

                    IEnumerable<uint> uids = imapClient.Search(SearchCondition.Unseen());
                    foreach (MailMessage msg in uids.Select(uid => imapClient.GetMessage(uid)))
                    {
                        try
                        {
                            ProcessMail(msg);
                        }
                        catch (Exception ex)
                        {
                            Logger.Instance.LogException(this, ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Sometimes an error occures, e.g. if the network was disconected or a timeout occured.
                Logger.Instance.LogException(this, ex);
            }
        }

        private void ProcessMail(MailMessage message)
        {
            Logger.Instance.LogFormat(LogType.Trace, this, Resources.ReceivedMailInfo, message.From, message.Subject);

            bool isSubjectMatch = message.Subject.ToLower().Contains(_configuration.MailSubject.ToLower());
            bool isMessageMatch = message.From.Address.ToLower().Contains(_configuration.MailSender.ToLower());

            if (isSubjectMatch && isMessageMatch)
            {
                string[] lines = (_configuration.AnalyzeAttachment) ? AnalyzeAttachment(message) : AnalyzeBody(message);
                if (lines != null)
                {
                    Operation operation = _mailParser.Parse(lines);
                    OnNewAlarm(operation);
                }
            }
            else
            {
                Logger.Instance.LogFormat(LogType.Info, this, Resources.MailDoesNotMatchCriteria);
            }
        }

        private string[] AnalyzeAttachment(MailMessage message)
        {
            Attachment attachment = message.Attachments.FirstOrDefault(att => string.Equals(att.Name, _configuration.AttachmentName, StringComparison.InvariantCultureIgnoreCase));
            if (attachment != null)
            {
                return GetLinesFromAttachment(attachment);
            }

            Logger.Instance.LogFormat(LogType.Info, this, Resources.NoAttachmentFound);
            return null;
        }

        private string[] GetLinesFromAttachment(Attachment attachment)
        {
            byte[] buffer = new byte[attachment.ContentStream.Length];
            attachment.ContentStream.Read(buffer, 0, buffer.Length);

            string content = GetContentWithEncoding(buffer);
            return content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private string GetContentWithEncoding(byte[] buffer)
        {
            Encoding encodingToUse = _configuration.Encoding ?? Encoding.ASCII;
            return encodingToUse.GetString(buffer);
        }

        private string[] AnalyzeBody(MailMessage message)
        {
            return message.Body.Split(new[] { "\r\n", "\n", "<br>" }, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion
    }
}