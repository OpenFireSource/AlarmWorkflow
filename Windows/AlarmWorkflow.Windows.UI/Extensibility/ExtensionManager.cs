// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

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
        private List<IIdleUIJob> _idleUiJobs;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionManager"/> class.
        /// </summary>
        public ExtensionManager()
        {
            _uiJobs = new List<IUIJob>();
            _idleUiJobs = new List<IIdleUIJob>();
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
            foreach (var export in ExportedTypeLibrary.GetExports(typeof(IIdleUIJob)).Where(j => App.GetApp().Configuration.EnabledIdleJobs.Contains(j.Attribute.Alias)))
            {
                IIdleUIJob job = export.CreateInstance<IIdleUIJob>();

                string jobName = job.GetType().Name;
                Logger.Instance.LogFormat(LogType.Info, this, Resources.JobInitializeBegin, jobName);

                try
                {
                    if (!job.Initialize())
                    {
                        Logger.Instance.LogFormat(LogType.Warning, this, Resources.JobInitializeError, jobName);
                        continue;
                    }
                    _idleUiJobs.Add(job);

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
        /// Calls each <see cref="IIdleUIJob"/>.
        /// </summary>
        public void RunIdleUIJobs()
        {
            foreach (IIdleUIJob job in _idleUiJobs)
            {
                if (job.IsAsync)
                {
                    RunIdleUIJobAsync(job);
                }
                else
                {
                    RunIdleUIJobSync(job);
                }
            }
        }

        private void RunIdleUIJobAsync(IIdleUIJob job)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                // Run the job. If the job fails, ignore that exception as well but log it too!
                try
                {
                    job.Run();
                }
                catch (Exception ex)
                {
                    // Be careful when processing the jobs, we don't want a malicious job to terminate the process!
                    Logger.Instance.LogFormat(LogType.Warning, this, string.Format("An error occurred while processing the asynchronous UI-job '{0}'!", job.GetType().Name));
                    Logger.Instance.LogException(this, ex);
                }
            });
        }

        private void RunIdleUIJobSync(IIdleUIJob job)
        {
            // Run the job. If the job fails, ignore that exception as well but log it too!
            try
            {
                job.Run();
            }
            catch (Exception ex)
            {
                // Be careful when processing the jobs, we don't want a malicious job to terminate the process!
                Logger.Instance.LogFormat(LogType.Warning, this, string.Format("An error occurred while processing UI-job '{0}'!", job.GetType().Name));
                Logger.Instance.LogException(this, ex);
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