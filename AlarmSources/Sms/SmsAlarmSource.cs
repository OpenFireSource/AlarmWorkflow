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
using System.Xml;
using AlarmWorkflow.AlarmSource.Sms.Server;
using AlarmWorkflow.BackendService.EngineContracts;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Specialized;

namespace AlarmWorkflow.AlarmSource.Sms
{
    [Export("SmsAlarmSource", typeof(IAlarmSource))]
    [Information(DisplayName = "ExportAlarmSourceDisplayName", Description = "ExportAlarmSourceDescription")]
    class SmsAlarmSource : IAlarmSource
    {
        #region Fields

        private ISettingsServiceInternal _settings;

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

            alarmText = _settings.GetSetting(SettingKeys.ReplaceDictionary).GetValue<ReplaceDictionary>().ReplaceInString(alarmText);

            Operation operation = null;

            try
            {
                operation = _parser.Parse(new[] { alarmText });
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

        void IAlarmSource.Initialize(IServiceProvider serviceProvider)
        {
            _settings = serviceProvider.GetService<ISettingsServiceInternal>();

            string smsParserAlias = _settings.GetSetting(SmsSettingKeys.SmsParser).GetValue<string>();

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
            _settings = null;
        }

        #endregion
    }
}