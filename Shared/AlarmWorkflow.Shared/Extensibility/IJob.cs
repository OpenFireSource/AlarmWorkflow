using System.Xml.XPath;
using AlarmWorkflow.Shared.Core;

// TODO: Support Shutdown() method!

namespace AlarmWorkflow.Shared.Extensibility
{
    /// <summary>
    /// Defines a means for a job, which is a task that runs on every new Operation.
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Called to initialize the job when it gets added to the job queue that runs on each processed alarm.
        /// </summary>
        /// <returns>Whether or not initialization was successful.</returns>
        bool Initialize();
        /// <summary>
        /// Executes the job for the given operation.
        /// </summary>
        /// <param name="operation">Current operation.</param>
        void DoJob(Operation operation);
    }
}
