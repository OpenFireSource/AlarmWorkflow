using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.UI.Properties;
using AlarmWorkflow.Windows.UIContracts.Extensibility;

namespace AlarmWorkflow.Windows.UI.Extensibility
{
    class ExtensionManager
    {
        #region Fields

        private List<IUIJob> _uiJobs;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionManager"/> class.
        /// </summary>
        public ExtensionManager()
        {
            _uiJobs = new List<IUIJob>();
            InitializeJobs();
        }

        #endregion

        #region Methods

        private void InitializeJobs()
        {
            foreach (var export in ExportedTypeLibrary.GetExports(typeof(IUIJob)).Where(j => App.GetApp().Configuration.EnabledJobs.Contains(j.Attribute.Alias)))
            {
                IUIJob job = export.CreateInstance<IUIJob>();

                string jobName = job.GetType().Name;
                Logger.Instance.LogFormat(LogType.Info, this, Resources.JobInitializeBegin, jobName);

                try
                {
                    if (!job.Initialize())
                    {
                        Logger.Instance.LogFormat(LogType.Warning, this, Resources.JobInitializeError, jobName);
                        continue;
                    }
                    _uiJobs.Add(job);

                    Logger.Instance.LogFormat(LogType.Info, this, Resources.JobInitializeSuccess, jobName);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, this, Resources.JobGenericError, jobName, ex.Message);
                    Logger.Instance.LogException(this, ex);
                }
            }
        }

        /// <summary>
        /// Calls each <see cref="IUIJob"/> using the given <see cref="IOperationViewer"/> and <see cref="Operation"/> instances.
        /// </summary>
        /// <param name="operationViewer"></param>
        /// <param name="operation"></param>
        public void RunUIJobs(IOperationViewer operationViewer, Operation operation)
        {
            foreach (IUIJob job in _uiJobs)
            {
                if (job.IsAsync)
                {
                    RunUIJobAsync(operationViewer, operation, job);
                }
                else
                {
                    RunUIJobSync(operationViewer, operation, job);
                }
            }
        }

        private void RunUIJobAsync(IOperationViewer operationViewer, Operation operation, IUIJob job)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                // Run the job. If the job fails, ignore that exception as well but log it too!
                try
                {
                    job.OnNewOperation(operationViewer, operation);
                }
                catch (Exception ex)
                {
                    // Be careful when processing the jobs, we don't want a malicious job to terminate the process!
                    Logger.Instance.LogFormat(LogType.Warning, this, string.Format("An error occurred while processing the asynchronous UI-job '{0}'!", job.GetType().Name));
                    Logger.Instance.LogException(this, ex);
                }
            });
        }

        private void RunUIJobSync(IOperationViewer operationViewer, Operation operation, IUIJob job)
        {
            // Run the job. If the job fails, ignore that exception as well but log it too!
            try
            {
                job.OnNewOperation(operationViewer, operation);
            }
            catch (Exception ex)
            {
                // Be careful when processing the jobs, we don't want a malicious job to terminate the process!
                Logger.Instance.LogFormat(LogType.Warning, this, string.Format("An error occurred while processing UI-job '{0}'!", job.GetType().Name));
                Logger.Instance.LogException(this, ex);
            }
        }

        #endregion
    }
}
