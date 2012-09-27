using System;
using System.Diagnostics;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Logging
{
    /// <summary>
    /// The EventLogLogger class log all events to the windows event log.
    /// </summary>
    [Export("EventLog", typeof(ILogger))]
    sealed class EventLogLogger : ILogger
    {
        #region Fields

        private EventLog _eventLog;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogLogger"/> class.
        /// </summary>
        public EventLogLogger()
        {
            _eventLog = new EventLog("Application", ".");
        }

        #endregion

        #region ILogger Members

        bool ILogger.Initialize()
        {
            try
            {
                if (!System.Diagnostics.EventLog.SourceExists("AlarmWorkflow"))
                {
                    System.Diagnostics.EventLog.CreateEventSource("AlarmWorkflow", "Application");
                }

                _eventLog.Source = "AlarmWorkflow";
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        void ILogger.WriteInformation(string info)
        {
            _eventLog.WriteEntry(info, EventLogEntryType.Information);
        }

        void ILogger.WriteWarning(string warning)
        {
            _eventLog.WriteEntry(warning, EventLogEntryType.Warning);
        }

        void ILogger.WriteError(string errorMessage)
        {
            _eventLog.WriteEntry(errorMessage, EventLogEntryType.Error);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool alsoManaged)
        {
            if (alsoManaged)
            {
                _eventLog.Dispose();
                _eventLog = null;
            }
        }

        #endregion
    }
}
