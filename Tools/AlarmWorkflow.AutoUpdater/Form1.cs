using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Ionic.Zip;

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

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            RetrieveLocalVersion();
            ThreadPool.QueueUserWorkItem(o => RetrieveServerVersion());
        }

        private void RetrieveLocalVersion()
        {
            _localVersion = VersioningHelper.GetLocalVersion();
            this.lblLocalVersion.Text = _localVersion.ToString();
        }

        private void RetrieveServerVersion()
        {
            _serverVersion = VersioningHelper.GetServerVersion();

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

        private void OfferUpdate(bool force)
        {
            if (bwDownloadUpdatePackage.IsBusy)
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

            bwDownloadUpdatePackage.RunWorkerAsync();
        }

        private void bwDownloadUpdatePackage_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            // Make the async operation synchronous to keep the BackgroundWorker busy
            ManualResetEventSlim waitHandle = new ManualResetEventSlim(false);

            using (WebClient client = new WebClient())
            {

                string serverVersionUri = string.Format("{0}/{1}/{2}", Properties.Settings.Default.UpdateServerName, Properties.Settings.Default.UpdateFilesDirectory, _serverVersion.ToString() + ".zip");

                client.DownloadProgressChanged += (oa, ea) =>
                {
                    bwDownloadUpdatePackage.ReportProgress(ea.ProgressPercentage);
                };
                client.DownloadDataCompleted += (oa, ea) =>
                {
                    e.Result = ea;
                    waitHandle.Set();
                };

                client.DownloadDataAsync(new Uri(serverVersionUri));
            }

            waitHandle.Wait();
        }

        private void bwDownloadUpdatePackage_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            this.prgProgress.Value = e.ProgressPercentage;
        }

        private void bwDownloadUpdatePackage_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            this.prgProgress.Value = 100;

            DownloadDataCompletedEventArgs args = e.Result as DownloadDataCompletedEventArgs;
            if (args == null)
            {
                this.prgProgress.Value = 0;
                return;
            }

            // TODO: Handle the following states with user interaction!
            if (args.Cancelled)
            {
                return;
            }
            if (args.Error != null)
            {
                WebException we = args.Error as WebException;
                if (we != null)
                {
                    Utilities.ShowMessageBox(MessageBoxIcon.Error, Properties.Resources.UpdateFailedWithExceptionMessage, we.Message);
                }
                return;
            }
            if (args.Result == null || args.Result.Length == 0)
            {
                return;
            }

            ExtractZipFile(args.Result);

            if (chkAutoInstallService.Checked)
            {
                InstallService();
            }
        }

        private void ExtractZipFile(byte[] buffer)
        {
            string tempFileName = Path.GetTempFileName();
            ZipFile zipFile = ZipFile.Read(buffer);
            zipFile.ExtractAll(Application.StartupPath, ExtractExistingFileAction.OverwriteSilently);

        }

        private void InstallService()
        {
            ProcessStartInfo serviceInstall = new ProcessStartInfo();
            serviceInstall.CreateNoWindow = false;
            serviceInstall.FileName = Application.StartupPath + "\\AlarmWorkflow.Windows.Service.exe";
            serviceInstall.Arguments = "--install";
            Process.Start(serviceInstall).WaitForExit();
        }

        #endregion


    }
}
