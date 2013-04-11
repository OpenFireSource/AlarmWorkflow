using System.ServiceProcess;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Diagnostics.Reports;

namespace AlarmWorkflow.Windows.Service
{
    /// <summary>
    /// Realizes the service-implementation around the business logic.
    /// </summary>
    public partial class AlarmWorkflowService : ServiceBase
    {
        #region Fields

        private AlarmWorkflowServiceManager _manager;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmWorkflowService"/> class.
        /// </summary>
        public AlarmWorkflowService()
        {
            InitializeComponent();

            ErrorReportManager.RegisterAppDomainUnhandledExceptionListener(this.GetType().Name);

            // Set up the logger for this instance
            Logger.Instance.Initialize(this.GetType().Name);

            _manager = new AlarmWorkflowServiceManager();
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
                _manager.OnStart();
            }
            catch (System.Exception)
            {
                // Perform a controlled stop when we encounter an exception
                this.Stop();
            }
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                _manager.OnStop();
            }
            catch (System.Exception)
            {
                // Stop shall always work
            }
        }

        /// <summary>
        /// When implemented in a derived class, executes when the system is shutting down. Specifies what should occur immediately prior to the system shutting down.
        /// </summary>
        protected override void OnShutdown()
        {
            base.OnShutdown();

            _manager.Dispose();
        }

        #endregion

    }
}
