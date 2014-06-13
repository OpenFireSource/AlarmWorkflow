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
using System.IO;
using System.Text;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.AlarmSource.Mail
{
    internal sealed class MailConfiguration : DisposableObject
    {
        #region Fields

        private ISettingsServiceInternal _settings;

        #endregion

        #region Properties

        internal string ServerName { get; private set; }
        internal ushort Port { get; private set; }
        internal string UserName { get; private set; }
        internal string Password { get; private set; }
        internal int PollInterval { get; private set; }
        internal bool SSL { get; private set; }

        internal string MailSubject { get; private set; }
        internal string MailSender { get; private set; }

        internal bool AnalyseAttachment { get; private set; }
        internal string AttachmentName { get; private set; }
        internal string ParserAlias { get; private set; }
        internal Encoding Encoding { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MailConfiguration"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public MailConfiguration(IServiceProvider serviceProvider)
        {
            _settings = serviceProvider.GetService<ISettingsServiceInternal>();

            ServerName = _settings.GetSetting("MailAlarmSource", "ServerName").GetValue<string>();
            Port = (ushort)_settings.GetSetting("MailAlarmSource", "Port").GetValue<int>();
            UserName = _settings.GetSetting("MailAlarmSource", "UserName").GetValue<string>();
            Password = _settings.GetSetting("MailAlarmSource", "Password").GetValue<string>();
            PollInterval = _settings.GetSetting("MailAlarmSource", "PollInterval").GetValue<int>();
            SSL = _settings.GetSetting("MailAlarmSource", "SSL").GetValue<bool>();

            MailSubject = _settings.GetSetting("MailAlarmSource", "MailSubject").GetValue<string>();
            MailSender = _settings.GetSetting("MailAlarmSource", "MailSender").GetValue<string>();

            AnalyseAttachment = _settings.GetSetting("MailAlarmSource", "AnalyseAttachment").GetValue<bool>();
            AttachmentName = _settings.GetSetting("MailAlarmSource", "AttachmentName").GetValue<string>();
            ParserAlias = _settings.GetSetting("MailAlarmSource", "MailParser").GetValue<string>();
            int codepage = _settings.GetSetting("MailAlarmSource", "CodePage").GetValue<int>();
            
            try
            {
                if (codepage != 0)
                {
                    Encoding = Encoding.GetEncoding(codepage);
                }
            }
            catch (Exception)
            {
                Encoding = null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected override void DisposeCore()
        {
            _settings = null;
        }

        #endregion
    }
}