using System;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Diagnostics
{
    /// <summary>
    /// Provides an <see cref="ILoggingListener"/>-implementation that calls a method when it writes something.
    /// </summary>
    public class RelayLoggingListener : ILoggingListener
    {
        #region Properties

        /// <summary>
        /// Gets the delegate method that is called when the "Write()" method was called.
        /// </summary>
        public Action<LogEntry> WriteAction { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the CRelayListener class.
        /// </summary>
        protected RelayLoggingListener()
        {

        }

        /// <summary>
        /// Initializes a new instance of the CRelayListener class.
        /// </summary>
        /// <param name="writeAction">The delegate method that is called when the "Write()" method was called.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="writeAction"/> was <c>null</c>.</exception>
        public RelayLoggingListener(Action<LogEntry> writeAction)
            : this()
        {
            Assertions.AssertNotNull(writeAction, "writeAction");

            this.WriteAction = writeAction;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Writes the given <see cref="LogEntry"/> and performs implementation-specific actions according to it.
        /// </summary>
        /// <param name="entry">The log entry to write.</param>
        protected virtual void Write(LogEntry entry)
        {
            WriteAction(entry);
        }

        /// <summary>
        /// Called when the parental <see cref="T:Logger"/>-instance is shutting down.
        /// </summary>
        protected virtual void Shutdown()
        {

        }

        #endregion

        #region ILoggingListener Member

        void ILoggingListener.Write(LogEntry entry)
        {
            this.Write(entry);
        }

        void ILoggingListener.Shutdown()
        {
            this.Shutdown();
        }

        #endregion
    }
}
