using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace AlarmWorkflow.Windows.Service.WcfServices
{
    /// <summary>
    /// Provides functionality to create connections to services.
    /// </summary>
    public static class ServiceFactory
    {
        #region Factory methods

        /// <summary>
        /// Creates a service connection to the service behind the specified service interface
        /// and returns a safe wrapper which shall be used inside a using-statement.
        /// </summary>
        /// <typeparam name="T">The service interface type.</typeparam>
        /// <returns>A safe wrapper which shall be used inside a using-statement.</returns>
        public static WrappedService<T> GetServiceWrapper<T>() where T : class
        {
            return new WrappedService<T>(GetServiceInstance<T>());
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
        private static T GetServiceInstance<T>() where T : class
        {
            // Don't allow implementations or structs
            if (typeof(T).IsValueType || typeof(T).IsClass || !typeof(T).IsInterface)
            {
                throw new InvalidOperationException("Invalid service interface requested! Can only work with interfaces!");
            }
            if (!typeof(T).Name.StartsWith("I"))
            {
                throw new InvalidOperationException("Interfaces must - by convention - start with an I!");
            }

            // Service names ought to start with an 'I'
            string serviceName = typeof(T).Name.Remove(0, 1);
            EndpointAddress endpointAddress = new EndpointAddress(ServiceConstants.GetServiceUrl(serviceName));

            ChannelFactory<T> d = new ChannelFactory<T>(ServiceConstants.ServicesBinding, endpointAddress);
            d.Endpoint.Behaviors.Add(new WebHttpBehavior());
            return d.CreateChannel();
        }

        #endregion
    }
}
