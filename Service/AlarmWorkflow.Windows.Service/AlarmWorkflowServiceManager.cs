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
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Engine;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Windows.Service.WcfServices;

namespace AlarmWorkflow.Windows.Service
{
    /// <summary>
    /// Contains the actual Service-logic of the Windows-Service.
    /// </summary>
    internal class AlarmWorkflowServiceManager : IDisposable
    {
        #region Fields

        private AlarmWorkflowEngine _alarmWorkflow;
        private WcfServicesHostManager _servicesHostManager;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmWorkflowServiceManager"/> class.
        /// </summary>
        public AlarmWorkflowServiceManager()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when the Windows-service is starting.
        /// </summary>
        internal void OnStart()
        {
            try
            {
                // Initialize the settings on every start.
                SettingsManager.Instance.Invalidate();
                SettingsManager.Instance.Initialize();

                _alarmWorkflow = new AlarmWorkflowEngine();
                _alarmWorkflow.Start();

                _servicesHostManager = new WcfServicesHostManager(_alarmWorkflow);
                _servicesHostManager.Initialize();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.ServiceStartError_Message, ex.Message);
                Logger.Instance.LogException(this, ex);
                throw ex;
            }
        }

        /// <summary>
        /// Stops the Windows-Service.
        /// </summary>
        internal void OnStop()
        {
            _alarmWorkflow.Stop();
            _servicesHostManager.Shutdown();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        #endregion
    }
}