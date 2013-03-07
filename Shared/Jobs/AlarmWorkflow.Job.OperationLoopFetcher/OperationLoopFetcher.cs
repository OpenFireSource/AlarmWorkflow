using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Engine;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Job.OperationLoopFetcher
{
    /// <summary>
    /// Represents a job that runs right after an operation has surfaced. It has the intention to supply loop information to the operation so that it can be further used in the application.
    /// The loop information is gathered by various devices.
    /// </summary>
    [Export("OperationLoopFetcher", typeof(IJob))]
    class OperationLoopFetcher : IJob
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationLoopFetcher"/> class.
        /// </summary>
        public OperationLoopFetcher()
        {

        }

        #endregion

        #region IJob Members

        void IJob.Execute(IJobContext context, Operation operation)
        {
            // This is a Pre-Job. Thus it only has to run right after the operation has surfaced and before being stored.
            if (context.Phase != JobPhase.OnOperationSurfaced)
            {
                return;
            }
        }

        bool IJob.Initialize()
        {
            return true;
        }

        bool IJob.IsAsync
        {
            get { return false; }
        }

        #endregion

        #region IDisposable Members

        void System.IDisposable.Dispose()
        {

        }

        #endregion
    }
}
