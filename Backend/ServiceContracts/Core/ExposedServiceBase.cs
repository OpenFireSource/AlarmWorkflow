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
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Backend.ServiceContracts.Core
{
    /// <summary>
    /// Provides the base implementation for a service running at the backend and exposed over WCF.
    /// </summary>
#if DEBUG
    [ServiceBehavior(UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
#else
    [ServiceBehavior(UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession)]
#endif
    public abstract class ExposedServiceBase : DisposableObject, IExposedService
    {
        #region Properties

        /// <summary>
        /// Gets/sets the service provider that can be used to access services.
        /// </summary>
        protected IServiceProvider ServiceProvider
        {
            get { return ServiceInternalProvider.Instance; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExposedServiceBase"/> class.
        /// </summary>
        protected ExposedServiceBase()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected override void DisposeCore()
        {
        }

        #endregion

        #region IExposedService Members

        IServiceProvider IExposedService.ServiceProvider
        {
            get { return ServiceProvider; }
        }

        /// <summary>
        /// Performs a ping to the service.
        /// </summary>
        public virtual void Ping()
        {

        }

        #endregion
    }
}
