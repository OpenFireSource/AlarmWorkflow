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
using AlarmWorkflow.BackendService.ManagementContracts;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.BackendService.Engine
{
    class AlarmWorkflowEngine : IDisposable
    {
        #region Constants

        private const string AlarmSourceThreadNameFormat = "AlarmWorkflow.Engine.Thread.${0}";

        #endregion

        #region Fields

        private bool _isStarted;
        private Configuration _configuration;

        private List<IAlarmSource> _alarmSources;
        private Dictionary<IAlarmSource, Thread> _alarmSourcesThreads;

        private JobManager _jobManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISettingsServiceInternal _settingsService;
        private readonly IOperationServiceInternal _operationService;

        #endregion

        #region Constructors

        private AlarmWorkflowEngine()
        {
            _alarmSources = new List<IAlarmSource>();
            _alarmSourcesThreads = new Dictionary<IAlarmSource, Thread>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmWorkflowEngine"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="serviceProvider"></param>
        /// <param name="settings"></param>
        public AlarmWorkflowEngine(Configuration configuration, IServiceProvider serviceProvider, ISettingsServiceInternal settings)
            : this()
        {
            Assertions.AssertNotNull(configuration, "configuration");
            Assertions.AssertNotNull(serviceProvider, "serviceProvider");
            Assertions.AssertNotNull(settings, "settings");

            _configuration = configuration;
            _serviceProvider = serviceProvider;

            _settingsService = settings;
            _operationService = serviceProvider.GetService<IOperationServiceInternal>();
        }

        #endregion

        #region Methods

        private void InitializeAlarmSources()
        {
            foreach (var export in ExportedTypeLibrary.GetExports(typeof(IAlarmSource)).Where(j => _configuration.EnabledAlarmSources.Contains(j.Attribute.Alias)))
            {
                Logger.Instance.LogFormat(LogType.Info, this, Resources.AlarmSourceEnabling, export.Type.Name);

                IAlarmSource alarmSource = export.CreateInstance<IAlarmSource>();
                _alarmSources.Add(alarmSource);

                Logger.Instance.LogFormat(LogType.Info, this, Resources.AlarmSourceEnabled, export.Type.Name);
            }
        }

        /// <summary>
        /// Starts the monitor thread, which is waiting for a new Alarm.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Service start failed because of one of the following reasons. - There are no running alarm sources. - A general exception occurred.</exception>
        internal void Start()
        {
            if (_isStarted)
            {
                return;
            }

            Logger.Instance.LogFormat(LogType.Info, this, Resources.EngineStarting);

            InitializeAlarmSources();

            _jobManager = new JobManager(_serviceProvider);
            _jobManager.Initialize();

            int iInitializedSources = 0;
            foreach (IAlarmSource alarmSource in _alarmSources)
            {
                try
                {
                    Logger.Instance.LogFormat(LogType.Info, this, Resources.AlarmSourceInitializing, alarmSource.GetType().Name);
                    alarmSource.Initialize(_serviceProvider);
                    alarmSource.NewAlarm += AlarmSource_NewAlarm;

                    Thread ast = new Thread(AlarmSourceThreadWrapper);
                    ast.Priority = ThreadPriority.BelowNormal;
                    ast.Name = string.Format(AlarmSourceThreadNameFormat, alarmSource.GetType().Name);
                    ast.IsBackground = true;

                    _alarmSourcesThreads.Add(alarmSource, ast);
                    Logger.Instance.LogFormat(LogType.Info, this, Resources.AlarmSourceStarting, alarmSource.GetType().Name);
                    ast.Start(alarmSource);

                    iInitializedSources++;
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, Resources.AlarmSourceStartException, alarmSource.GetType().FullName);
                    Logger.Instance.LogException(this, ex);
                }
            }

            if (iInitializedSources > 0)
            {
                Logger.Instance.LogFormat(LogType.Info, this, Resources.EngineStarted);
                _isStarted = true;
            }
        }

        /// <summary>
        /// Wraps execution of an <see cref="IAlarmSource"/>-plugin to avoid terminating the whole process.
        /// </summary>
        /// <param name="parameter">The parameter (of type <see cref="IAlarmSource"/>).</param>
        private void AlarmSourceThreadWrapper(object parameter)
        {
            IAlarmSource source = (IAlarmSource)parameter;
            try
            {
                source.RunThread();
            }
            catch (ThreadAbortException)
            {
                // Ignore this exception. It is caused by calling Stop().
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Resources.AlarmSourceThreadException, source.GetType().FullName);
                Logger.Instance.LogException(this, ex);
            }
        }

        /// <summary>
        /// Stops the engine and disposes all alarm sources and other plugins.
        /// </summary>
        internal void Stop()
        {
            if (!_isStarted)
            {
                return;
            }

            Logger.Instance.LogFormat(LogType.Info, this, Resources.EngineStopping);

            foreach (IAlarmSource alarmSource in _alarmSources)
            {
                try
                {
                    alarmSource.NewAlarm -= AlarmSource_NewAlarm;
                    alarmSource.Dispose();

                    if (_alarmSourcesThreads.ContainsKey(alarmSource))
                    {
                        Thread ast = _alarmSourcesThreads[alarmSource];
                        ast.Abort();

                        _alarmSourcesThreads.Remove(alarmSource);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, Resources.AlarmSourceDisposeException, alarmSource.GetType().FullName);
                    Logger.Instance.LogException(this, ex);
                }
            }

            _jobManager.Dispose();
            _jobManager = null;

            Logger.Instance.LogFormat(LogType.Info, this, Resources.EngineStopped);

            _isStarted = false;
        }

        private void AlarmSource_NewAlarm(object sender, AlarmSourceEventArgs e)
        {
            IAlarmSource source = (IAlarmSource)sender;

            Operation operation = e.Operation;

            if (operation == null)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Resources.AlarmSourceNoOperation, source.GetType().FullName);
                return;
            }

            Logger.Instance.LogFormat(LogType.Info, this, Resources.AlarmSourceReceivedOperation, operation.ToString(), sender.GetType().Name);
            try
            {
                if (!ShouldStoreOperation(operation))
                {
                    Logger.Instance.LogFormat(LogType.Info, this, Resources.NewAlarmIgnoringAlreadyPresentOperation, operation.OperationNumber);
                    return;
                }

                // If there is no timestamp, use the current time.
                if (operation.Timestamp.Year == 1)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, Resources.NewAlarmInvalidTimestamp);
                    operation.Timestamp = DateTime.Now;
                }

                JobContext context = new JobContext(source, e);
                context.Phase = JobPhase.OnOperationSurfaced;
                _jobManager.ExecuteJobs(context, operation);

                operation = StoreOperation(operation);
                if (operation == null)
                {
                    return;
                }

                Logger.Instance.LogFormat(LogType.Info, this, Resources.NewAlarmStored, operation.Id);

                context.Phase = JobPhase.AfterOperationStored;
                _jobManager.ExecuteJobs(context, operation);

                Logger.Instance.LogFormat(LogType.Info, this, Resources.NewAlarmHandlingFinished, operation.Id);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Resources.NewAlarmHandlingException);
                Logger.Instance.LogException(this, ex);
            }
        }

        private bool ShouldStoreOperation(Operation operation)
        {
            bool settingEnabled = _settingsService.GetSetting(SettingKeys.IgnoreOperationsWithSameOperationNumber).GetValue<bool>();
            if (settingEnabled)
            {
                return !_operationService.ExistsOperation(operation.OperationNumber);
            }

            return true;
        }

        private Operation StoreOperation(Operation operation)
        {
            try
            {
                return _operationService.StoreOperation(operation);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Resources.NewAlarmStoreOperationFailedMessage);
                Logger.Instance.LogException(this, ex);
                return null;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}