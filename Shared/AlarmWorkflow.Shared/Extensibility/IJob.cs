using System;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Engine;

namespace AlarmWorkflow.Shared.Extensibility
{
    /// <summary>
    /// Defines a means for a job, which is a task that runs on every new Operation.
    /// </summary>
    public interface IJob : IDisposable
    {
        /// <summary>
        /// Returns whether or not this job is executed asynchronously by the engine.
        /// This should be set to true for longer-running tasks.
        /// </summary>
        bool IsAsync { get; }

        /// <summary>
        /// Called to initialize the job when it gets added to the job queue that runs on each processed alarm.
        /// </summary>
        /// <returns>Whether or not initialization was successful.</returns>
        bool Initialize();
        /// <summary>
        /// Executes the job for the given context and operation.
        /// </summary>
        /// <param name="context">The context in which the current job runs.</param>
        /// <param name="operation">Current operation.</param>
        void Execute(IJobContext context, Operation operation);
    }
}
