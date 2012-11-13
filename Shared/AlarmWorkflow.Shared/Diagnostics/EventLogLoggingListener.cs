using System;
using System.Diagnostics;

namespace AlarmWorkflow.Shared.Diagnostics
{
    /// <summary>
    /// Writes log messages to the Event Log. Using this listener requires administrator rights for the executing process.
    /// </summary>
    public sealed class EventLogLoggingListener : ILoggingListener
    {
        #region Constants

        /// <summary>
        /// Defines the default name of the event log source.
        /// </summary>
        public static readonly string DefaultEventLogSourceName = "AlarmWorkflow";

        #endregion

        #region Fields

        private EventLog _eventLog;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogLoggingListener"/> class
        /// and uses the default name for the event log source.
        /// </summary>
        public EventLogLoggingListener()
            : this(DefaultEventLogSourceName)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogLoggingListener"/> class.
        /// </summary>
        /// <param name="eventLogSourceName">Name of the event log source.</param>
        public EventLogLoggingListener(string eventLogSourceName)
        {
            try
            {
                if (!EventLog.SourceExists(eventLogSourceName))
                {
                    EventLog.CreateEventSource(eventLogSourceName, "Application");
                }
                _eventLog = new EventLog("Application", ".", eventLogSourceName);
            }
            catch (Exception)
            {
                // It's ok to ignore this.
                Trace.WriteLine("Could not connect to EventLog. Administrator rights are required for this.");
            }
        }

        #endregion

        #region ILoggingListener Members

        void ILoggingListener.Write(LogEntry entry)
        {
            if (_eventLog == null)
            {
                return;
            }

            EventLogEntryType type = EventLogEntryType.Information;
            switch (entry.MessageType)
            {
                case LogType.None:
                case LogType.Console:
                case LogType.Trace:
                case LogType.Debug:
                case LogType.Info:
                default:
                    type = EventLogEntryType.Information;
                    break;
                case LogType.Warning:
                    type = EventLogEntryType.Warning;
                    break;
                case LogType.Error:
                case LogType.Exception:
                    type = EventLogEntryType.Error;
                    break;
            }

            _eventLog.WriteEntry(entry.Message, type);
            // If the entry is an exception, write a separate entry for this case
            if (entry.Exception != null)
            {
                _eventLog.WriteEntry(entry.Exception.Message, EventLogEntryType.Error);
            }
        }

        void ILoggingListener.Shutdown()
        {
            if (_eventLog != null)
            {
                _eventLog.Dispose();
                _eventLog = null;
            }
        }

        #endregion
    }
}
