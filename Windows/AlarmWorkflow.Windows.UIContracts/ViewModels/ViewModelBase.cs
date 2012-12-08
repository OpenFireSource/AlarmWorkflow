using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace AlarmWorkflow.Windows.UIContracts.ViewModels
{
    /// <summary>
    /// ViewModel base class.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets/sets whether or not to automatically wireup commands on constructor-call of the ViewModel.
        /// The default value of this property is <c>true</c>.
        /// </summary>
        protected static bool AutomaticWireupCommands { get; set; }
        /// <summary>
        /// Gets/sets whether or not to automatically unwire commands on dispose of the ViewModel.
        /// The default value of this property is <c>true</c> and is recommended to be left <c>true</c> except for a good reason.
        /// </summary>
        protected static bool AutomaticUnwireCommands { get; set; }

        /// <summary>
        /// Gets whether or not this object was already disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static ViewModelBase()
        {
            AutomaticWireupCommands = true;
            AutomaticUnwireCommands = true;
        }

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        public ViewModelBase()
        {
            if (AutomaticWireupCommands)
            {
                CommandHelper.WireupRelayCommands(this);
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when the value of a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Manually raises the PropertyChanged event for the given property.
        /// </summary>
        /// <param name="propertyName">The name of the property to raise this event for.</param>
        public virtual void OnPropertyChanged(string propertyName)
        {
            var copy = PropertyChanged;
            if (copy != null)
            {
                copy(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Called when this viewmodel shall perform cleanup work prior to it being disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Thrown if the model was already disposed.</exception>
        public void Dispose()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            try
            {
                // first, do a custom dispose
                // we need to do this inside a try-catch because:
                // - the application can shut down, but the finalizer may come up
                // - then, dispose() is called... we never know what user-code is executed!
                this.DisposeInner();
            }
            catch (Exception ex)
            {
                if (Application.Current != null)
                {
                    // only throw this exception if the application is running, see remark above
                    throw ex;
                }

                // either way, trace this
                System.Diagnostics.Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Warning: Exception of type '{0}' caught while disposing object of type '{1}'. The error message was: {2}", ex.GetType().Name, this.GetType().Name, ex.Message));
            }

            // automatically unwire commands?
            if (AutomaticUnwireCommands)
            {
                CommandHelper.UnwireRelayCommands(this);
            }

            // mark instance as disposed
            this.IsDisposed = true;

            // avoid calls to finalizer
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs implementation-specific tasks on Dispose.
        /// </summary>
        protected virtual void DisposeInner()
        {

        }

        #endregion
    }
}
