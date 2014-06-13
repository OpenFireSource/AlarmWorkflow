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
using System.ComponentModel.Design;

namespace AlarmWorkflow.Backend.ServiceContracts.Core
{
    /// <summary>
    /// Global instance that is used internally by the service.
    /// Acts as providing services to various types, most notably the hosted services.
    /// </summary>
    class ServiceInternalProvider : IServiceProvider
    {
        #region Fields

        private static readonly ServiceInternalProvider _instance;
        private ServiceContainer _serviceContainer;

        #endregion

        #region Properties

        internal static ServiceInternalProvider Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Constructors

        static ServiceInternalProvider()
        {
            _instance = new ServiceInternalProvider();
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="ServiceInternalProvider"/> class from being created.
        /// </summary>
        private ServiceInternalProvider()
        {
            _serviceContainer = new ServiceContainer();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a new service type and object to the provider.
        /// </summary>
        /// <param name="type">The type of the service to add.</param>
        /// <param name="serviceInstance">The concrete instance to add.</param>
        internal void AddService(Type type, object serviceInstance)
        {
            _serviceContainer.AddService(type, serviceInstance);
        }

        #endregion

        #region IServiceProvider Members

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        public object GetService(Type serviceType)
        {
            return _serviceContainer.GetService(serviceType);
        }

        #endregion
    }
}
