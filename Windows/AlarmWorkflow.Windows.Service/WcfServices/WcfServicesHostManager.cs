using System;
using System.Collections.Generic;
using System.ServiceModel;
using AlarmWorkflow.Shared;

namespace AlarmWorkflow.Windows.Service.WcfServices
{
    /// <summary>
    /// Host for the AlarmWorkflow-WCF-Services.
    /// </summary>
    public sealed class WcfServicesHostManager
    {
        #region Fields

        private List<ServiceHost> _services;
        private AlarmWorkflowEngine _parent;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfServicesHostManager"/> class.
        /// </summary>
        /// <param name="parent"></param>
        public WcfServicesHostManager(AlarmWorkflowEngine parent)
        {
            _services = new List<ServiceHost>();
            _parent = parent;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes and hosts all services.
        /// </summary>
        public void Initialize()
        {
            HostService();
        }

        /// <summary>
        /// Hosts the main service.
        /// </summary>
        private void HostService()
        {
            try
            {
                // Create a new Singleton-instance
                AlarmWorkflowService instance = new AlarmWorkflowService(_parent);

                // Create a new host for the Singleton-instance
                ServiceHost host = new ServiceHost(instance);
                // Create endpoints for per-session instances (used by the clients)
                host.AddServiceEndpoint(typeof(IAlarmWorkflowService), ServiceConstants.ServicesBinding, ServiceConstants.GetServiceUrl("AlarmWorkflowService"));

                // Add the service to the services list...
                _services.Add(host);

                // ... and try to open the host
                host.Open();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Shuts down and closes all hosted services.
        /// </summary>
        public void Shutdown()
        {
            if (_services != null)
            {
                _services.ForEach(s =>
                {
                    if (s.State == CommunicationState.Opened)
                    {
                        s.Close();
                    }
                });
                _services = null;
            }
        }

        #endregion
    }
}
