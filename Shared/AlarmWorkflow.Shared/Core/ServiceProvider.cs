using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Provides an application-wide point for services.
    /// </summary>
    [DebuggerDisplay("{_serviceContainer.Services.Count} Service(s)")]
    public sealed class ServiceProvider : IServiceContainer
    {
        #region Singleton

        private static volatile ServiceProvider _instance;

        /// <summary>
        /// (Singleton) Gets the current instance.
        /// </summary>
        public static ServiceProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServiceProvider();
                }
                return _instance;
            }
        }

        #endregion

        #region Fields

        private ServiceContainer _serviceContainer;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Services class.
        /// </summary>
        private ServiceProvider()
        {
            _serviceContainer = new ServiceContainer();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Removes all services safely.
        /// </summary>
        public void RemoveAllServices()
        {
            // Grab the private hashtable ...
            var services = _serviceContainer.GetType().GetField("services", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_serviceContainer) as Dictionary<Type, object>;
            
            // Afterwards remove each registered service
            while (services.Keys.Count > 0)
            {
                Type service = (Type)services.Keys.OfType<Type>().First();
                RemoveService(service);
            }
        }

        #endregion

        #region IServiceContainer Member

        /// <summary>
        /// This method is not supported.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="callback"></param>
        /// <param name="promote"></param>
        void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// This method is not supported.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="callback"></param>
        void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// This method is not supported.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="serviceInstance"></param>
        /// <param name="promote"></param>
        void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Adds a new service.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="serviceInstance"></param>
        public void AddService(Type serviceType, object serviceInstance)
        {
            _serviceContainer.AddService(serviceType, serviceInstance);

            Logger.Instance.LogFormat(LogType.Info, this, "Registered service type '{0}' (instance type: {1}).", serviceType, serviceInstance.GetType());
        }

        /// <summary>
        /// This method is not supported.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="promote"></param>
        void IServiceContainer.RemoveService(Type serviceType, bool promote)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Removes the given service.
        /// </summary>
        /// <param name="serviceType"></param>
        public void RemoveService(Type serviceType)
        {
            _serviceContainer.RemoveService(serviceType);
        }

        #endregion

        #region IServiceProvider Member

        /// <summary>
        /// Returns the service from its type.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns>The service from its type.
        /// -or- <c>null</c>, if the service did not exist.</returns>
        public object GetService(Type serviceType)
        {
            return _serviceContainer.GetService(serviceType);
        }

        #endregion
    }
}
