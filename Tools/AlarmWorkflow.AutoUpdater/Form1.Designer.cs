namespace AlarmWorkflow.Tools.AutoUpdater
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblLocalVersion = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCurrentVersion = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lnkUpdate = new System.Windows.Forms.LinkLabel();
            this.prgProgress = new System.Windows.Forms.ProgressBar();
            this.bwDownloadUpdatePackage = new System.ComponentModel.BackgroundWorker();
            this.chkAutoInstallService = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblLocalVersion
            // 
            this.lblLocalVersion.AutoSize = true;
            this.lblLocalVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLocalVersion.Location = new System.Drawing.Point(231, 29);
            this.lblLocalVersion.Name = "lblLocalVersion";
            this.lblLocalVersion.Size = new System.Drawing.Size(49, 13);
            this.lblLocalVersion.TabIndex = 3;
            this.lblLocalVersion.Text = "Prüfe...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(76, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Lokale Version:";
            // 
            // lblCurrentVersion
            // 
            this.lblCurrentVersion.AutoSize = true;
            this.lblCurrentVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentVersion.Location = new System.Drawing.Point(231, 71);
            this.lblCurrentVersion.Name = "lblCurrentVersion";
            this.lblCurrentVersion.Size = new System.Drawing.Size(49, 13);
            this.lblCurrentVersion.TabIndex = 5;
            this.lblCurrentVersion.Text = "Prüfe...";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(76, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Aktuelle Version:";
            // 
            // lnkUpdate
            // 
            this.lnkUpdate.AutoSize = true;
            this.lnkUpdate.Enabled = false;
            this.lnkUpdate.Location = new System.Drawing.Point(231, 101);
            this.lnkUpdate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lnkUpdate.Name = "lnkUpdate";
            this.lnkUpdate.Size = new System.Drawing.Size(67, 13);
            this.lnkUpdate.TabIndex = 7;
            this.lnkUpdate.TabStop = true;
            this.lnkUpdate.Text = "&Aktualisieren";
            this.lnkUpdate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkUpdate_LinkClicked);
            // 
            // prgProgress
            // 
            this.prgProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prgProgress.Location = new System.Drawing.Point(9, 207);
            this.prgProgress.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.prgProgress.Name = "prgProgress";
            this.prgProgress.Size = new System.Drawing.Size(352, 19);
            this.prgProgress.TabIndex = 8;
            // 
            // bwDownloadUpdatePackage
            // 
            this.bwDownloadUpdatePackage.WorkerReportsProgress = true;
            this.bwDownloadUpdatePackage.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwDownloadUpdatePackage_DoWork);
            this.bwDownloadUpdatePackage.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwDownloadUpdatePackage_ProgressChanged);
            this.bwDownloadUpdatePackage.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwDownloadUpdatePackage_RunWorkerCompleted);
            // 
            // chkAutoInstallService
            // 
            this.chkAutoInstallService.AutoSize = true;
            this.chkAutoInstallService.Location = new System.Drawing.Point(9, 143);
            this.chkAutoInstallService.Name = "chkAutoInstallService";
            this.chkAutoInstallService.Size = new System.Drawing.Size(174, 17);
            this.chkAutoInstallService.TabIndex = 9;
            this.chkAutoInstallService.Text = "Service automatisch &installieren";
            this.chkAutoInstallService.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 236);
            this.Controls.Add(this.chkAutoInstallService);
            this.Controls.Add(this.prgProgress);
            this.Controls.Add(this.lnkUpdate);
            this.Controls.Add(this.lblCurrentVersion);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblLocalVersion);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Auto-Updater";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLocalVersion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCurrentVersion;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel lnkUpdate;
        private System.Windows.Forms.ProgressBar prgProgress;
        private System.ComponentModel.BackgroundWorker bwDownloadUpdatePackage;
        private System.Windows.Forms.CheckBox chkAutoInstallService;
    }
}

