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
using System.ComponentModel;
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
    public partial class ClockWidget : IUIWidget, INotifyPropertyChanged
    {
        #region Fields

        private readonly bool _blink;
        private readonly int _waitTimeSetting;
        private readonly DispatcherTimer _clockTimer;
        private readonly SolidColorBrush _black;
        private readonly SolidColorBrush _transparent;
        private readonly SolidColorBrush _color;
        private SolidColorBrush _foreColor;
        private Operation _operation;
        private bool _odd;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets/sets the foregroundcolor of the texts.
        /// </summary>
        public SolidColorBrush ForeColor
        {
            get { return _foreColor; }
            set
            {
                if (!Equals(value, _foreColor))
                {
                    _foreColor = value;
                    OnPropertyChanged("ForeColor");
                }
            }
        }

        /// <summary>
        /// Gets/sets the current time.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Gets/sets the timespan between the alarm time and the current time.
        /// </summary>
        public TimeSpan AlarmTime { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Provides a ClockWidget
        /// </summary>
        public ClockWidget()
        {
            DataContext = this;

            using (var service = ServiceFactory.GetCallbackServiceWrapper<ISettingsService>(new SettingsServiceCallback()))
            {
                object colorString = ColorConverter.ConvertFromString(service.Instance.GetSetting(SettingKeys.Color).GetValue<string>());
                if (colorString != null)
                {
                    _color = new SolidColorBrush((Color)colorString);
                }
                else
                {
                    _color = new SolidColorBrush(Colors.Red);
                }

                _waitTimeSetting = service.Instance.GetSetting(SettingKeys.WaitTime).GetValue<int>();
                _blink = service.Instance.GetSetting(SettingKeys.Blink).GetValue<bool>();
            }

            _black = new SolidColorBrush(Colors.Black);
            _transparent = new SolidColorBrush(Colors.Transparent);
            _black.Freeze();
            _color.Freeze();
            _transparent.Freeze();

            InitializeComponent();

            ForeColor = _black;
            _clockTimer = new DispatcherTimer();
            _clockTimer.Interval = TimeSpan.FromSeconds(0.5);
            _clockTimer.Tick += ClockTimer_Tick;
        }

        #endregion Constructors

        #region Event handlers

        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            Time = DateTime.Now;
            if (_operation != null)
            {
                TimeSpan timeSpan = Time - _operation.Timestamp;
                AlarmTime = timeSpan;
                if (_blink && timeSpan.TotalMinutes > _waitTimeSetting)
                {
                    if (_odd)
                    {
                        ForeColor = _transparent;
                        _odd = false;
                    }
                    else
                    {
                        ForeColor = _color;
                        _odd = true;
                    }
                }
                else
                {
                    ForeColor = _black;
                }
            }
            UpdateProperties();
        }

        private void UpdateProperties()
        {
            OnPropertyChanged("Time");
            OnPropertyChanged("AlarmTime");
        }

        #endregion

        #region IUIWidget Member

        bool IUIWidget.Initialize()
        {
            return true;
        }

        void IUIWidget.OnOperationChange(Operation operation)
        {
            if (operation == null)
            {
                if (_clockTimer.IsEnabled)
                {
                    _clockTimer.Stop();
                }
                return;
            }
            _operation = operation;

            if (!_clockTimer.IsEnabled)
            {
                _clockTimer.Start();
            }
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

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when the value of a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Manually raises the PropertyChanged event for the given property.
        /// </summary>
        /// <param name="propertyName">The name of the property to raise this event for.</param>
        protected void OnPropertyChanged(string name)
        {
            var copy = PropertyChanged;
            if (copy != null)
            {
                copy(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}