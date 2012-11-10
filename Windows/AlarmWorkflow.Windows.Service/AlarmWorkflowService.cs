using System.ServiceProcess;

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
            _manager.OnStart();
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            _manager.OnStop();
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
