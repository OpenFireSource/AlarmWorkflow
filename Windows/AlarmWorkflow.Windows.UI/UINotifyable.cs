using System;
using System.Threading;
using AlarmWorkflow.Job.ComponentNotificator;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Windows.UI
{
    /// <summary>
    /// Implements the <see cref="INotifyable"/>-interface to spawn a WPF-window on a screen when a new operation arrives,
    /// and supports multiple events being shown in one window.
    /// </summary>
    [Export("UINotifyable", typeof(INotifyable))]
    class UINotifyable : INotifyable
    {
        #region Fields

        private readonly object Lock = new object();

        private Thread _appThread;
        private App _app;
        private EventWindow _eventWindow;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UINotifyable"/> class.
        /// </summary>
        public UINotifyable()
        {
        }

        #endregion

        #region Methods

        private void PushEvent(Operation operation)
        {
            lock (Lock)
            {
                _app.Dispatcher.Invoke((Action)(() =>
                {
                    if (_eventWindow == null)
                    {
                        _eventWindow = new EventWindow();
                        _eventWindow.Closed += (o, e) => { _eventWindow = null; };
                        _eventWindow.Show();
                    }

                    _eventWindow.PushEvent(operation);

                }));
            }
        }

        #endregion

        #region Event handlers

        private void EventWindow_Closed(object sender, System.EventArgs e)
        {
            lock (Lock)
            {
                _eventWindow = null;
            }
        }

        #endregion

        #region INotifyable Members

        void INotifyable.Initialize()
        {
            _appThread = new Thread(() =>
            {
                _app = new App(true);
                _app.Run();
            });
            _appThread.SetApartmentState(ApartmentState.STA);
            _appThread.Start();
        }

        void INotifyable.Notify(Operation operation)
        {
            PushEvent(operation);
        }

        // TODO
        //void INotifyable.Shutdown()
        //{
        //    if (_appThread != null)
        //    {
        //        _appThread.Abort();
        //        _appThread = null;
        //    }
        //    if (_app != null)
        //    {
        //        _app.Shutdown();
        //        _app = null;
        //    }
        //}

        #endregion
    }
}
