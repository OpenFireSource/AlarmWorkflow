using System;
using System.Collections.Generic;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Extensibility
{
    /// <summary>
    /// Event arguments for <see cref="IAlarmSource"/>.
    /// </summary>
    public sealed class AlarmSourceEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="Operation"/>-instance that resulted from the new alarm.
        /// </summary>
        public Operation Operation { get; private set; }
        /// <summary>
        /// Gets/sets the parameters for this event args.
        /// Used within the context and contains data for jobs.
        /// </summary>
        public IDictionary<string, object> Parameters { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmSourceEventArgs"/> class.
        /// </summary>
        /// <param name="operation">The <see cref="Operation"/>-instance that resulted from the new alarm.</param>
        public AlarmSourceEventArgs(Operation operation)
            : base()
        {
            this.Operation = operation;
        }

        #endregion
    }
}
