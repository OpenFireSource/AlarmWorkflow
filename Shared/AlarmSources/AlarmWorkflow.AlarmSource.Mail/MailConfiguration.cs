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

using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.AlarmSource.Mail
{
    /// <summary>
    ///     Represents the current configuration. Wraps the SettingsManager-calls.
    /// </summary>
    internal sealed class MailConfiguration
    {
        #region Properties

        internal string ServerName { get; private set; }
        internal ushort Port { get; private set; }
        internal string UserName { get; private set; }
        internal string Password { get; private set; }
        internal int PollInterval { get; private set; }
        internal string POPIMAP { get; private set; }
        internal bool SSL { get; private set; }

        internal string MailSubject { get; private set; }
        internal string MailSender { get; private set; }
        
        internal bool AnalyseAttachment { get; private set; }
        internal string AttachmentName { get; private set; }
        internal string ParserAlias { get; private set; }
        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MailConfiguration" /> class.
        /// </summary>
        public MailConfiguration()
        {
            ServerName = SettingsManager.Instance.GetSetting("MailAlarmSource", "ServerName").GetString();
            Port = (ushort) SettingsManager.Instance.GetSetting("MailAlarmSource", "Port").GetInt32();
            UserName = SettingsManager.Instance.GetSetting("MailAlarmSource", "UserName").GetString();
            Password = SettingsManager.Instance.GetSetting("MailAlarmSource", "Password").GetString();
            PollInterval = SettingsManager.Instance.GetSetting("MailAlarmSource", "PollInterval").GetInt32();
            POPIMAP = SettingsManager.Instance.GetSetting("MailAlarmSource", "POPIMAP").GetString();
            SSL = SettingsManager.Instance.GetSetting("MailAlarmSource", "SSL").GetBoolean();

            MailSubject = SettingsManager.Instance.GetSetting("MailAlarmSource", "MailSubject").GetString();
            MailSender = SettingsManager.Instance.GetSetting("MailAlarmSource", "MailSender").GetString();

            AnalyseAttachment = SettingsManager.Instance.GetSetting("MailAlarmSource", "AnalyseAttachment").GetBoolean();
            AttachmentName = SettingsManager.Instance.GetSetting("MailAlarmSource", "AttachmentName").GetString();
            ParserAlias = SettingsManager.Instance.GetSetting("MailAlarmSource", "MailParser").GetString();
        }

        #endregion
    }
}