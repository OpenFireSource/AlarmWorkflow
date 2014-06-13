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