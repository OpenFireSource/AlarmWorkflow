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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using AlarmWorkflow.Backend.Service.Communication;
using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.Backend.ServiceContracts.ServiceDefinition;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Backend.Service
{
    class AlarmWorkflowServiceManager : DisposableObject, IServiceProvider
    {
        #region Fields

        private List<IInternalService> _hostedInternalServices;
        private List<ServiceHost> _hostedExposedServices;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceManager"/> class.
        /// </summary>
        public AlarmWorkflowServiceManager()
        {
            _hostedInternalServices = new List<IInternalService>();
            _hostedExposedServices = new List<ServiceHost>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Start the service manager by initializing and starting all registered services.
        /// </summary>
        /// <param name="args">The arguments that have been passed to the service.</param>
        /// <exception cref="System.ObjectDisposedException">The instance has been disposed of.</exception>
        public void OnStart(string[] args)
        {
            AssertNotDisposed();

            Logger.Instance.LogFormat(LogType.Info, this, Properties.Resources.SvcMgrStarting);

            try
            {
                ServiceFactory.EndPointResolver = new AnyInterfaceEndPointResolver();

                HostAllServices();

                // After all services have been hosed, iterate over the internal services and signal start.
                foreach (IInternalService service in _hostedInternalServices)
                {
                    service.OnStart();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.ServiceStartError_Message, ex.Message);
                Logger.Instance.LogException(this, ex);
                throw ex;
            }

            Logger.Instance.LogFormat(LogType.Info, this, Properties.Resources.SvcMgrStarted, _hostedInternalServices.Count, _hostedExposedServices.Count);
        }

        private void HostAllServices()
        {
            foreach (IBackendServiceLocation serviceLocation in ServiceBindingCache.GetServiceLocations())
            {
                Logger.Instance.LogFormat(LogType.Info, this, Properties.Resources.SvcMgrServiceHosting, serviceLocation.Name);
                try
                {
                    Stopwatch sw = Stopwatch.StartNew();

                    HostService(serviceLocation);

                    sw.Stop();
                    Logger.Instance.LogFormat(LogType.Info, this, Properties.Resources.SvcMgrServiceHosted, sw.ElapsedMilliseconds);
                }
                catch (Exception exception)
                {
                    Logger.Instance.LogException(this, exception);
                    Logger.Instance.LogFormat(LogType.Info, this, Properties.Resources.SvcMgrServiceHostingError);
                }
            }
        }

        private void HostService(IBackendServiceLocation serviceLocation)
        {
            // Decide which kind of service to host.
            Type[] interfaces = serviceLocation.ContractType.GetInterfaces();
            if (interfaces.Any(f => f.Equals(typeof(IInternalService))))
            {
                HostInternalService(serviceLocation);
            }
            else if (interfaces.Any(f => f.Equals(typeof(IExposedService))))
            {
                HostExposedService(serviceLocation);
            }
            else
            {
                // TODO: Details!
                throw new InvalidOperationException("Invalid service detected!");
            }
        }

        private void HostInternalService(IBackendServiceLocation serviceLocation)
        {
            object serviceInstanceRaw = Activator.CreateInstance(serviceLocation.ServiceType);
            IInternalService serviceInstance = serviceInstanceRaw as IInternalService;
            if (serviceInstance == null)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.SvcMgrServiceInstanceIllegalType, typeof(IInternalService));
                return;
            }

            serviceInstance.Initialize(this);

            _hostedInternalServices.Add(serviceInstance);
            ServiceInternalProvider.Instance.AddService(serviceLocation.ContractType, serviceInstance);
        }

        private void HostExposedService(IBackendServiceLocation serviceLocation)
        {
            // Configure the ServiceHost to be a Per-Session service.
            ServiceHost host = new ServiceHost(serviceLocation.ServiceType);

            Binding binding = ServiceBindingCache.GetBindingForContractType(serviceLocation.ContractType);
            EndpointAddress address = ServiceFactory.GetEndpointAddress(serviceLocation.ContractType, binding);
            ServiceEndpoint endpoint = host.AddServiceEndpoint(serviceLocation.ContractType, binding, address.Uri);

            _hostedExposedServices.Add(host);

            host.Open();
        }

        /// <summary>
        /// Stops the service manager by shutting down all services, releasing resources etc.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">The instance has been disposed of.</exception>
        public void OnStop()
        {
            AssertNotDisposed();

            Logger.Instance.LogFormat(LogType.Info, this, Properties.Resources.SvcMgrStopping);

            for (int i = _hostedExposedServices.Count - 1; i >= 0; i--)
            {
                ServiceHost s = _hostedExposedServices[i];
                switch (s.State)
                {
                    case CommunicationState.Faulted:
                        s.Abort();
                        break;
                    case CommunicationState.Opened:
                    case CommunicationState.Opening:
                        s.Close();
                        break;
                    default:
                        break;
                }
            }

            _hostedExposedServices.Clear();

            for (int i = _hostedInternalServices.Count - 1; i >= 0; i--)
            {
                IInternalService s = _hostedInternalServices[i];
                s.OnStop();
                s.Dispose();
            }

            _hostedInternalServices.Clear();

            Logger.Instance.LogFormat(LogType.Info, this, Properties.Resources.SvcMgrStopped);
        }

        /// <summary>
        /// Disposes of the current instance.
        /// </summary>
        protected override void DisposeCore()
        {
        }

        #endregion

        #region IServiceProvider Members

        object IServiceProvider.GetService(Type serviceType)
        {
            return ServiceInternalProvider.Instance.GetService(serviceType);
        }

        #endregion
    }
}