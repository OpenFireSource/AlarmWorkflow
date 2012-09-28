using System;
using System.Diagnostics;
using System.ServiceModel;

namespace AlarmWorkflow.Windows.Service.WcfServices
{
    /// <summary>
    /// Wraps a service type for comfortable dynamic creation and cleanup.
    /// </summary>
    /// <typeparam name="TService">The type of the wrapped service.</typeparam>
    [DebuggerDisplay("Service wrapper for service type {_instance.GetType().Name}")]
    public sealed class WrappedService<TService> : IDisposable where TService : class
    {
        #region Fields

        private bool _isDisposed;
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
                        throw new InvalidOperationException("Could not connect to the service!");
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

        private void AssertNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        private ICommunicationObject GetCommunicationObject()
        {
            return (ICommunicationObject)_instance;
        }

        /// <summary>
        /// Disposes this instance and closes the underlying communication object.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            // A dispose-method should never throw any exceptions
            try
            {
                GetCommunicationObject().Close();
            }
            catch { }
            _isDisposed = true;
        }

        #endregion
    }
}
