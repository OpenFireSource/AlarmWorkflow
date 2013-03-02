using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Shared.Engine
{
    /// <summary>
    /// Represents the main entry point of the AlarmWorkflow application. This class manages the parsing of the Alarmfaxes and is responsible for calling the configured jobs.
    /// </summary>
    public sealed class AlarmWorkflowEngine : IDisposable
    {
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
            _operationStore = ExportedTypeLibrary.Import<IOperationStore>(AlarmWorkflowConfiguration.Instance.OperationStoreAlias);
            Logger.Instance.LogFormat(LogType.Info, this, "Using operation store '{0}'.", _operationStore.GetType().FullName);
        }

        private void InitializeAlarmSources()
        {
            foreach (var export in ExportedTypeLibrary.GetExports(typeof(IAlarmSource)).Where(j => AlarmWorkflowConfiguration.Instance.EnabledAlarmSources.Contains(j.Attribute.Alias)))
            {
                Logger.Instance.LogFormat(LogType.Info, this, "Enabling alarm source '{0}'...", export.Type.Name);

                IAlarmSource alarmSource = export.CreateInstance<IAlarmSource>();
                _alarmSources.Add(alarmSource);

                Logger.Instance.LogFormat(LogType.Info, this, "Alarm source '{0}' enabled.", export.Type.Name);
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

            InitializeOperationStore();
            InitializeAlarmSources();

            _jobManager = new JobManager();
            _jobManager.Initialize();

            // Initialize each alarm source and register event handler
            int iInitializedSources = 0;
            foreach (IAlarmSource alarmSource in _alarmSources)
            {
                try
                {
                    Logger.Instance.LogFormat(LogType.Info, this, "Initializing Alarmsource " + alarmSource.GetType().Name);
                    alarmSource.Initialize();
                    alarmSource.NewAlarm += AlarmSource_NewAlarm;

                    // Create new thread
                    Thread ast = new Thread(AlarmSourceThreadWrapper);
                    // Use lower priority since we have many threads
                    ast.Priority = ThreadPriority.BelowNormal;
                    ast.Name = string.Format("AlarmWorkflow.Engine.Thread.$" + alarmSource.GetType().Name);

                    // Start the thread
                    _alarmSourcesThreads.Add(alarmSource, ast);
                    Logger.Instance.LogFormat(LogType.Info, this, "Starting Alarmsource " + alarmSource.GetType().Name);
                    ast.Start(alarmSource);

                    iInitializedSources++;
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, "Error initializing alarm source '{0}'. It will not run.", alarmSource.GetType().FullName);
                    Logger.Instance.LogException(this, ex);
                }
            }

            if (iInitializedSources > 0)
            {
                Logger.Instance.LogFormat(LogType.Info, this, "Started Service");
                IsStarted = true;
                return;
            }
            else
            {
                // Having no alarm sources is very critical - throw exception
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.ServiceStartFailedNoAlarmSourceException);
                throw new InvalidOperationException(Properties.Resources.ServiceStartFailedNoAlarmSourceException);
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
                Logger.Instance.LogFormat(LogType.Error, this, "An unhandled exception occurred while running the thread for alarm source '{0}'. The thread is being terminated. Please check the log.", source.GetType().FullName);
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

            Logger.Instance.LogFormat(LogType.Info, this, "Stopping Service");

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
                    Logger.Instance.LogFormat(LogType.Warning, this, "Error disposing alarm source '{0}'.", alarmSource.GetType().FullName);
                    Logger.Instance.LogException(this, ex);
                }
            }

            _jobManager.Dispose();
            _jobManager = null;

            // Dispose operation store
            _operationStore = null;

            Logger.Instance.LogFormat(LogType.Info, this, "Stopped Service");

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

            // Sanity-checks
            if (e.Operation == null)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "Alarm Source '{0}' did not return an operation! This may indicate that parsing an operation has failed. Please check the log!", source.GetType().FullName);
                return;
            }
            Logger.Instance.LogFormat(LogType.Info, this, "Recived operation (" + e.Operation + ") by Alarm Source (" + sender.GetType().Name + "). ");
            try
            {
                // If there is no timestamp, use the current time. Not too good but better than MinValue :-/
                if (e.Operation.Timestamp.Year == 1)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, "Could not parse timestamp from the fax. Using the current time as the timestamp.");
                    e.Operation.Timestamp = DateTime.Now;
                }

                Operation storedOperation = StoreOperation(e.Operation);
                if (storedOperation == null)
                {
                    return;
                }
                Logger.Instance.LogFormat(LogType.Info, this,"Stored operation (ID: "+storedOperation.Id+").");
                IJobContext context = JobContext.FromEventArgs(sender, e);

                _jobManager.ExecuteJobs(context, storedOperation);
                Logger.Instance.LogFormat(LogType.Info, this, "Finished handling operation (ID: " + storedOperation.Id + ").");
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "An exception occurred while processing the operation!");
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
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.NewAlarmStoreOperationFailedMessage);
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
