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
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Backend.ServiceContracts.Core
{
    /// <summary>
    /// Provides the base implementation for a service that is hosted by the Service, but not exposed over WCF.
    /// </summary>
    public abstract class InternalServiceBase : DisposableObject, IInternalService
    {
        #region Properties

        /// <summary>
        /// Gets whether or not this servicce has been initialized.
        /// </summary>
        protected bool IsInitialized { get; private set; }
        /// <summary>
        /// Gets an object that can be used for synchronizing access.
        /// </summary>
        protected object SyncRoot { get; private set; }
        /// <summary>
        /// Gets/sets the service provider that can be used to access services.
        /// </summary>
        protected IServiceProvider ServiceProvider { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalServiceBase"/> class.
        /// </summary>
        protected InternalServiceBase()
        {
            SyncRoot = new object();
        }

        #endregion

        #region Methods

        /// <summary>
        /// When overridden in a derived class, allows for a custom initialization procedure.
        /// At this point, the <see cref="ServiceProvider"/> has already been set.
        /// </summary>
        protected virtual void InitializeOverride()
        {

        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected override void DisposeCore()
        {
        }

        #endregion

        #region IInternalService Members

        /// <summary>
        /// Initializes this service after being added to the service manager.
        /// </summary>
        /// <param name="serviceProvider">The service provider to set.</param>
        public void Initialize(IServiceProvider serviceProvider)
        {
            Assertions.AssertNotNull(serviceProvider, "serviceProvider");

            if (IsInitialized)
            {
                return;
            }

            ServiceProvider = serviceProvider;
            InitializeOverride();
            IsInitialized = true;
        }

        /// <summary>
        /// Called when the parent service is iterating through all services to signal them they can start.
        /// </summary>
        public virtual void OnStart()
        {
        }

        /// <summary>
        /// Called when the parent service is iterating through all services - in inverse order - to signal them they can stop.
        /// </summary>
        public virtual void OnStop()
        {
        }

        #endregion
    }
}
