#region

using System;
using System.Windows;
using System.Windows.Threading;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.CustomViewer.Extensibility;

#endregion

namespace AlarmWorkflow.Windows.UIWidgets.Clock
{
    /// <summary>
    ///     Interaktionslogik für UserControl1.xaml
    /// </summary>
    [Export("ClockWidget", typeof (IUIWidget))]
    public partial class ClockWidget : IUIWidget
    {
        #region Fields

        private Operation _operation;

        #endregion Fields

        #region Constructors

        public ClockWidget()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer
                                        {
                                            Interval = TimeSpan.FromSeconds(1.0)
                                        };
            timer.Start();
            timer.Tick += delegate
                              {
                                  ClockBlock.Text = DateTime.Now.ToString("HH:mm:ss");
                                  if (_operation != null)
                                  {
                                      TimeSpan timeSpan = DateTime.Now - _operation.Timestamp;
                                      AlarmClock.Text = new DateTime(timeSpan.Ticks).ToString("HH:mm:ss");
                                  }
                              };
        }

        #endregion Constructors

        #region IUIWidget Member

        bool IUIWidget.Initialize()
        {
            return true;
        }

        void IUIWidget.OnOperationChange(Operation operation)
        {
            _operation = operation;
        }

        UIElement IUIWidget.UIElement
        {
            get { return this; }
        }

        string IUIWidget.ContentGuid
        {
            get { return "F059039A-B0CF-41FE-9E17-33261E423308"; }
        }

        string IUIWidget.Title
        {
            get { return "Uhr"; }
        }

        #endregion IUIWidget Member
    }
}