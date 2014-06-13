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
using System.Windows.Controls;
using System.Windows.Threading;

namespace AlarmWorkflow.Windows.UI.Views
{
    /// <summary>
    /// Interaction logic for ContentAlarmsAvailableControl.xaml
    /// </summary>
    public partial class ContentAlarmsAvailableControl : UserControl
    {
        #region Constants

        private const double MouseMoveTimerInterval = 10.0d;

        #endregion

        #region Fields

        private DispatcherTimer _mouseMoveTimer;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentAlarmsAvailableControl"/> class.
        /// </summary>
        public ContentAlarmsAvailableControl()
        {
            InitializeComponent();

            _mouseMoveTimer = new DispatcherTimer(DispatcherPriority.Background, this.Dispatcher);
            _mouseMoveTimer.Interval = TimeSpan.FromSeconds(MouseMoveTimerInterval);
            _mouseMoveTimer.Tick += _mouseMoveTimer_Tick;

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        #endregion

        #region Methods

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (App.GetApp().Configuration.AvoidScreensaver)
            {
                _mouseMoveTimer.Start();
                Helper.SetDisplayModeRequired(true);
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;

            _mouseMoveTimer.Stop();
        }

        private void _mouseMoveTimer_Tick(object sender, EventArgs e)
        {
            AlarmWorkflow.Windows.UI.Helper.POINT currentPos;
            if (Helper.GetCursorPos(out currentPos))
            {
                Helper.SetCursorPos(currentPos.X + 1, currentPos.Y);
                Helper.SetCursorPos(currentPos.X, currentPos.Y);
            }
        }

        #endregion
    }
}