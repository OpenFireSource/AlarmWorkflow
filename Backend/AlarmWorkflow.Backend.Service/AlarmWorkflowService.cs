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

using System.ServiceProcess;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Diagnostics.Reports;

namespace AlarmWorkflow.Backend.Service
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
                _manager.OnStart(args);
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