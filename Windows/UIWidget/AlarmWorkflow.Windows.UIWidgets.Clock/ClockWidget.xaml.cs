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
using System.Windows.Media;
using System.Windows.Threading;
using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.CustomViewer.Extensibility;

namespace AlarmWorkflow.Windows.UIWidgets.Clock
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    [Export("ClockWidget", typeof(IUIWidget))]
    [Information(DisplayName = "ExportUIWidgetDisplayName", Description = "ExportUIWidgetDescription")]
    public partial class ClockWidget : IUIWidget
    {
        #region Fields

        private Operation _operation;
        private readonly bool _blink;
        private readonly int _waitTimeSetting;
        private readonly Color _color;
        private bool _odd;
        private DispatcherTimer _countdown;
        private readonly DispatcherTimer _clockTimer;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Provides a ClockWidget
        /// </summary>
        public ClockWidget()
        {
            using (var service = ServiceFactory.GetCallbackServiceWrapper<ISettingsService>(new SettingsServiceCallback()))
            {
                object convertFromString = ColorConverter.ConvertFromString(service.Instance.GetSetting(SettingKeys.Color).GetValue<string>());
                if (convertFromString != null)
                {
                    _color = (Color)convertFromString;
                }
                else
                {
                    _color = Colors.Red;
                }
                _waitTimeSetting = service.Instance.GetSetting(SettingKeys.WaitTime).GetValue<int>();
                _blink = service.Instance.GetSetting(SettingKeys.Blink).GetValue<bool>();
            }

            InitializeComponent();

            _clockTimer = new DispatcherTimer();
            _clockTimer.Interval = TimeSpan.FromSeconds(1.0);
            _clockTimer.Start();
            _clockTimer.Tick += ClockTimer_Tick;
        }

        #endregion Constructors

        #region Event handlers

        private void Countdown_Tick(object sender, EventArgs e)
        {
            _clockBlock.Foreground = new SolidColorBrush(_color);
            _alarmClock.Foreground = new SolidColorBrush(_color);
            _countdown.Stop();
            _clockTimer.Tick -= ClockTimer_Tick;
            _clockTimer.Tick += BlinkTimer_Tick;
        }

        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            _clockBlock.Text = DateTime.Now.ToString("HH:mm:ss");
            if (_operation != null)
            {
                TimeSpan timeSpan = DateTime.Now - _operation.Timestamp;
                _alarmClock.Text = new DateTime(timeSpan.Ticks).ToString("HH:mm:ss");
            }
        }

        private void BlinkTimer_Tick(object sender, EventArgs e)
        {
            if (_odd)
            {
                _clockBlock.Text = "";
                _alarmClock.Text = "";
                _odd = false;
            }
            else
            {
                _clockBlock.Text = DateTime.Now.ToString("HH:mm:ss");
                if (_operation != null)
                {
                    TimeSpan timeSpan = DateTime.Now - _operation.Timestamp;
                    _alarmClock.Text = new DateTime(timeSpan.Ticks).ToString("HH:mm:ss");
                }
                _odd = true;
            }
        }

        #endregion

        #region IUIWidget Member

        bool IUIWidget.Initialize()
        {
            return true;
        }

        void IUIWidget.OnOperationChange(Operation operation)
        {
            if (_blink)
            {
                if (_countdown != null && _countdown.IsEnabled)
                {
                    _countdown.Stop();
                }
                TimeSpan timeDiff = DateTime.Now - operation.Timestamp;
                _countdown = new DispatcherTimer();
                if (timeDiff.Minutes > _waitTimeSetting)
                {
                    _countdown.Interval = new TimeSpan(0);
                }
                else
                {
                    _countdown.Interval = TimeSpan.FromMinutes(_waitTimeSetting) - timeDiff;
                }
                _countdown.Tick += Countdown_Tick;
                _countdown.Start();
            }
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