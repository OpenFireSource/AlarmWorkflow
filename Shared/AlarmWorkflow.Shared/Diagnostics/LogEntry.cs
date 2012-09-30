using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace AlarmWorkflow.Shared.Diagnostics
{
    /// <summary>
    /// Provides one single log entry.
    /// </summary>
    [DebuggerDisplay("Id = {Id}, Source = {Source}, Message = {Message}")]
    public sealed class LogEntry
    {
        #region Constants

        private static readonly char SeparatorChar = ';';
        private static readonly char SeparatorReplacementChar = ',';
        private static readonly string FormatCSV = "[{0}];[{1}];[{2}];[{3}];[{4}]";

        #endregion

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

        #region Methods

        /// <summary>
        /// Translates the <see cref="LogType"/> value into the correct three-character prefix that is used in the log files.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <returns>The correct three-character prefix that is used in the log files. This prefix comes with a tab appended to its end.</returns>
        private string GetPrefix(LogType messageType)
        {
            // create temp string
            string prefix = "INF";

            switch (messageType)
            {
                case LogType.Debug: prefix = "DBG"; break;
                case LogType.Trace: { prefix = "TRC"; break; }
                case LogType.Info: { prefix = "INF"; break; }
                case LogType.Warning: { prefix = "WRN"; break; }
                case LogType.Error: { prefix = "ERR"; break; }
                case LogType.Exception: { prefix = "EXC"; break; }
            }

            return prefix;
        }

        private LogType GetMessageType(string prefix)
        {
            switch (prefix)
            {
                case "DBG": return LogType.Debug;
                case "TRC": return LogType.Trace;
                case "INF": return LogType.Info;
                case "WRN": return LogType.Warning;
                case "ERR": return LogType.Error;
                case "EXC": return LogType.Exception;
                default: return LogType.None;
            }
        }

        /// <summary>
        /// Returns this log entry formatted to be used in the log file.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>This method will replace all semicolons within the <see cref="P:Source"/> and <see cref="P:Message"/> strings with a comma,
        /// since the semicolon is used as a CSV-specific separator.</remarks>
        /// <returns>This log entry formatted to be used in the log file.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, FormatCSV,
                GetPrefix(MessageType),
                Timestamp,
                Source.Replace(SeparatorChar, SeparatorReplacementChar),
                Message.Replace(SeparatorChar, SeparatorReplacementChar),
                GetExceptionText());
        }

        private string GetExceptionText()
        {
            if (Exception == null)
            {
                return "";
            }

            return string.Format("Type = {0}; Message = {1}; Stack Trace = {2}", Exception.GetType().Name, Exception.Message, Exception.StackTrace.Replace(Environment.NewLine, "/r/"));
        }

        #endregion

        #region Factory methods

        /// <summary>
        /// Creates the log entry from its string representation.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static LogEntry CreateFromString(string value)
        {
            LogEntry entry = new LogEntry();
            string[] tokens = value.Split(';');
            if (tokens.Length != 4)
            {
                // malformed
                return null;
            }
            entry.MessageType = entry.GetMessageType(tokens[0]);
            entry.Timestamp = DateTime.Parse(tokens[1], CultureInfo.InvariantCulture);
            entry.Source = tokens[2];
            entry.Message = tokens[3];

            return entry;
        }

        #endregion

    }
}
