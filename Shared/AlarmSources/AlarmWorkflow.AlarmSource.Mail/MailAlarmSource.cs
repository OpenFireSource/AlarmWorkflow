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
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using AlarmWorkflow.AlarmSource.Mail.Properties;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;
using S22.Imap;

namespace AlarmWorkflow.AlarmSource.Mail
{
    [Export("MailAlarmSource", typeof(IAlarmSource))]
    [Information(DisplayName = "ExportAlarmSourceDisplayName", Description = "ExportAlarmSourceDescription")]
    internal class MailAlarmSource : IAlarmSource
    {
        #region Fields

        private readonly MailConfiguration _configuration;
        private ImapClient _imapClient;
        private IParser _mailParser;

        #endregion Fields

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MailAlarmSource" /> class.
        /// </summary>
        public MailAlarmSource()
        {
            _configuration = new MailConfiguration();
        }

        #endregion Constructors

        #region IAlarmSource Members

        /// <summary>
        /// Raised when a new alarm has surfaced and processed for the Engine to handle.
        /// See documentation for further information.
        /// </summary>
        public event EventHandler<AlarmSourceEventArgs> NewAlarm;

        void IAlarmSource.Initialize()
        {
            _mailParser = ExportedTypeLibrary.Import<IParser>(_configuration.ParserAlias);
        }

        void IAlarmSource.RunThread()
        {
            switch (_configuration.POPIMAP.ToLower())
            {
                case "imap":
                    while (true)
                    {
                        CheckImapMail();
                        Thread.Sleep(_configuration.PollInterval);
                    }

            }
        }

        private void OnNewAlarm(Operation operation)
        {
            EventHandler<AlarmSourceEventArgs> copy = NewAlarm;
            if (copy != null)
            {
                copy(this, new AlarmSourceEventArgs(operation));
            }
        }

        #endregion IAlarmSource Members

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion IDisposable Members

        #region Methods

        private void CheckImapMail()
        {
            using (_imapClient = new ImapClient(_configuration.ServerName, _configuration.Port, _configuration.SSL))
            {
                _imapClient.Login(_configuration.UserName, _configuration.Password, AuthMethod.Login);
                try
                {
                    uint[] uids = _imapClient.Search(SearchCondition.Unseen());
                    foreach (MailMessage msg in uids.Select(uid => _imapClient.GetMessage(uid)))
                    {
                        Logger.Instance.LogFormat(LogType.Debug, this, "New mail " + msg.Subject);
                        MailOperation(msg);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, this, ex.ToString());
                }
            }

        }

        private void MailOperation(MailMessage message)
        {
            if (message.Subject.ToLower().Contains(_configuration.MailSubject.ToLower()) &&
                message.From.Address.ToLower() == _configuration.MailSender.ToLower())
            {
                Operation operation = null;
                if (_configuration.AnalyseAttachment)
                {
                    if (message.Attachments.Count > 0)
                    {
                        if (message.Attachments[0].Name.ToLowerInvariant() == _configuration.AttachmentName.ToLowerInvariant())
                        {
                            Attachment attachment = message.Attachments[0];
                            byte[] buffer = new byte[attachment.ContentStream.Length];
                            attachment.ContentStream.Read(buffer, 0, buffer.Length);
                            string content = Encoding.UTF8.GetString(buffer);
                            string[] lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                            operation = _mailParser.Parse(lines);
                        }
                    }
                    else
                    {
                        Logger.Instance.LogFormat(LogType.Error, this, Resources.NoAttachmentFound);
                    }
                }
                else
                {
                    operation = _mailParser.Parse(message.Body.Split(new[] { "\r\n", "\n", "<br>" }, StringSplitOptions.RemoveEmptyEntries));
                }
                if (operation != null)
                {
                    OnNewAlarm(operation);
                }
            }
        }

        #endregion Methods
    }
}