using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace AlarmWorkflow.Windows.UI.ViewModels
{
    /// <summary>
    /// Base for all FireFighter Client ViewModels.
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

        /// <summary>
        /// Finalizing destructor, which will perform some cleanup work.
        /// </summary>
        ~ViewModelBase()
        {
            // the finalizer is there to cleanup if the user forgot to
            // but only proceed if this instance wasn't already disposed
            if (!this.IsDisposed)
            {
                this.Dispose();
            }

            // only when the application is alive, track this occurrence (see remark below)
            if (Application.Current != null)
            {
                // trace this. one should always manually call Dispose(), which is faster since the GC doesn't need to do extra work then!
                System.Diagnostics.Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Warning: The finalizer was called on object of type '{0}'. Please revise your code and see if you can call Dispose() manually!", this.GetType().Name));
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

        /// <summary>
        /// Manually raises the PropertyChanged event, and looks up the appropriate property using stack trace.
        /// See documentation for further (important) information.
        /// </summary>
        /// <remarks><list type="bullet">
        /// <item><description>Use as scarcely as possible!</description></item>
        /// <item><description>This does only work when it is executed right inside the setter of the property!</description></item>
        /// <item><description>Calling this method takes some time because of using the stack trace (around 0 - 5 ms depending on performance).</description></item>
        /// </list></remarks>
        protected void OnPropertyChanged()
        {
#if DEBUG
            Stopwatch sw = Stopwatch.StartNew();
#endif

            // retrieve calling property method (the setter) via stack trace
            var st = new StackTrace();
            if (st.FrameCount < 2)
            {
                throw new InvalidOperationException("Encountered stack trace that seems to be invalid!");
            }
            // retrieve the calling method (which is number 2)
            var sf = st.GetFrame(1);
            // retrieve the calling method, and hope it's a setter
            var m = sf.GetMethod();

            // ensure we come from a setter
            if (!m.Name.StartsWith("set_"))
            {
                throw new InvalidOperationException("Invalid usage of OnPropertyChanged(void)! Method call must be placed inside the setter of the affected property!");
            }

            // get the property name, and then we win!
            string propertyName = m.Name.Remove(0, 4);
            OnPropertyChanged(propertyName);

#if DEBUG
            sw.Stop();
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Call to 'OnPropertyChanged()' (from {0}) took {1} ms.", this.GetType().Name, sw.ElapsedMilliseconds));
#endif
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
