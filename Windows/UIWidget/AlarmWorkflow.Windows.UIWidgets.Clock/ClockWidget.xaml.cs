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
using System.Windows;
using System.Windows.Threading;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.CustomViewer.Extensibility;

namespace AlarmWorkflow.Windows.UIWidgets.Clock
{
    /// <summary>
    ///     Interaktionslogik für UserControl1.xaml
    /// </summary>
    [Export("ClockWidget", typeof (IUIWidget))]
    [Information(DisplayName = "ExportUIWidgetDisplayName", Description = "ExportUIWidgetDescription")]
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
                                  _clockBlock.Text = DateTime.Now.ToString("HH:mm:ss");
                                  if (_operation != null)
                                  {
                                      TimeSpan timeSpan = DateTime.Now - _operation.Timestamp;
                                      _alarmClock.Text = new DateTime(timeSpan.Ticks).ToString("HH:mm:ss");
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