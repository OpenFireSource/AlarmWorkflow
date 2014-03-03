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
using System.Diagnostics;
using System.ServiceModel;
using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Backend.ServiceContracts.Communication
{
    /// <summary>
    /// Wraps a service type for comfortable dynamic creation and cleanup.
    /// </summary>
    /// <typeparam name="TService">The type of the wrapped service.</typeparam>
    [DebuggerDisplay("Service wrapper for service type {_instance.GetType().Name}")]
    public sealed class WrappedService<TService> : DisposableObject where TService : class, IExposedService
    {
        #region Fields

        private TService _instance;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the service instance for access.
        /// </summary>
        public TService Instance
        {
            get
            {
                AssertNotDisposed();

                if (!IsOpen)
                {
                    if (!IsFaulted && !IsClosed)
                    {
                        try
                        {
                            GetCommunicationObject().Open();
                        }
                        catch
                        {
                            throw;
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException(Properties.Resources.WrappedService_GetInstanceException);
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Gets whether or not this service is open and can be used.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                AssertNotDisposed();

                return GetCommunicationObject().State == CommunicationState.Opened;
            }
        }

        /// <summary>
        /// Gets whether or not this service is faulted and cannot be used anymore.
        /// Trying to access this service if this is <c>true</c> then it will throw an exception.
        /// </summary>
        public bool IsFaulted
        {
            get
            {
                AssertNotDisposed();

                return GetCommunicationObject().State == CommunicationState.Faulted;
            }
        }

        /// <summary>
        /// Gets whether or not this service is closed and cannot be used anymore.
        /// Trying to access this service if this is <c>true</c> then it will throw an exception.
        /// </summary>
        public bool IsClosed
        {
            get
            {
                AssertNotDisposed();

                return GetCommunicationObject().State == CommunicationState.Closed;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WrappedService&lt;TService&gt;"/> class.
        /// </summary>
        /// <param name="serviceInstance">The service instance.</param>
        internal WrappedService(TService serviceInstance)
            : base()
        {
            _instance = serviceInstance;
        }

        #endregion

        #region Methods

        private ICommunicationObject GetCommunicationObject()
        {
            return (ICommunicationObject)_instance;
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        protected override void DisposeCore()
        {
            // A dispose-method should never throw any exceptions
            try
            {
                GetCommunicationObject().Close();
            }
            catch { }
        }

        #endregion
    }
}
