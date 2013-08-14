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
using System.Windows.Threading;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.UI.Windows
{
    /// <summary>
    /// Interaction logic for AcknowledgeOperationDialog.xaml
    /// </summary>
    internal partial class AcknowledgeOperationDialog : Window
    {
        #region Constants

        private const int CancelTimeSeconds = 5;

        #endregion

        #region Fields

        private DispatcherTimer _cancelTimer;
        private ViewModel _viewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AcknowledgeOperationDialog"/> class.
        /// </summary>
        public AcknowledgeOperationDialog()
        {
            InitializeComponent();
            if (App.Current.MainWindow != null)
            {
                this.Owner = App.Current.MainWindow;
            }

            _viewModel = new ViewModel();
            _viewModel.Time = CancelTimeSeconds;
            this.DataContext = _viewModel;

            _cancelTimer = new DispatcherTimer(TimeSpan.FromSeconds(1.0d), DispatcherPriority.Normal, CancelTimerTick, this.Dispatcher);
            _cancelTimer.Start();
        }

        #endregion

        #region Event handlers

        private void CancelTimerTick(object sender, EventArgs e)
        {
            _viewModel.Time--;
            if (_viewModel.Time == 0)
            {
                this.DialogResult = false;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// Overridden to stop the automatic timer.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            _cancelTimer.Tick -= CancelTimerTick;
            _cancelTimer.Stop();
            _cancelTimer = null;
        }

        #endregion

        #region Nested types

        class ViewModel : ViewModelBase
        {
            #region Fields

            private int _time;

            #endregion

            #region Properties

            /// <summary>
            /// Gets/sets the time that is left until the dialog will be automatically cancelled.
            /// </summary>
            public int Time
            {
                get { return _time; }
                set
                {
                    _time = value;
                    OnPropertyChanged("Time");
                }
            }

            #endregion

        }

        #endregion
    }
}
