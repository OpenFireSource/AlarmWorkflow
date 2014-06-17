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
using System.Diagnostics;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Properties;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Represents a wrapper around a <see cref="Process"/> that allows for convenient running a process and listening to its StdOut/StdErr to log them.
    /// </summary>
    public sealed class ProcessWrapper : DisposableObject
    {
        #region Fields

        private Process _process;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the working directory for the process to be started. 
        /// </summary>
        public string WorkingDirectory
        {
            get { return _process.StartInfo.WorkingDirectory; }
            set { _process.StartInfo.WorkingDirectory = value; }
        }

        /// <summary>
        /// Gets/Sets the name or path of the application to start.
        /// </summary>
        public string FileName
        {
            get { return _process.StartInfo.FileName; }
            set { _process.StartInfo.FileName = value; }
        }

        /// <summary>
        /// Gets/Sets the arguments that have to be used when starting the given application.
        /// </summary>
        public string Arguments
        {
            get { return _process.StartInfo.Arguments; }
            set { _process.StartInfo.Arguments = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessWrapper"/> class.
        /// </summary>
        public ProcessWrapper()
        {
            _process = new Process();

            _process.EnableRaisingEvents = true;
            _process.StartInfo.RedirectStandardError = true;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.CreateNoWindow = true;

            _process.OutputDataReceived += _process_OutputDataReceived;
            _process.ErrorDataReceived += _process_ErrorDataReceived;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the process, while reading the output and error events from the process.
        /// </summary>
        /// <exception cref="ObjectDisposedException">This instance has been disposed of and cannot be used anymore.</exception>
        public void Start()
        {
            Start(false);
        }

        /// <summary>
        /// Starts the process and waits for completion, while reading the output and error events from the process.
        /// </summary>
        /// <exception cref="ObjectDisposedException">This instance has been disposed of and cannot be used anymore.</exception>
        public void StartAndWait()
        {
            Start(true);
        }

        private void Start(bool wait)
        {
            AssertNotDisposed();

            if (!wait)
            {
                _process.Exited += _process_Exited;
            }

            try
            {
                _process.Start();
                _process.BeginErrorReadLine();
                _process.BeginOutputReadLine();

                Logger.Instance.LogFormat(LogType.Trace, this, Resources.ProgramStart, FileName, Arguments);

                if (wait)
                {
                    _process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Resources.ProcessWrapperStartError, _process.StartInfo.FileName);
                Logger.Instance.LogException(this, ex);

                if (wait)
                {
                    this.Dispose();
                }
            }
        }

        private void _process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                return;
            }

            Logger.Instance.LogFormat(LogType.Trace, this, Resources.ProcessDataEvent, e.Data);
        }

        private void _process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                return;
            }

            Logger.Instance.LogFormat(LogType.Error, this, Resources.ProcessErrorEvent, e.Data);
        }

        private void _process_Exited(object sender, EventArgs e)
        {
            Logger.Instance.LogFormat(LogType.Trace, this, Resources.ProgramFinished, FileName, _process.ExitCode);

            this.Dispose();
        }

        /// <summary>
        /// Overridden to cleanup the used process.
        /// </summary>
        protected override void DisposeCore()
        {
            if (_process != null)
            {
                _process.Exited -= _process_Exited;

                _process.OutputDataReceived -= _process_OutputDataReceived;
                _process.ErrorDataReceived -= _process_ErrorDataReceived;

                _process.Dispose();
                _process = null;
            }
        }

        #endregion
    }
}
