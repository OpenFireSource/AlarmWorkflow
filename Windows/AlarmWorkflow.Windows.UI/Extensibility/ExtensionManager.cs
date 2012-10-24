using System;
using System.Collections.Generic;
using System.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.UI.Contracts.Extensibility;

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
                Logger.Instance.LogFormat(LogType.Info, this, "Initializing UI-job type '{0}'...", jobName);

                try
                {
                    if (!job.Initialize())
                    {
                        Logger.Instance.LogFormat(LogType.Warning, this, "UI-Job type '{0}' initialization failed. The UI-job will not be executed.", jobName);
                        continue;
                    }
                    _uiJobs.Add(job);

                    Logger.Instance.LogFormat(LogType.Info, this, "UI-Job type '{0}' initialization successful.", jobName);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, this, "An error occurred while initializing UI-job type '{0}'. The error message was: {1}", jobName, ex.Message);
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
        }

        #endregion
    }
}
