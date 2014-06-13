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
using System.Collections.Generic;
using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ConfigurationContracts;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    /// <summary>
    /// Interaction logic for DurationTypeEditor.xaml
    /// </summary>
    [Export("DurationTypeEditor", typeof(ITypeEditor))]
    public partial class DurationTypeEditor : UserControl, ITypeEditor
    {
        #region Fields

        private DurationTypeEditorViewModel _viewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DurationTypeEditor"/> class.
        /// </summary>
        public DurationTypeEditor()
        {
            InitializeComponent();

            _viewModel = new DurationTypeEditorViewModel();
            this.DataContext = _viewModel;
        }

        #endregion

        #region ITypeEditor Members

        /// <summary>
        /// Gets/sets the value that is edited.
        /// </summary>
        public object Value
        {
            get { return _viewModel.Duration; }
            set { _viewModel.Duration = (int)value; }
        }

        /// <summary>
        /// Gets the visual element that is editing the value.
        /// </summary>
        public System.Windows.UIElement Visual
        {
            get { return this; }
        }

        void ITypeEditor.Initialize(string editorParameter)
        {
            if (string.IsNullOrWhiteSpace(editorParameter))
            {
                return;
            }

            IDictionary<string, string> options = OptionStringHelper.GetAsPairs(editorParameter);
            string minRaw = options.SafeGetValue("Min", null);
            if (minRaw != null)
            {
                _viewModel.DurationMin = int.Parse(minRaw);
            }

            string maxRaw = options.SafeGetValue("Max", null);
            if (maxRaw != null)
            {
                _viewModel.DurationMax = int.Parse(maxRaw);
            }

            string scaleRaw = options.SafeGetValue("Scale", DurationUnit.Milliseconds.ToString());
            DurationUnit scale = DurationUnit.Milliseconds;
            Enum.TryParse<DurationUnit>(scaleRaw, out scale);
            _viewModel.UnderlyingDurationScale = scale;
        }

        #endregion

        #region Methods

        private void PART_OpenPopup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!PART_Popup.IsOpen)
            {
                PART_Popup.IsOpen = true;
                PART_Popup.Focus();
            }
            else
            {
                PART_Popup.IsOpen = false;
            }
        }

        private void PART_Popup_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (e.OriginalSource != PART_Popup)
            {
                return;
            }
            PART_Popup.IsOpen = false;
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Specifies the duration unit used in the editor.
        /// </summary>
        public enum DurationUnit
        {
            /// <summary>
            /// Enumeration default value. Don't use.
            /// </summary>
            Invalid = 0,
            /// <summary>
            /// Unit is ms.
            /// </summary>
            Milliseconds,
            /// <summary>
            /// Unit is S.
            /// </summary>
            Seconds,
            /// <summary>
            /// Unit is M.
            /// </summary>
            Minutes,
            /// <summary>
            /// Unit is H.
            /// </summary>
            Hours,
            /// <summary>
            /// Unit is D.
            /// </summary>
            Days,
        }

        class DurationTypeEditorViewModel : ViewModelBase
        {
            #region Fields

            private TimeSpan _duration;

            #endregion

            #region Properties

            /// <summary>
            /// Gets/sets the duration in the underlying setting's scale.
            /// Depending on the setting, this may be in milliseconds, seconds, minutes etc.
            /// </summary>
            public int Duration
            {
                get { return GetTimeInUnderlyingUnit(_duration); }
                set { SetDurationProperties(value); }
            }

            /// <summary>
            /// Gets/sets the inclusive minimum of the duration.
            /// </summary>
            public int? DurationMin { get; set; }
            /// <summary>
            /// Gets/sets the inclusive maximum of the duration.
            /// </summary>
            public int? DurationMax { get; set; }

            /// <summary>
            /// Gets the underlying duration scale.
            /// </summary>
            public DurationUnit UnderlyingDurationScale { get; set; }

            /// <summary>
            /// Gets/sets the milliseconds-part of the <see cref="Duration"/>.
            /// </summary>
            public int Milliseconds
            {
                get { return _duration.Milliseconds; }
                set { SetTimeSpan(this.Days, this.Hours, this.Minutes, this.Seconds, value); }
            }

            /// <summary>
            /// Gets/sets the seconds-part of the <see cref="Duration"/>.
            /// </summary>
            public int Seconds
            {
                get { return _duration.Seconds; }
                set { SetTimeSpan(this.Days, this.Hours, this.Minutes, value, this.Milliseconds); }
            }

            /// <summary>
            /// Gets/sets the minutes-part of the <see cref="Duration"/>.
            /// </summary>
            public int Minutes
            {
                get { return _duration.Minutes; }
                set { SetTimeSpan(this.Days, this.Hours, value, this.Seconds, this.Milliseconds); }
            }

            /// <summary>
            /// Gets/sets the hours-part of the <see cref="Duration"/>.
            /// </summary>
            public int Hours
            {
                get { return _duration.Hours; }
                set { SetTimeSpan(this.Days, value, this.Minutes, this.Seconds, this.Milliseconds); }
            }

            /// <summary>
            /// Gets/sets the days-part of the <see cref="Duration"/>.
            /// </summary>
            public int Days
            {
                get { return _duration.Days; }
                set { SetTimeSpan(value, this.Hours, this.Minutes, this.Seconds, this.Milliseconds); }
            }

            #endregion

            #region Methods

            private int GetTimeInUnderlyingUnit(TimeSpan timeSpan)
            {
                switch (UnderlyingDurationScale)
                {
                    case DurationUnit.Seconds: return (int)timeSpan.TotalSeconds;
                    case DurationUnit.Minutes: return (int)timeSpan.TotalMinutes;
                    case DurationUnit.Hours: return (int)timeSpan.TotalHours;
                    case DurationUnit.Days: return (int)timeSpan.TotalDays;
                    default:
                    case DurationUnit.Milliseconds: return (int)timeSpan.TotalMilliseconds;
                }
            }

            private void SetDurationProperties(int value)
            {
                TimeSpan ts = new TimeSpan();
                switch (UnderlyingDurationScale)
                {
                    case DurationUnit.Milliseconds:
                        ts = TimeSpan.FromMilliseconds(value);
                        break;
                    case DurationUnit.Seconds:
                        ts = TimeSpan.FromSeconds(value);
                        break;
                    case DurationUnit.Minutes:
                        ts = TimeSpan.FromMinutes(value);
                        break;
                    case DurationUnit.Hours:
                        ts = TimeSpan.FromHours(value);
                        break;
                    case DurationUnit.Days:
                        ts = TimeSpan.FromDays(value);
                        break;
                    default:
                        throw new InvalidOperationException("Invalid case!");
                }

                AssertBoundaries(ts);
                _duration = ts;

                UpdateTimeProperties();
            }

            private void SetTimeSpan(int days, int hours, int minutes, int seconds, int milliseconds)
            {
                TimeSpan proposed = new TimeSpan(days, hours, minutes, seconds, milliseconds);
                AssertBoundaries(proposed);

                _duration = proposed;
                UpdateTimeProperties();
            }

            private void AssertBoundaries(TimeSpan proposed)
            {
                int allowedMin = this.DurationMin.HasValue ? this.DurationMin.Value : int.MinValue;
                int allowedMax = this.DurationMax.HasValue ? this.DurationMax.Value : int.MaxValue;

                int durationTmp = GetTimeInUnderlyingUnit(proposed);
                if ((this.DurationMin.HasValue && (durationTmp < this.DurationMin.Value)) ||
                    (this.DurationMax.HasValue && (durationTmp > this.DurationMax.Value)))
                {
                    ThrowOutOfRange(allowedMin, allowedMax, durationTmp);
                }
            }

            private void ThrowOutOfRange(int allowedMin, int allowedMax, int value)
            {
                throw new ArgumentOutOfRangeException("Duration", value, string.Format(Properties.Resources.DurationTypeEditorOutsideBoundaries, allowedMin, allowedMax, UnderlyingDurationScale));
            }

            private void UpdateTimeProperties()
            {
                OnPropertyChanged("Milliseconds");
                OnPropertyChanged("Seconds");
                OnPropertyChanged("Minutes");
                OnPropertyChanged("Hours");
                OnPropertyChanged("Days");

                OnPropertyChanged("Duration");
            }

            #endregion
        }

        #endregion
    }
}