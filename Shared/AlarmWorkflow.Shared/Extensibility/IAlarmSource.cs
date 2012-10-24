using System;

namespace AlarmWorkflow.Shared.Extensibility
{
    /// <summary>
    /// Defines a means for a type that acts as a source of Operations, that is a type that processes alarms, such as a Fax, SMS or E-Mail.
    /// </summary>
    public interface IAlarmSource : IDisposable
    {
        /// <summary>
        /// Raised when a new alarm has surfaced and processed for the Engine to handle.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>This event handler is only registered by the Engine if Initialization was successful.</remarks>
        event EventHandler<AlarmSourceEventArgs> NewAlarm;

        /// <summary>
        /// Initializes this alarm source prior to its first use.
        /// </summary>
        void Initialize();
        /// <summary>
        /// Called by the Engine when initialization has succeeded. This method call is run within its own thread.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>The thread that contains this method is run when the Engine starts, and is aborted when the Engine stops.
        /// Exceptions that are thrown in this method are logged and handled.</remarks>
        void RunThread();
    }
}
