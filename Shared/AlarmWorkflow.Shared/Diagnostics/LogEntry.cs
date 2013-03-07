using System;
using System.Diagnostics;

namespace AlarmWorkflow.Shared.Diagnostics
{
    /// <summary>
    /// Provides one single log entry.
    /// </summary>
    [DebuggerDisplay("Id = {Id}, Source = {Source}, Message = {Message}")]
    public sealed class LogEntry
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="LogType"/>.
        /// </summary>
        public LogType MessageType { get; private set; }
        /// <summary>
        /// Gets the source that emitted the log entry.
        /// </summary>
        public string Source { get; private set; }
        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message { get; private set; }
        /// <summary>
        /// Gets the time this entry occured.
        /// </summary>
        public DateTime Timestamp { get; private set; }
        /// <summary>
        /// Gets/sets the exception, if this entry is an exception-entry.
        /// </summary>
        public Exception Exception { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor.
        /// </summary>
        private LogEntry()
        {
            MessageType = LogType.None;
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Initializes a new instance of the CLogEntry class.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="source">The source that has emitted this log entry.</param>
        /// <param name="message">The message.</param>
        public LogEntry(LogType messageType, string source, string message)
            : this(messageType, source, message, DateTime.Now)
        {

        }

        /// <summary>
        /// Initializes a new instance of the CLogEntry class.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="source">The source that has emitted this log entry.</param>
        /// <param name="message">The message.</param>
        /// <param name="timestamp">A custom <see cref="DateTime"/> defining the time stamp of this entry, if different.</param>
        public LogEntry(LogType messageType, string source, string message, DateTime timestamp)
            : this()
        {
            MessageType = messageType;
            Source = source;
            Message = message;
            Timestamp = timestamp;
        }

        #endregion
    }
}
