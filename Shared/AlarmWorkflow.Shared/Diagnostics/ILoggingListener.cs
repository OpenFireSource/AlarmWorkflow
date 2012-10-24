#region Using Statements
#endregion

namespace AlarmWorkflow.Shared.Diagnostics
{
    /// <summary>
    /// Defines the mechanisms for any listener that is used by the <see cref="T:Logger"/> interface.
    /// </summary>
    public interface ILoggingListener
    {
        /// <summary>
        /// Writes the given <see cref="LogEntry"/> and performs implementation-specific actions according to it.
        /// </summary>
        /// <param name="entry">The log entry to write.</param>
        void Write(LogEntry entry);
        /// <summary>
        /// Called when the parental <see cref="T:Logger"/>-instance is shutting down.
        /// </summary>
        void Shutdown();
    }
}
