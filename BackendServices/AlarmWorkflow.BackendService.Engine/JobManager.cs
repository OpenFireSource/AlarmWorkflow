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
using AlarmWorkflow.BackendService.Engine.Properties;
using AlarmWorkflow.BackendService.EngineContracts;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.BackendService.Engine
{
    /// <summary>
    /// Wrapper class to manage all <see cref="IJob"/>s that are used by the engine.
    /// </summary>
    sealed class JobManager : DisposableObject
    {
        #region Fields

        private IEnumerable<string> _enabledJobs;

        private List<IJob> _jobs;
        private IServiceProvider _serviceProvider;
        private ISettingsServiceInternal _settingsService;

        #endregion

        #region Constructors

        private JobManager()
        {
            _jobs = new List<IJob>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JobManager"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider used for retrieving server-side services.</param>
        public JobManager(IServiceProvider serviceProvider)
            : this()
        {
            Assertions.AssertNotNull(serviceProvider, "serviceProvider");

            _serviceProvider = serviceProvider;

            _settingsService = _serviceProvider.GetService<ISettingsServiceInternal>();
            _settingsService.SettingChanged += SettingsService_OnSettingChanged;
        }

        #endregion

        #region Methods

        private IEnumerable<string> RetrieveEnabledJobs()
        {
            return _settingsService.GetSetting(SettingKeys.JobsConfigurationKey).GetValue<ExportConfiguration>().GetEnabledExports();
        }

        /// <summary>
        /// Initializes this instance for usage and sets up all configured jobs.
        /// </summary>
        public void Initialize()
        {
            _enabledJobs = RetrieveEnabledJobs();

            InitializeAndAddJobs(_enabledJobs);
        }

        private void InitializeAndAddJobs(IEnumerable<string> jobsToInitialize)
        {
            AssertNotDisposed();

            foreach (var export in ExportedTypeLibrary.GetExports(typeof(IJob)).Where(j => jobsToInitialize.Contains(j.Attribute.Alias)))
            {
                IJob job = export.CreateInstance<IJob>();
                if (InitializeJob(job))
                {
                    _jobs.Add(job);
                }
            }
        }

        private bool InitializeJob(IJob job)
        {
            string jobName = job.GetType().Name;
            Logger.Instance.LogFormat(LogType.Info, this, Resources.JobInitializeBegin, jobName);

            try
            {
                if (!job.Initialize(_serviceProvider))
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, Resources.JobInitializeError, jobName);
                    return false;
                }

                Logger.Instance.LogFormat(LogType.Info, this, Resources.JobInitializeSuccess, jobName);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Resources.JobGenericError, jobName, ex.Message);
                Logger.Instance.LogException(this, ex);
                return false;
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
                RunJob(context, operation, job);
            }
        }

        private void RunJob(IJobContext context, Operation operation, IJob job)
        {
            if (job.IsAsync)
            {
                Logger.Instance.LogFormat(LogType.Info, this, Resources.JobExecuteAsyncStart, job.GetType().Name, context.Phase);
                ThreadPool.QueueUserWorkItem(o => RunJobCore(context, operation, job));
            }
            else
            {
                Logger.Instance.LogFormat(LogType.Info, this, Resources.JobExecuteSyncStart, job.GetType().Name, context.Phase);
                RunJobCore(context, operation, job);
            }
        }

        private void RunJobCore(IJobContext context, Operation operation, IJob job)
        {
            try
            {
                job.Execute(context, operation);

                Logger.Instance.LogFormat(LogType.Trace, this, Resources.JobExecuteFinished, job.GetType().Name);
            }
            catch (Exception ex)
            {
                string message = (job.IsAsync) ? Properties.Resources.JobExecuteAsyncFailed : Properties.Resources.JobExecuteSyncFailed;

                Logger.Instance.LogFormat(LogType.Warning, this, string.Format(message, job.GetType().Name));
                Logger.Instance.LogException(this, ex);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected override void DisposeCore()
        {
            _settingsService.SettingChanged -= SettingsService_OnSettingChanged;

            DisposeJobs(_enabledJobs, false);
        }

        private void DisposeJobs(IEnumerable<string> jobsToDispose, bool isDueToSettingsChange)
        {
            AssertNotDisposed();

            foreach (IJob job in _jobs.Where(item => jobsToDispose.Contains(ExportedTypeLibrary.GetExportAlias(item.GetType()))).ToList())
            {
                if (isDueToSettingsChange)
                {
                    Logger.Instance.LogFormat(LogType.Trace, this, Resources.SettingsJobDisabled, job.GetType().Name);
                }

                try
                {
                    job.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, Resources.JobManagerDisposeJobFailed, job.GetType().Name);
                    Logger.Instance.LogException(this, ex);
                }
                finally
                {
                    _jobs.Remove(job);
                }
            }
        }

        private void SettingsService_OnSettingChanged(object sender, SettingChangedEventArgs settingChangedEventArgs)
        {
            if (settingChangedEventArgs.Keys.Contains(SettingKeys.JobsConfigurationKey))
            {
                IEnumerable<string> oldConfig = _enabledJobs;
                _enabledJobs = RetrieveEnabledJobs();

                IEnumerable<string> disabledJobs = oldConfig.Except(_enabledJobs);
                DisposeJobs(disabledJobs, true);

                IEnumerable<string> enabledJobs = _enabledJobs.Except(oldConfig);
                InitializeAndAddJobs(enabledJobs);
            }
        }

        #endregion
    }
}