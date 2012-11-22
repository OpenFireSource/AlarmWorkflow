using System;
using System.IO;
using System.Xml;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.AlarmSource.Sms
{
    [Export("SmsAlarmSource", typeof(IAlarmSource))]
    class SmsAlarmSource : IAlarmSource
    {
        #region Fields

        private AlarmServer _server;

        #endregion

        #region Methods

        internal void PushIncomingAlarm(string alarmText)
        {
            string message = "";

            using (XmlReader reader = XmlReader.Create(new StringReader(alarmText)))
            {
                reader.ReadToFollowing("message");
                message = reader.ReadElementContentAsString();
            }

            // TODO: There is a need for a parser! Right now the Operation-object is pretty useless...
            Operation operation = new Operation();
            operation.Timestamp = DateTime.UtcNow;
            operation.Comment = message;

            OnNewAlarm(new AlarmSourceEventArgs(operation));
        }

        #endregion

        #region IAlarmSource Members

        void IAlarmSource.Initialize()
        {
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
