using System;
using System.Diagnostics;

namespace AlarmWorkflow.Shared.Logging
{
    /// <summary>
    /// The EventLogLogger class log all events to the windows event log.
    /// </summary>
    public class EventLogLogger : ILogger
    {
        /// <summary>
        /// The Eventlog, in which the logger logs.
        /// </summary>
        private EventLog eventLog1 = new EventLog("Application", ".");

        /// <summary>
        /// Gets/sets whether or not the Logger is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Inherited by iLogger abstract class. Initializes the logger.
        /// </summary>
        /// <returns>False when an error occured, otherwise true.</returns>
        public bool Initialize()
        {
            if (IsEnabled)
            {
                try
                {
                    if (!System.Diagnostics.EventLog.SourceExists("AlarmWorkflow"))
                    {
                        System.Diagnostics.EventLog.CreateEventSource("AlarmWorkflow", "Application");
                    }

                    this.eventLog1.Source = "AlarmWorkflow";
                }
                catch (Exception)
                {
                    IsEnabled = false;
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Inherited by iLogger abstract class. Write some information to the Log.
        /// </summary>
        /// <param name="info">The information which will be loged.</param>
        public void WriteInformation(string info)
        {
            if (IsEnabled)
            {
                this.eventLog1.WriteEntry(info, EventLogEntryType.Information);
            }
        }

        /// <summary>
        /// Inherited by iLogger abstract class. Write some warning to the Log.
        /// </summary>
        /// <param name="warning">The warning which will be loged.</param>
        public void WriteWarning(string warning)
        {
            if (IsEnabled)
            {
                this.eventLog1.WriteEntry(warning, EventLogEntryType.Warning);
            }
        }

        /// <summary>
        /// Inherited by iLogger abstract class. Write some error to the Log.
        /// </summary>
        /// <param name="errorMessage">The error which will be loged.</param>
        public void WriteError(string errorMessage)
        {
            if (IsEnabled)
            {
                this.eventLog1.WriteEntry(errorMessage, EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Inherited by IDisposable interface.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Clean the object.
        /// </summary>
        /// <param name="alsoManaged">Indicates if also managed code shoud be cleaned up.</param>
        protected virtual void Dispose(bool alsoManaged)
        {
            if (alsoManaged == true)
            {
                this.eventLog1.Dispose();
                this.eventLog1 = null;
            }
        }
    }
}
