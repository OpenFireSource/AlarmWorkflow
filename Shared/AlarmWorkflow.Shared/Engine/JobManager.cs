using System;
using System.Collections.Generic;
using System.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;
using System.Threading;

namespace AlarmWorkflow.Shared.Engine
{
    /// <summary>
    /// Wrapper class to manage all <see cref="IJob"/>s that are used by the engine.
    /// </summary>
    sealed class JobManager : DisposableObject
    {
        #region Fields

        private List<IJob> _jobs;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JobManager"/> class.
        /// </summary>
        public JobManager()
        {
            _jobs = new List<IJob>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes this instance for usage.
        /// </summary>
        public void Initialize()
        {
            AssertNotDisposed();

            InitializeJobs();
        }

        private void InitializeJobs()
        {
            foreach (var export in ExportedTypeLibrary.GetExports(typeof(IJob)).Where(j => AlarmWorkflowConfiguration.Instance.EnabledJobs.Contains(j.Attribute.Alias)))
            {
                IJob job = export.CreateInstance<IJob>();

                string jobName = job.GetType().Name;
                Logger.Instance.LogFormat(LogType.Info, this, "Initializing job type '{0}'...", jobName);

                try
                {
                    if (!job.Initialize())
                    {
                        Logger.Instance.LogFormat(LogType.Warning, this, "Job type '{0}' initialization failed. The job will not be executed.", jobName);
                        continue;
                    }
                    _jobs.Add(job);

                    Logger.Instance.LogFormat(LogType.Info, this, "Job type '{0}' initialization successful.", jobName);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, this, "An error occurred while initializing job type '{0}'. The error message was: {1}", jobName, ex.Message);
                }
            }
        }

        /// <summary>
        /// Executes the jobs that are registered at this job manager.
        /// </summary>
        /// <param name="context">The context  on which basis the jobs are executed.</param>
        /// <param name="operation">The operation on which basis the jobs are executed.</param>
        public void ExecuteJobs(IJobContext context, Operation operation)
        {
            AssertNotDisposed();

            foreach (IJob job in _jobs)
            {
                if (job.IsAsync)
                {
                    Logger.Instance.LogFormat(LogType.Info, this, "Executing asnyc job " + job.GetType().Name);
                }
                else
                {
                    Logger.Instance.LogFormat(LogType.Info, this, "Executing asnyc job " + job.GetType().Name);
                }
                RunJob(context, operation, job);
            }
        }

        private void RunJob(IJobContext context, Operation operation, IJob job)
        {
            if (job.IsAsync)
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    RunJobCore(context, operation, job);
                });
            }
            else
            {
                RunJobCore(context, operation, job);
            }
        }

        private void RunJobCore(IJobContext context, Operation operation, IJob job)
        {
            // Run the job. If the job fails, ignore that exception as well but log it too!
            try
            {
                job.Execute(context, operation);
            }
            catch (Exception ex)
            {
                string message = (job.IsAsync) ? Properties.Resources.JobExecuteAsyncFailed : Properties.Resources.JobExecuteSyncFailed;

                // Be careful when processing the jobs, we don't want a malicious job to terminate the process!
                Logger.Instance.LogFormat(LogType.Warning, this, string.Format(message, job.GetType().Name));
                Logger.Instance.LogException(this, ex);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected override void DisposeCore()
        {
            foreach (IJob job in _jobs)
            {
                try
                {
                    job.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.JobManagerDisposeJobFailed, job.GetType().Name);
                    Logger.Instance.LogException(this, ex);
                }
            }

            _jobs.Clear();
        }

        #endregion
    }
}
