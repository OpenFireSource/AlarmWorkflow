using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Shared
{
    /// <summary>
    /// Represents the main entry point of the AlarmWorkflow application. This class manages the parsing of the Alarmfaxes and is responsible for calling the configured jobs.
    /// </summary>
    public sealed class AlarmWorkflowEngine : IDisposable
    {
        #region Fields

        private List<IAlarmSource> _alarmSources;
        private Dictionary<IAlarmSource, Thread> _alarmSourcesThreads;

        private List<IJob> _jobs;
        private IOperationStore _operationStore;
        private IRoutePlanProvider _routePlanProvider;

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether or not the AlarmWorkflow-process is running.
        /// </summary>
        public bool IsStarted { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the AlarmWorkflowEngine class.
        /// Constructor is reading the XML File, and safe the Settings in to the WorkingThread Instance.
        /// </summary>
        public AlarmWorkflowEngine()
        {
            _jobs = new List<IJob>();

            _alarmSources = new List<IAlarmSource>();
            _alarmSourcesThreads = new Dictionary<IAlarmSource, Thread>();
        }

        #endregion

        #region Methods

        private void InitializeRoutePlanProvider()
        {

            _routePlanProvider = ExportedTypeLibrary.Import<IRoutePlanProvider>(AlarmWorkflowConfiguration.Instance.RoutePlanProviderAlias);
            Logger.Instance.LogFormat(LogType.Info, this, "Using route plan provider '{0}'.", _routePlanProvider.GetType().FullName);
        }

        private void InitializeOperationStore()
        {
            _operationStore = ExportedTypeLibrary.Import<IOperationStore>(AlarmWorkflowConfiguration.Instance.OperationStoreAlias);
            Logger.Instance.LogFormat(LogType.Info, this, "Using operation store '{0}'.", _operationStore.GetType().FullName);
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
        /// Downloads the route planning info if it is enabled and the location datas are meaningful enough.
        /// </summary>
        /// <param name="operation"></param>
        private void DownloadRoutePlan(Operation operation)
        {
            if (!AlarmWorkflowConfiguration.Instance.DownloadRoutePlan)
            {
                return;
            }

            // Get start address and check if it is meaningful enough (if not then bail out)
            PropertyLocation source = AlarmWorkflowConfiguration.Instance.FDInformation.Location;
            if (!source.IsMeaningful)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "Cannot download route plan because the location information for this fire department is not meaningful enough: '{0}'. Please fill the correct address!", source);
                return;
            }

            // Get destination address and check if it is meaningful enough (if not then bail out)
            PropertyLocation destination = operation.GetDestinationLocation();
            if (!operation.GetDestinationLocation().IsMeaningful)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "Destination location is unknown! Cannot download route plan!");
            }
            else
            {
                Logger.Instance.LogFormat(LogType.Trace, this, "Downloading route plan to destination '{0}'...", destination.ToString());

                Stopwatch sw = Stopwatch.StartNew();
                try
                {
                    Image image = _routePlanProvider.GetRouteImage(source, destination);

                    if (image != null)
                    {
                        // Save the image as PNG
                        using (MemoryStream ms = new MemoryStream())
                        {
                            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                            operation.RouteImage = ms.ToArray();
                        }
                    }

                    sw.Stop();

                    if (operation.RouteImage == null)
                    {
                        Logger.Instance.LogFormat(LogType.Warning, this, "The download of the route plan did not succeed. Please check the log for information!");
                    }
                    else
                    {
                        Logger.Instance.LogFormat(LogType.Trace, this, "Downloaded route plan in '{0}' milliseconds.", sw.ElapsedMilliseconds);
                    }
                }
                catch (Exception ex)
                {
                    sw.Stop();
                    Logger.Instance.LogFormat(LogType.Error, this, "An error occurred while trying to download the route plan! The image will not be available.");
                    Logger.Instance.LogException(this, ex);
                }
            }
        }

        /// <summary>
        /// Starts the monitor thread, which is waiting for a new Alarm.
        /// </summary>
        public void Start()
        {
            if (IsStarted)
            {
                return;
            }

            InitializeOperationStore();
            InitializeRoutePlanProvider();
            InitializeJobs();
            InitializeAlarmSources();

            // Initialize each alarm source and register event handler
            int iInitializedSources = 0;
            foreach (IAlarmSource alarmSource in _alarmSources)
            {
                try
                {
                    alarmSource.Initialize();
                    alarmSource.NewAlarm += AlarmSource_NewAlarm;

                    // Create new thread
                    Thread ast = new Thread(AlarmSourceThreadWrapper);
                    // Use lower priority since we have many threads
                    ast.Priority = ThreadPriority.BelowNormal;
                    ast.Name = string.Format("AlarmWorkflow.Engine.Thread.$" + alarmSource.GetType().Name);

                    // Start the thread
                    _alarmSourcesThreads.Add(alarmSource, ast);
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

            Logger.Instance.LogFormat(LogType.Error, this, "Service failed to start because there are no running alarm sources! Please check the log.");
            this.IsStarted = false;
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
        /// Stops the Thread.
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

            // TODO: Dispose jobs!
            _jobs.Clear();

            // Dispose operation store
            _operationStore = null;
            // Dispose route plan provider
            _routePlanProvider = null;

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

            try
            {
                // If there is no timestamp, use the current time. Not too good but better than MinValue :-/
                if (e.Operation.Timestamp.Year == 1)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, "Could not parse timestamp from the alarmsource. Using the current time as the timestamp.");
                    e.Operation.Timestamp = DateTime.Now;
                }

                // Download the route plan information (if data is meaningful)
                DownloadRoutePlan(e.Operation);

                Operation storedOperation = StoreOperation(e.Operation);
                // When storing the operation failed, leave
                if (storedOperation == null)
                {
                    return;
                }

                foreach (IJob job in _jobs)
                {
                    // Run the job. If the job fails, ignore that exception as well but log it too!
                    try
                    {
                        job.DoJob(storedOperation);
                    }
                    catch (Exception ex)
                    {
                        // Be careful when processing the jobs, we don't want a malicious job to terminate the process!
                        Logger.Instance.LogFormat(LogType.Warning, this, string.Format("An error occurred while processing job '{0}'!", job.GetType().Name));
                        Logger.Instance.LogException(this, ex);
                    }
                }
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
