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
        /// Called to initialize the job when it gets added to the job queue that runs on each processed fax.
        /// </summary>
        /// <returns>Whether or not initialization was successful.</returns>
        bool Initialize();
        /// <summary>
        /// This methode do the jobs job.
        /// </summary>
        /// <param name="operation">Current operation.</param>
        void DoJob(Operation operation);
    }
}
