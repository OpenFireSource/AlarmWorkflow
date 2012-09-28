using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace AlarmWorkflow.Windows.Service.WcfServices
{
    /// <summary>
    /// Host for the AlarmWorkflow-WCF-Services.
    /// </summary>
    public sealed class WcfServicesHostManager
    {
        #region Fields

        private List<ServiceHost> _services;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfServicesHostManager"/> class.
        /// </summary>
        public WcfServicesHostManager()
        {
            _services = new List<ServiceHost>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes and hosts all services.
        /// </summary>
        public void Initialize()
        {
            HostService(new ServiceConfiguration()
            {
                ContractType = typeof(IAlarmWorkflowService),
                ServiceType = typeof(AlarmWorkflowService),
                UrlPath = "AlarmWorkflowService"
            });
        }

        /// <summary>
        /// Hosts the given service.
        /// </summary>
        /// <param name="configuration"></param>
        private void HostService(ServiceConfiguration configuration)
        {
            try
            {
                // Create a new Singleton-instance
                object instance = Activator.CreateInstance(configuration.ServiceType);

                // Create a new host for the Singleton-instance
                ServiceHost host = new ServiceHost(instance);
                // Create endpoints for per-session instances (used by the clients)
                host.AddServiceEndpoint(configuration.ContractType, ServiceConstants.ServicesBinding, ServiceConstants.GetServiceUrl(configuration.UrlPath));

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

        #region Nested types

        class ServiceConfiguration
        {
            public Type ContractType { get; set; }
            public Type ServiceType { get; set; }
            public string UrlPath { get; set; }
        }

        #endregion
    }
}
