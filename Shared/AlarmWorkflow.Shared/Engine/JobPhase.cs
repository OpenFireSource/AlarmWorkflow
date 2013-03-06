using System;

namespace AlarmWorkflow.Shared.Engine
{
    /// <summary>
    /// Specifies the available phases a job can run in.
    /// </summary>
    public enum JobPhase
    {
        /// <summary>
        /// Enumeration default value. Don't use.
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// Represents the phase after the operation has surfaced from the alarm source and is prior to being saved.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>Jobs in this phase can provide additional information to the operation,
        /// such as loop information or other external details.</remarks>
        OnOperationSurfaced = 1,
        /// <summary>
        /// Represents the phase after the operation has been stored by the operation store.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>This is the usual phase for most jobs.</remarks>
        AfterOperationStored = 2,
    }
}
