﻿using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;
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
            // Host the public web service
            {
                string address = "http://localhost:60002/AlarmWorkflow/AlarmWorkflowService";
                Binding binding = new WebHttpBinding()
                {
                    HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.WeakWildcard,
                    MaxReceivedMessageSize = int.MaxValue,
                    ReaderQuotas = XmlDictionaryReaderQuotas.Max,
                };

                HostService(address, binding, typeof(IAlarmWorkflowService), new AlarmWorkflowService(_parent));
            }

            // Host the service used for local-machine communication between service and clients (such as the Windows/Linux UI)
            {
                string address = "net.pipe://localhost/alarmworkflow/service";
                NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport)
                {
                    MaxReceivedMessageSize = int.MaxValue,
                    ReaderQuotas = XmlDictionaryReaderQuotas.Max,
                };

                HostService(address, binding, typeof(IAlarmWorkflowServiceInternal), new AlarmWorkflowServiceInternal(_parent));
            }
        }


        /// <summary>
        /// Hosts the service using the given A-B-C-parameters.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="binding">The binding.</param>
        /// <param name="contractType">Type of the contract.</param>
        private void HostService(string address, Binding binding, Type contractType, object instance)
        {
            try
            {
                // See http://www.codeproject.com/Articles/358867/WCF-and-Android-Part-I for information

                // Create a new Singleton-instance

                // Create a new host for the Singleton-instance
                ServiceHost host = new ServiceHost(instance);
                // Create endpoints for per-session instances (used by the clients)
                ServiceEndpoint endpoint = host.AddServiceEndpoint(contractType, binding, address);
                // Add behaviors if necessary
                if (binding is WebHttpBinding)
                {
                    endpoint.Behaviors.Add(new WebHttpBehavior()
                    {
#if DEBUG
                        HelpEnabled = true,
#endif
                        AutomaticFormatSelectionEnabled = true
                    });
                }

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