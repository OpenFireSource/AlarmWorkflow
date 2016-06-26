using System;
using System.Collections.Generic;
using System.Linq;

namespace AlarmWorkflow.Shared.Diagnostics
{
    /// <summary>
    /// Writes Exception Loggs at a predetermined interval.
    /// </summary>
    public class ExceptionIntervalLogger
    {
        #region Constructors

        /// <summary>
        /// Initializes the Exception Logger.
        /// </summary>
        /// <param name="logInterval">the interval how often a Exception should be logged</param>
        public ExceptionIntervalLogger(TimeSpan logInterval)
        {
            _logInterval = logInterval;
            _exceptionEntries = new List<TimedException>();
        }

        #endregion

        #region Private Sublclasses

        private class TimedException
        {
            public TimedException(Exception exception)
            {
                Exception = exception;
                LastLogTime = DateTime.UtcNow;
            }

            public Exception Exception { get; }
            public DateTime LastLogTime { get; set; }
        }

        #endregion

        #region Fields

        private readonly List<TimedException> _exceptionEntries;
        private readonly TimeSpan _logInterval;

        #endregion

        #region Methods

        /// <summary>
        /// Log a exception. The Logger decides to log or discard the exception.
        /// </summary>
        /// <param name="source">The component from which this type comes. The type name of the instance is used.</param>
        /// <param name="ex">The exception.</param>
        public void LogException(object source, Exception ex)
        {
            if (ShouldLogException(ex))
            {
                Logger.Instance.LogException(source, ex);
            }
        }

        private bool ShouldLogException(Exception ex)
        {
            if (_exceptionEntries.Any(e => e.Exception.StackTrace == ex.StackTrace) &&
                _exceptionEntries.Any(e => e.Exception.Message == ex.Message))
            {
                var exception = _exceptionEntries.Single(e => e.Exception.StackTrace == ex.StackTrace && e.Exception.Message == ex.Message);
                if (exception.LastLogTime.Add(_logInterval) > DateTime.UtcNow) return false;
                exception.LastLogTime = DateTime.UtcNow;
            }
            else
            {
                _exceptionEntries.Add(new TimedException(ex));
            }
            return true;
        }

        /// <summary>
        /// Ability to log the exception as formatted text.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <param name="type">The message type.</param>
        /// <param name="source">The component from which this type comes. The type name of the instance is used.</param>
        /// <param name="format">The text to use as the format string.</param>
        /// <param name="arguments">The arguments to use for the format string.</param>
        public void LogFormat(Exception ex, LogType type, object source, string format, params object[] arguments)
        {
            if (ShouldLogException(ex))
            {
                Logger.Instance.LogFormat(type, source, format, arguments);
            }
        }

        /// <summary>
        /// Reset the stored exception entries.
        /// </summary>
        public void ResetExceptionCollection()
        {
            _exceptionEntries.Clear();
        }

        #endregion
    }
}
