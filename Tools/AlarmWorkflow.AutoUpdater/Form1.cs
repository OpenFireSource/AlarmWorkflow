using System;
using System.Threading;
using System.Windows.Forms;

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

            // If the versions differ, offer the possibility to update
            if (_serverVersion > _localVersion)
            {
                this.btnUpdate.Invoke((Action)(() => btnUpdate.Enabled = true));
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

        }

        #endregion
    }
}
