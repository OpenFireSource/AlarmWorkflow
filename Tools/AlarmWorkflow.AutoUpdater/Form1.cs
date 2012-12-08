using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using AlarmWorkflow.Tools.AutoUpdater.Tasks;

namespace AlarmWorkflow.Tools.AutoUpdater
{
    /// <summary>
    /// Interaction logic for Form1.
    /// </summary>
    public partial class Form1 : Form
    {
        #region Fields

        private Version _localVersion;
        private Version _serverVersion;

        private InstallOptions _options;
        private List<ITask> _tasks;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            _options = new InstallOptions();
            this.pgOptions.SelectedObject = _options;

            _tasks = new List<ITask>();

            Log.PostText += Log_PostText;
        }

        #endregion

        #region Methods

        private void Log_PostText(string text)
        {
            txtLog.Invoke((Action)(() =>
            {
                txtLog.AppendText(text + Environment.NewLine);
                txtLog.ScrollToCaret();
            }));
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            UpdateVersionDisplay();
        }

        private void UpdateVersionDisplay()
        {
            RetrieveLocalVersion();
            ThreadPool.QueueUserWorkItem(o => RetrieveServerVersion());
        }

        private void RetrieveLocalVersion()
        {
            _localVersion = VersioningHelper.GetLocalVersion();
            this.lblLocalVersion.Text = _localVersion.ToString();

            Log.Write("Current local version is: {0}", _localVersion);
        }

        private void RetrieveServerVersion()
        {
            this.lnkUpdate.Invoke((Action)(() =>
            {
                lnkUpdate.Enabled = false;
            }));

            _serverVersion = VersioningHelper.GetServerVersion();
            Log.Write("Current server version is: {0}", _serverVersion);

            this.lblCurrentVersion.Invoke((Action)(() =>
            {
                lblCurrentVersion.Text = _serverVersion.ToString();
            }));

            this.lnkUpdate.Invoke((Action)(() =>
            {
                lnkUpdate.Enabled = true;
            }));
        }

        private void lnkUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_serverVersion == null || _localVersion == null)
            {
                return;
            }

            bool isNewerVersion = _serverVersion > _localVersion;
            OfferUpdate(!isNewerVersion);
        }

        private void SetEnableFormForInput(bool state)
        {
            pgOptions.Enabled = state;
            lnkUpdate.Enabled = state;
        }

        private void OfferUpdate(bool force)
        {
            if (bwUpdateProcess.IsBusy)
            {
                return;
            }

            if (force)
            {
                if (!Utilities.ConfirmMessageBox(Properties.Resources.OfferForceUpdateMessage))
                {
                    return;
                }
            }

            if (!Utilities.ConfirmMessageBox(Properties.Resources.ConfirmUpdateMessage))
            {
                return;
            }

            SetEnableFormForInput(false);

            _tasks.Clear();

            // Add selected tasks
            if (_options.AutomaticServiceUnInstall)
            {
                _tasks.Add(new Tasks.StartStopServiceTask());
            }
            if (_options.KillAlarmWorkflowProcesses)
            {
                _tasks.Add(new Tasks.StopProcessesTask());
            }
            if (_options.DownloadCuneiform)
            {
                _tasks.Add(new Tasks.DownloadCuneiformTask());
            }
            _tasks.Add(new Tasks.DownloadUpdatePackageTask());

            bwUpdateProcess.RunWorkerAsync();
        }

        private void bwUpdateProcess_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            TaskArgs args = new TaskArgs();
            args.Context["InstalllOptions"] = _options;
            args.Context["LocalVersion"] = _localVersion;
            args.Context["ServerVersion"] = _serverVersion;

            args.Action = TaskArgs.TaskAction.Pre;
            ExecuteTasks(args);

            args.Action = TaskArgs.TaskAction.Installation;
            ExecuteTasks(args);

            args.Action = TaskArgs.TaskAction.Post;
            ExecuteTasks(args);
        }

        private void bwUpdateProcess_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            // TODO: Handle the following states with user interaction!
            if (e.Cancelled)
            {
            }
            if (e.Error != null)
            {
                Utilities.ShowMessageBox(MessageBoxIcon.Error, Properties.Resources.UpdateFailedWithExceptionMessage, e.Error.Message);
            }
            else
            {
                Utilities.ShowMessageBox(MessageBoxIcon.Information, Properties.Resources.UpdateCompleteMessage);
            }

            SetEnableFormForInput(true);
            UpdateVersionDisplay();
        }

        private void ExecuteTasks(TaskArgs args)
        {
            foreach (ITask task in _tasks)
            {
                Log.Write("");

                Stopwatch sw = Stopwatch.StartNew();
                string taskName = task.GetType().Name;
                try
                {
                    Log.Write("Executing task '{0}' (in phase '{1}')...", taskName, args.Action);

                    task.Execute(args);

                    sw.Stop();
                    Log.Write("Task finished in '{0}' milliseconds.", sw.ElapsedMilliseconds);
                }
                catch (Exception)
                {
                    // A failing task is always critical
                    Log.Write("Failed at task '{0}'!", taskName);

                    throw;
                }
            }
        }

        #endregion
    }
}
