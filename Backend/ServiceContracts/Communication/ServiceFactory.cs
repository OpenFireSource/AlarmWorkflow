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
using System.ServiceModel;
using System.ServiceModel.Channels;
using AlarmWorkflow.Backend.ServiceContracts.Communication.EndPointResolvers;
using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Backend.ServiceContracts.Communication
{
    /// <summary>
    /// Provides functionality to create connections to services.
    /// </summary>
    public static class ServiceFactory
    {
        #region Constants

        private const string ServicesPath = "alarmworkflow/services";

        #endregion

        #region Fields

        private static readonly object _lock = new object();
        private static bool _isInitialized;

        private static IBackendConfigurator _backendConfigurator;
        private static IEndPointResolver _endPointResolver;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the type that provides access to the backend configuration.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The service factory has already been initialized and cannot be changed anymore.</exception>
        public static IBackendConfigurator BackendConfigurator
        {
            get
            {
                lock (_lock)
                {
                    SetInitialized();
                    return _backendConfigurator;
                }
            }
            set
            {
                Assertions.AssertNotNull(value, "value");
                lock (_lock)
                {
                    AssertNotInitialized();
                    _backendConfigurator = value;
                }
            }
        }
        /// <summary>
        /// Gets/sets the type that is used to resolve the IP and port of the server to connect to.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>By default, the <see cref="RedirectableEndPointResolver"/> is used.</remarks>
        /// <exception cref="System.InvalidOperationException">The service factory has already been initialized and cannot be changed anymore.</exception>
        public static IEndPointResolver EndPointResolver
        {
            get
            {
                lock (_lock)
                {
                    SetInitialized();
                    return _endPointResolver;
                }
            }
            set
            {
                Assertions.AssertNotNull(value, "value");
                lock (_lock)
                {
                    AssertNotInitialized();
                    _endPointResolver = value;
                }
            }
        }

        #endregion

        #region Constructors

        static ServiceFactory()
        {

        }

        #endregion

        #region Factory methods

        /// <summary>
        /// Throws an exception if this instance is already initialized.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The service factory has already been initialized and cannot be changed anymore.</exception>
        private static void AssertNotInitialized()
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException("The service factory has already been initialized and cannot be changed!");
            }
        }

        private static void SetInitialized()
        {
            lock (_lock)
            {
                if (_isInitialized)
                {
                    return;
                }

                if (_backendConfigurator == null)
                {
                    _backendConfigurator = new BackendConfigurator();
                }
                if (_endPointResolver == null)
                {
                    _endPointResolver = new RedirectableEndPointResolver(_backendConfigurator);
                }

                _isInitialized = true;
            }
        }

        /// <summary>
        /// Creates the <see cref="EndpointAddress"/> appropriate for the given contract type.
        /// </summary>
        /// <param name="contractType">The type representing the WCF-service contract to get the endpoint address for.</param>
        /// <param name="binding">The binding that is used.</param>
        /// <returns>The <see cref="EndpointAddress"/> appropriate for the given contract type.</returns>
        public static EndpointAddress GetEndpointAddress(Type contractType, Binding binding)
        {
            Assertions.AssertNotNull(contractType, "contractType");
            Assertions.AssertNotNull(binding, "binding");
            AssertContractTypeCorrect(contractType);

            // Infer type name
            string serviceName = contractType.Name.Remove(0, 1);

            return new EndpointAddress(string.Format("{0}://{1}:{2}/{3}/{4}",
                binding.Scheme,
                EndPointResolver.GetServerAddress().ToString(),
                GetPortForBinding(binding),
                ServicesPath,
                serviceName.ToLowerInvariant()));
        }

        private static int GetPortForBinding(Binding binding)
        {
            switch (binding.Scheme)
            {
                case "net.tcp": return int.Parse(BackendConfigurator.Get("Server.NetTcpPort"));
                default:
                    throw new NotSupportedException(string.Format(Properties.Resources.InvalidSupportedBindingValue, binding.Name));
            }
        }

        /// <summary>
        /// Creates a service connection to the service behind the specified service interface.
        /// Please prefer <see cref="M:GetServiceWrapper{T}()"/> method!
        /// See documentation for further information.
        /// </summary>
        /// <remarks>Use this method only if you need to keep a connection to a service open longer than possible with <see cref="M:GetServiceWrapper{T}()"/>.
        /// When finished with the work (or it is safe to release the instance), make a call to <see cref="M:CloseServiceInstance(object)"/> to safely close and dispose the service connection.</remarks>
        /// <typeparam name="T">The service interface type.</typeparam>
        /// <returns>A service connection to the service behind the specified service interface.</returns>
        public static T GetServiceInstance<T>() where T : class, IExposedService
        {
            AssertContractTypeCorrect(typeof(T));

            Binding binding = ServiceBindingCache.GetBindingForContractType(typeof(T));
            ChannelFactory<T> d = new ChannelFactory<T>(binding, GetEndpointAddress(typeof(T), binding));

            T channel = d.CreateChannel();
            channel.Ping();
            return channel;
        }

        private static void AssertContractTypeCorrect(Type type)
        {
            // Don't allow implementations or structs
            if (type.IsValueType || type.IsClass)
            {
                throw new InvalidOperationException(Properties.Resources.ServiceFactoryInvalidType);
            }
            // Service names ought to start with an 'I'
            if (!type.Name.StartsWith("I"))
            {
                throw new InvalidOperationException(Properties.Resources.ServiceFactoryInterfaceNameMalformed);
            }
        }

        /// <summary>
        /// Creates a service connection to the callback service behind the specified service interface.
        /// Please prefer <see cref="M:GetServiceWrapper{T}()"/> method!
        /// See documentation for further information.
        /// </summary>
        /// <remarks>Use this method only if you need to keep a connection to a service open longer than possible with <see cref="M:GetServiceWrapper{T}()"/>.
        /// When finished with the work (or it is safe to release the instance), make a call to <see cref="M:CloseServiceInstance(object)"/> to safely close and dispose the service connection.</remarks>
        /// <typeparam name="T">The service interface type.</typeparam>
        /// <param name="callbackObject">The object representing the callback to use. Must not be null.</param>
        /// <returns>A service connection to the service behind the specified service interface.</returns>
        public static T GetCallbackServiceInstance<T>(object callbackObject) where T : class, IExposedService
        {
            AssertContractTypeCorrect(typeof(T));
            Assertions.AssertNotNull(callbackObject, "callbackObject");

            Binding binding = ServiceBindingCache.GetBindingForContractType(typeof(T));
            DuplexChannelFactory<T> d = new DuplexChannelFactory<T>(callbackObject, binding, GetEndpointAddress(typeof(T), binding));

            T channel = d.CreateChannel();
            channel.Ping();
            return channel;
        }

        /// <summary>
        /// Manually closes the specified service instance. This method is a counterpart to GetServiceInstance().
        /// </summary>
        /// <param name="serviceInstance">The service instance to close.</param>
        public static void CloseServiceInstance(object serviceInstance)
        {
            Assertions.AssertNotNull(serviceInstance, "serviceInstance");

            ICommunicationObject obj = serviceInstance as ICommunicationObject;
            if (obj == null)
            {
                throw new ArgumentException(Properties.Resources.ServiceFactoryInstanceIsNotAServiceObject);
            }
            // Pessimistic: Don't continue if this object is faulted (don't throw an exception here)
            if (obj.State == CommunicationState.Faulted)
            {
                return;
            }

            obj.Close();
        }

        /// <summary>
        /// Creates a service connection to the service behind the specified service interface
        /// and returns a safe wrapper which shall be used inside a using-statement.
        /// </summary>
        /// <typeparam name="T">The service interface type.</typeparam>
        /// <returns>A safe wrapper which shall be used inside a using-statement.</returns>
        public static WrappedService<T> GetServiceWrapper<T>() where T : class, IExposedService
        {
            return new WrappedService<T>(GetServiceInstance<T>());
        }

        /// <summary>
        /// Creates a service connection to the callback service behind the specified service interface
        /// and returns a safe wrapper which shall be used inside a using-statement.
        /// </summary>
        /// <typeparam name="T">The service interface type.</typeparam>
        /// <param name="callbackObject">The object representing the callback to use. Must not be null.</param>
        /// <returns>A safe wrapper which shall be used inside a using-statement.</returns>
        public static WrappedService<T> GetCallbackServiceWrapper<T>(object callbackObject) where T : class, IExposedService
        {
            return new WrappedService<T>(GetCallbackServiceInstance<T>(callbackObject));
        }

        #endregion
    }
}
