using System;
using System.IO;
using System.Xml;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.AlarmSource.Sms
{
    [Export("SmsAlarmSource", typeof(IAlarmSource))]
    class SmsAlarmSource : IAlarmSource
    {
        #region Fields

        private AlarmServer _server;
        private IParser _parser;

        #endregion

        #region Methods

        internal void PushIncomingAlarm(string alarmText)
        {
            if (_parser == null)
            {
                return;
            }

            string message = "";

            using (XmlReader reader = XmlReader.Create(new StringReader(alarmText)))
            {
                reader.ReadToFollowing("message");
                message = reader.ReadElementContentAsString();
            }

            alarmText = AlarmWorkflowConfiguration.Instance.ReplaceDictionary.ReplaceInString(alarmText);

            Operation operation = null;
            // Let the parser do the job...
            try
            {
                operation = _parser.Parse(new[]{alarmText});
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.SmsParserError);
                Logger.Instance.LogException(this, ex);
            }

            if (operation == null)
            {
                return;
            }

            OnNewAlarm(new AlarmSourceEventArgs(operation));
        }

        #endregion

        #region IAlarmSource Members

        void IAlarmSource.Initialize()
        {
            string smsParserAlias = SettingsManager.Instance.GetSetting("SmsAlarmSource", "SMSParser").GetString();

            _parser = ExportedTypeLibrary.Import<IParser>(smsParserAlias);
            if (_parser == null)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.SmsParserNotFoundError, smsParserAlias);
                _parser = new DefaultSMSParser();
            }
        }

        /// <summary>
        /// Raised when a new alarm has surfaced and processed for the Engine to handle.
        /// See documentation for further information.
        /// </summary>
        public event EventHandler<AlarmSourceEventArgs> NewAlarm;

        private void OnNewAlarm(AlarmSourceEventArgs e)
        {
            var copy = NewAlarm;
            if (copy != null)
            {
                copy(this, e);
            }
        }

        void IAlarmSource.RunThread()
        {
            _server = new AlarmServer(this);
            _server.Start();
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            _server.Stop();
        }

        #endregion

    }
}
