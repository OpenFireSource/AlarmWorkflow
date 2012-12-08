
namespace AlarmWorkflow.Shared.Diagnostics
{
    /// <summary>
    /// Represents a logging listener that logs to the Diagnostics debug and trace outs.
    /// </summary>
    public sealed class DiagnosticsLoggingListener : ILoggingListener
    {
        #region Fields

        private bool _isDebug;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the DiagnosticsLoggingListener class.
        /// </summary>
        public DiagnosticsLoggingListener()
        {

        }

        /// <summary>
        /// Initializes a new instance of the DiagnosticsLoggingListener class.
        /// </summary>
        /// <param name="isDebug">Whether or not we are in debug configuration mode.</param>
        public DiagnosticsLoggingListener(bool isDebug)
            : this()
        {
            _isDebug = isDebug;
        }

        #endregion

        #region ILoggingListener Member

        void ILoggingListener.Write(LogEntry entry)
        {
            if (_isDebug)
            {
                System.Diagnostics.Debug.WriteLine(entry.ToString());
            }
            else
            {
                System.Diagnostics.Trace.WriteLine(entry.ToString());
            }
        }

        void ILoggingListener.Shutdown()
        {

        }

        #endregion
    }
}
