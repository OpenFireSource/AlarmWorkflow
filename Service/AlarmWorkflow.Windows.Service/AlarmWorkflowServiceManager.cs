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
