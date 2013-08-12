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
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Properties;

namespace AlarmWorkflow.Shared.Engine
{
    /// <summary>
    /// Represents the main entry point of the AlarmWorkflow application. This class manages the parsing of the Alarmfaxes and is responsible for calling the configured jobs.
    /// </summary>
    public sealed class AlarmWorkflowEngine : IDisposable
    {
        #region Constants

        private const string AlarmSourceThreadNameFormat = "AlarmWorkflow.Engine.Thread.${0}";

        #endregion

        #region Fields

        private List<IAlarmSource> _alarmSources;
        private Dictionary<IAlarmSource, Thread> _alarmSourcesThreads;

        private JobManager _jobManager;
        private IOperationStore _operationStore;

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether or not the AlarmWorkflow-process is running.
        /// </summary>
        public bool IsStarted { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmWorkflowEngine"/> class.
        /// </summary>
        public AlarmWorkflowEngine()
        {
            _alarmSources = new List<IAlarmSource>();
            _alarmSourcesThreads = new Dictionary<IAlarmSource, Thread>();
        }

        #endregion

        #region Methods

        private void InitializeOperationStore()
        {
            // Note: This approach will take the first occurrence. This is fine for the moment, since we plan to only support MySQL.
            _operationStore = ExportedTypeLibrary.Import<IOperationStore>();
            Logger.Instance.LogFormat(LogType.Info, this, Resources.InitializedOperationStore, _operationStore.GetType().FullName);
        }

        private void InitializeAlarmSources()
        {
            foreach (var export in ExportedTypeLibrary.GetExports(typeof(IAlarmSource)).Where(j => AlarmWorkflowConfiguration.Instance.EnabledAlarmSources.Contains(j.Attribute.Alias)))
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
        public void Start()
        {
            if (IsStarted)
            {
                return;
            }

            Logger.Instance.LogFormat(LogType.Info, this, Resources.EngineStarting);

            InitializeOperationStore();
            InitializeAlarmSources();

            _jobManager = new JobManager();
            _jobManager.Initialize();

            int iInitializedSources = 0;
            foreach (IAlarmSource alarmSource in _alarmSources)
            {
                try
                {
                    Logger.Instance.LogFormat(LogType.Info, this, Resources.AlarmSourceInitializing, alarmSource.GetType().Name);
                    alarmSource.Initialize();
                    alarmSource.NewAlarm += AlarmSource_NewAlarm;

                    // Create new thread
                    Thread ast = new Thread(AlarmSourceThreadWrapper);
                    // Use lower priority since we have many threads
                    ast.Priority = ThreadPriority.BelowNormal;
                    ast.Name = string.Format(AlarmSourceThreadNameFormat, alarmSource.GetType().Name);

                    // Start the thread
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
                IsStarted = true;
                return;
            }
            else
            {
                // Having no alarm sources is very critical - throw exception
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.EngineStartFailedNoAlarmSourceException);
                throw new InvalidOperationException(Properties.Resources.EngineStartFailedNoAlarmSourceException);
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
                // Log any exception (the thread will end afterwards).
                Logger.Instance.LogFormat(LogType.Error, this, Resources.AlarmSourceThreadException, source.GetType().FullName);
                Logger.Instance.LogException(this, ex);
            }
        }

        /// <summary>
        /// Stops the engine and disposes all alarm sources and other plugins.
        /// </summary>
        public void Stop()
        {
            if (!IsStarted)
            {
                return;
            }

            Logger.Instance.LogFormat(LogType.Info, this, Resources.EngineStopping);

            // Dispose and kill all threads
            foreach (IAlarmSource alarmSource in _alarmSources)
            {
                try
                {
                    // Unregister from event and dispose source
                    alarmSource.NewAlarm -= AlarmSource_NewAlarm;
                    alarmSource.Dispose();

                    // Stop and remove the thread, if existing
                    if (_alarmSourcesThreads.ContainsKey(alarmSource))
                    {
                        Thread ast = _alarmSourcesThreads[alarmSource];
                        // Abort and ignore exception
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

            _operationStore = null;

            Logger.Instance.LogFormat(LogType.Info, this, Resources.EngineStopped);

            IsStarted = false;
        }

        /// <summary>
        /// Returns the <see cref="IOperationStore"/>-instance that is used.
        /// </summary>
        /// <returns></returns>
        public IOperationStore GetOperationStore()
        {
            return _operationStore;
        }

        #endregion

        #region Event handlers

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

        private Operation StoreOperation(Operation operation)
        {
            try
            {
                return _operationStore.StoreOperation(operation);
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