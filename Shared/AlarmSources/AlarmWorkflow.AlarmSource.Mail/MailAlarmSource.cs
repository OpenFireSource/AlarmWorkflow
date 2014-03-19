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
        #region Fields

        private MailConfiguration _configuration;

        private ImapClient _imapClient;
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
        /// Raised when a new alarm has surfaced and processed for the Engine to handle.
        /// See documentation for further information.
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
                Thread.Sleep(_configuration.PollInterval);
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

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            _configuration.Dispose();
            _configuration = null;
        }

        #endregion

        #region Methods

        private void CheckImapMail()
        {
            try
            {
                using (_imapClient = new ImapClient(_configuration.ServerName, _configuration.Port, _configuration.SSL, null, _configuration.Encoding))
                {
                    try
                    {
                        _imapClient.Login(_configuration.UserName, _configuration.Password, AuthMethod.Login);
                        IEnumerable<uint> uids = _imapClient.Search(SearchCondition.Unseen());
                        foreach (MailMessage msg in uids.Select(uid => _imapClient.GetMessage(uid)))
                        {
                            Logger.Instance.LogFormat(LogType.Debug, this, "New mail " + msg.Subject);
                            MailOperation(msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.LogException(this, ex);
                    }
                }

            }
            catch (Exception ex)
            {
                //Sometimes an error occues e.g. if the internet connection was disconected or an timeout occured. 
                //Instead of "stopping" the complete alarmsource we log it and continue as normal.
                Logger.Instance.LogException(this, ex);
            }
        }

        private void MailOperation(MailMessage message)
        {
            if (message.Subject.ToLower().Contains(_configuration.MailSubject.ToLower()) &&
                message.From.Address.ToLower().Contains(_configuration.MailSender.ToLower()))
            {
                Logger.Instance.LogFormat(LogType.Debug, this, "Found matching mail.");
                Operation operation = null;
                if (_configuration.AnalyseAttachment)
                {
                    if (message.Attachments.Count > 0)
                    {
                        if (string.Equals(message.Attachments[0].Name, _configuration.AttachmentName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            Attachment attachment = message.Attachments[0];
                            byte[] buffer = new byte[attachment.ContentStream.Length];
                            attachment.ContentStream.Read(buffer, 0, buffer.Length);
                            string content;
                            if (_configuration.Encoding != null)
                            {
                                content = _configuration.Encoding.GetString(buffer);
                            }
                            else
                            {
                                content = Encoding.ASCII.GetString(buffer);
                            }
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

        #endregion
    }
}