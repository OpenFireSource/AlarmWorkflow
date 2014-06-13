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
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace AlarmWorkflow.Windows.UIContracts.ViewModels
{
    /// <summary>
    /// Represents a command which triggers a long-running operation and notifies consumers about the progress.
    /// </summary>
    public abstract class BackgroundTaskCommand : ICommand, INotifyPropertyChanged
    {
        #region Fields

        private readonly object _lock = new object();

        private BackgroundTaskState _state;
        private int _progress;
        private string _progressText;

        private bool _isSTAThread;
        private bool _isBackgroundThread;
        private Thread _longRunningTask;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the state of the underlying task.
        /// </summary>
        public BackgroundTaskState State
        {
            get { return _state; }
            private set
            {
                if (value != _state)
                {
                    _state = value;
                    OnPropertyChangedDispatched("State");
                    OnPropertyChangedDispatched("IsRunning");
                }
            }
        }
        /// <summary>
        /// Gets the progress of the underlying task.
        /// </summary>
        public int Progress
        {
            get { return _progress; }
            protected set
            {
                if (value != _progress)
                {
                    _progress = value;
                    OnPropertyChangedDispatched("Progress");
                }
            }
        }
        /// <summary>
        /// Gets some text describing the current progress in more detail.
        /// This can be used for displaying in a statusbar or such.
        /// </summary>
        public string ProgressText
        {
            get { return _progressText; }
            protected set
            {
                if (value != _progressText)
                {
                    _progressText = value;
                    OnPropertyChangedDispatched("ProgressText");
                }
            }
        }
        /// <summary>
        /// Gets/sets the timeout to use when exiting the underlying task.
        /// This timeout should not be set too high.
        /// </summary>
        protected TimeSpan TaskJoinTimeout { get; set; }
        /// <summary>
        /// Gets/sets whether an exception that was thrown during Execute() should be swallowed or re-thrown.
        /// </summary>
        protected bool IgnoreExceptionDuringExecute { get; set; }
        /// <summary>
        /// Gets whether or not the operation should be canceled at the next-best occassion.
        /// </summary>
        protected bool IsCancelRequested { get; private set; }
        /// <summary>
        /// Gets a dictionary that can be used to store command-specific data or return values for later use.
        /// </summary>
        public IDictionary<string, object> Parameters { get; private set; }

        /// <summary>
        /// Determines whether or not the underlying task is currently in progress.
        /// </summary>
        public bool IsRunning
        {
            get { return State == BackgroundTaskState.Running; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundTaskCommand"/> class
        /// and uses an STA-thread that runs in the background.
        /// </summary>
        public BackgroundTaskCommand()
            : this(true, true)
        {
            TaskJoinTimeout = TimeSpan.FromSeconds(5.0d);
            Application.Current.Exit += Current_Exit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundTaskCommand"/> class with advanced options.
        /// </summary>
        /// <param name="isSTAThread">Whether or not the underlying thread shall be marked as being an STA thread instead of an MTA thread.</param>
        /// <param name="isBackgroundThread">Whether or not the underlying thread shall be declared as a background thread.</param>
        public BackgroundTaskCommand(bool isSTAThread, bool isBackgroundThread)
        {
            Parameters = new Dictionary<string, object>();

            _isSTAThread = isSTAThread;
            _isBackgroundThread = isBackgroundThread;
        }

        #endregion

        #region Methods

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            Application.Current.Exit -= Current_Exit;
            if (_longRunningTask == null)
            {
                return;
            }

            Debug.Print("[Task] Signalling cancel to thread due to shutdown...");

            IsCancelRequested = true;
            bool result = _longRunningTask.Join(TaskJoinTimeout);

            Debug.Print("[Task] Ended with result: {0}.", result);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/>-event within the safe execution of the current app's dispatcher.
        /// The property changes are dispatched using BeginInvoke().
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        protected void OnPropertyChangedDispatched(string propertyName)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(() => OnPropertyChanged(propertyName)));
            }
        }

        /// <summary>
        /// Convenience method to update the <see cref="ProgressText"/> with some specific text.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected void SetProgressText(string format, params object[] args)
        {
            ProgressText = string.Format(format, args);
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var copy = PropertyChanged;
            if (copy != null)
            {
                copy(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region ICommand Members

        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        void ICommand.Execute(object parameter)
        {
            lock (_lock)
            {
                if (IsRunning)
                {
                    return;
                }

                if (_longRunningTask != null)
                {
                    _longRunningTask = null;
                }

                _longRunningTask = new Thread(BeginTask);
                _longRunningTask.SetApartmentState(_isSTAThread ? ApartmentState.STA : ApartmentState.MTA);
                _longRunningTask.IsBackground = _isBackgroundThread;
                _longRunningTask.Priority = ThreadPriority.BelowNormal;
                _longRunningTask.Name = string.Format("{0}_ExecutionThread [{1}]", this.GetType().Name, Guid.NewGuid().ToString().Substring(0, 8));
                _longRunningTask.Start(parameter);
            }
        }

        private void BeginTask(object parameter)
        {
            try
            {
                Progress = 0;
                State = BackgroundTaskState.Running;

                Execute(parameter);

                State = BackgroundTaskState.Completed;
            }
            catch (Exception)
            {
                State = BackgroundTaskState.Faulted;
                if (!IgnoreExceptionDuringExecute)
                {
                    throw;
                }
            }
            finally
            {
                if (Application.Current != null)
                {
                    Application.Current.Dispatcher.BeginInvoke((Action)(() => Application.Current.Exit -= Current_Exit));
                }
            }
        }

        /// <summary>
        /// Performs the work to be done in a background thread.
        /// </summary>
        /// <param name="parameter"></param>
        protected abstract void Execute(object parameter);

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute(parameter);
        }

        /// <summary>
        /// Determines whether or not this command can be executed.
        /// The base implementation returns the negated result from <see cref="IsRunning"/>.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected virtual bool CanExecute(object parameter)
        {
            return !IsRunning;
        }

        #endregion
    }
}
