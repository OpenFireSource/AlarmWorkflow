using System;
using System.ServiceProcess;
using AlarmWorkflow.Shared;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Windows.Service
{
    /// <summary>
    /// Realizes the service-implementation around the business logic.
    /// </summary>
    public partial class AlarmWorkflowService : ServiceBase
    {
        #region Constants

        private const string EventLogSourceName = "AlarmWorkflow";

        #endregion

        #region Fields

        private AlarmWorkflowEngine _alarmWorkflow;
        private WcfServices.WcfServicesHostManager _servicesHostManager;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmWorkflowService"/> class.
        /// </summary>
        public AlarmWorkflowService()
        {
            InitializeComponent();

            // Then initialize the logger.
            Logger.Instance.Initialize();
            // Then initialize the settings.
            SettingsManager.Instance.Initialize();

            // This call requires Administrator rights. We don't put a Try-catch around because the Windows Service must run with Admin rights.
            if (!System.Diagnostics.EventLog.SourceExists(EventLogSourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(EventLogSourceName, "Application");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// FOR DEBUGGING ONLY!
        /// </summary>
        internal void OnStart()
        {
            OnStart(new string[0]);
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            try
            {
                _alarmWorkflow = new AlarmWorkflowEngine();
                _alarmWorkflow.Start();

                _servicesHostManager = new WcfServices.WcfServicesHostManager(_alarmWorkflow);
                _servicesHostManager.Initialize();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.ServiceStartError_Message, ex.Message);
                Logger.Instance.LogException(this, ex);
            }
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            _alarmWorkflow.Stop();
            _servicesHostManager.Shutdown();

            Logger.Instance.Shutdown();
            SettingsManager.Instance.SaveSettings();
        }

        #endregion

    }
}
