using System;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Represents an object that is disposable, and that throws an exception if it is used after it has been disposed.
    /// </summary>
    public abstract class DisposableObject : IDisposable
    {
        #region Fields

        private readonly object Lock = new object();

        #endregion

        #region Properties

        /// <summary>
        /// Returns whether or not this instance has been disposed and is no longer usable.
        /// </summary>
        public bool IsDisposed { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if this object is not disposed, and throws an exception if it is.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">The object has been disposed.</exception>
        protected void AssertNotDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            lock (Lock)
            {
                if (IsDisposed)
                {
                    return;
                }

                this.DisposeCore();
                GC.SuppressFinalize(this);

                this.IsDisposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected abstract void DisposeCore();

        #endregion
    }
}
