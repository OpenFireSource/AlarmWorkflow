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
using System.Drawing;
using System.ServiceProcess;
using System.Windows.Forms;

namespace AlarmWorkflow.Backend.Service.UI
{
    partial class ManagementForm : Form
    {
        #region Constructors

        public ManagementForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        private void ManagementForm_Load(object sender, EventArgs e)
        {
            UpdateUI();

            if (!ServiceHelper.IsCurrentUserAdministrator())
            {
                this.Text += " (no administrator)";
            }
        }

        private void tmrUpdateUI_Tick(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void btnServiceAction_Click(object sender, EventArgs e)
        {
            string tag = (string)((Button)sender).Tag;

            switch (tag)
            {
                case "install":
                    TryExecuteWithNotification(() => ServiceHelper.InstallService(), Properties.Resources.ServiceInstallFailedMessage);
                    break;
                case "uninstall":
                    TryExecuteWithNotification(() => ServiceHelper.UninstallService(), Properties.Resources.ServiceUninstallFailedMessage);
                    break;
                case "start":
                    TryExecuteWithNotification(() => ServiceHelper.StartService(), Properties.Resources.ServiceStartError);
                    break;
                case "stop":
                    TryExecuteWithNotification(() => ServiceHelper.StopService(), Properties.Resources.ServiceStopError);
                    break;
            }

            UpdateUI();
        }

        private static void TryExecuteWithNotification(Action action, string failMessageTemplate)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(failMessageTemplate, ex.Message), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateUI()
        {
            if (!ServiceHelper.IsCurrentUserAdministrator())
            {
                btnInstall.Enabled = false;
                btnUninstall.Enabled = false;
                btnStart.Enabled = false;
                btnStop.Enabled = false;
            }
            else
            {
                btnInstall.Enabled = !ServiceHelper.IsServiceInstalled();
                btnUninstall.Enabled = ServiceHelper.IsServiceInstalled();
                btnStart.Enabled = ServiceHelper.IsServiceInstalled() && !ServiceHelper.IsServiceRunning();
                btnStop.Enabled = ServiceHelper.IsServiceInstalled() && ServiceHelper.IsServiceRunning();
            }

            UpdateStatusLabel();
        }

        private void UpdateStatusLabel()
        {
            Color statusColor = Color.Black;
            FontStyle statusStyle = FontStyle.Bold;

            try
            {
                ServiceControllerStatus state = ServiceHelper.GetServiceState();

                /* The service does not support pausing and continuing. Thus those are omitted.
                 */
                switch (state)
                {
                    case ServiceControllerStatus.StartPending:
                        statusColor = Color.Green;
                        statusStyle = FontStyle.Italic;
                        break;
                    case ServiceControllerStatus.Running:
                        statusColor = Color.Green;
                        statusStyle = FontStyle.Bold;
                        break;
                    case ServiceControllerStatus.StopPending:
                        statusColor = Color.OrangeRed;
                        statusStyle = FontStyle.Italic;
                        break;
                    case ServiceControllerStatus.Stopped:
                        statusColor = Color.Red;
                        statusStyle = FontStyle.Bold;
                        break;
                    default:
                        break;
                }

                lblStatus.Text = state.ToString();
            }
            catch (Exception)
            {
                lblStatus.Text = Properties.Resources.ServiceNotInstalledStatus;
            }

            lblStatus.ForeColor = statusColor;
            lblStatus.Font = new Font(lblStatus.Font, statusStyle);
        }

        #endregion
    }
}
