using System;
using System.Diagnostics;
using System.ServiceProcess;
using AlarmWorkflow.Shared;

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

        private AlarmworkflowClass _alarmWorkflow;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmWorkflowService"/> class.
        /// </summary>
        public AlarmWorkflowService()
        {
            InitializeComponent();

            // This call requires Administrator rights. We don't put a Try-catch around because the Windows Service must run with Admin rights.
            if (!System.Diagnostics.EventLog.SourceExists(EventLogSourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(EventLogSourceName, "Application");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            try
            {
                _alarmWorkflow = new AlarmworkflowClass();
                _alarmWorkflow.Start();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(EventLogSourceName, string.Format(Properties.Resources.ServiceStartError_Message, ex.Message), EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            _alarmWorkflow.Stop();
        }

        #endregion

    }
}
