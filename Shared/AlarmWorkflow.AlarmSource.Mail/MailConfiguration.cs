using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.AlarmSource.Mail
{
    /// <summary>
    /// Represents the current configuration. Wraps the SettingsManager-calls.
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

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MailConfiguration"/> class.
        /// </summary>
        public MailConfiguration()
        {
            this.ServerName = SettingsManager.Instance.GetSetting("MailAlarmSource", "ServerName").GetString();
            this.Port = (ushort)SettingsManager.Instance.GetSetting("MailAlarmSource", "Port").GetInt32();
            this.UserName = SettingsManager.Instance.GetSetting("MailAlarmSource", "UserName").GetString();
            this.Password = SettingsManager.Instance.GetSetting("MailAlarmSource", "Password").GetString();
            this.PollInterval = SettingsManager.Instance.GetSetting("MailAlarmSource", "PollInterval").GetInt32();
            this.POPIMAP = SettingsManager.Instance.GetSetting("MailAlarmSource", "POPIMAP").GetString();
            this.SSL = SettingsManager.Instance.GetSetting("MailAlarmSource", "SSL").GetBoolean();

            this.MailSubject = SettingsManager.Instance.GetSetting("MailAlarmSource", "MailSubject").GetString();
            this.MailSender = SettingsManager.Instance.GetSetting("MailAlarmSource", "MailSender").GetString();
        }

        #endregion
    }
}
