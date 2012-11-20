using System;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.AlarmSource.Sms
{
    [Export("SmsAlarmSource", typeof(IAlarmSource))]
    class SmsAlarmSource : IAlarmSource
    {
        #region IAlarmSource Members

        void IAlarmSource.Initialize()
        {
            // TODO: Initialize work (if any)
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
            // TODO: Listen to sockets here or whatever
            // TODO: Call "OnNewAlarm()" when a SMS has been received!
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            // TODO: Stop listening on sockets etc.
        }

        #endregion
    }
}
