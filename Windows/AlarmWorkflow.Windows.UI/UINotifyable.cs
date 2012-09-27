using System;
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

        private App _app;
        private EventWindow _eventWindow;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UINotifyable"/> class.
        /// </summary>
        public UINotifyable()
        {
            _app = new App(true);
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
            _app.Run();
        }

        void INotifyable.Notify(Operation operation)
        {
            PushEvent(operation);
        }

        #endregion

        #region IDisposable Members

        void System.IDisposable.Dispose()
        {
            if (_app != null)
            {
                _app.Shutdown();
                _app = null;
            }
        }

        #endregion
    }
}
