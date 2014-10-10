namespace AlarmWorkflow.Backend.Service.UI
{
    partial class ManagementForm
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnInstall = new System.Windows.Forms.Button();
            this.tmrUpdateUI = new System.Windows.Forms.Timer(this.components);
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnUninstall = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Status:";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(96, 9);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 1;
            // 
            // btnInstall
            // 
            this.btnInstall.Location = new System.Drawing.Point(12, 54);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(75, 63);
            this.btnInstall.TabIndex = 2;
            this.btnInstall.Tag = "install";
            this.btnInstall.Text = "&Install";
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new System.EventHandler(this.btnServiceAction_Click);
            // 
            // tmrUpdateUI
            // 
            this.tmrUpdateUI.Enabled = true;
            this.tmrUpdateUI.Interval = 2000;
            this.tmrUpdateUI.Tick += new System.EventHandler(this.tmrUpdateUI_Tick);
            // 
            // btnStart
            // 
            this.btnStart.Image = global::AlarmWorkflow.Backend.Service.Properties.Resources.servicerunning;
            this.btnStart.Location = new System.Drawing.Point(110, 54);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 63);
            this.btnStart.TabIndex = 3;
            this.btnStart.Tag = "start";
            this.btnStart.Text = "&Start";
            this.btnStart.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnStart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnServiceAction_Click);
            // 
            // btnStop
            // 
            this.btnStop.Image = global::AlarmWorkflow.Backend.Service.Properties.Resources.servicestopped;
            this.btnStop.Location = new System.Drawing.Point(191, 54);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 63);
            this.btnStop.TabIndex = 4;
            this.btnStop.Tag = "stop";
            this.btnStop.Text = "Sto&p";
            this.btnStop.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnServiceAction_Click);
            // 
            // btnUninstall
            // 
            this.btnUninstall.Location = new System.Drawing.Point(293, 54);
            this.btnUninstall.Name = "btnUninstall";
            this.btnUninstall.Size = new System.Drawing.Size(75, 63);
            this.btnUninstall.TabIndex = 5;
            this.btnUninstall.Tag = "uninstall";
            this.btnUninstall.Text = "&Uninstall";
            this.btnUninstall.UseVisualStyleBackColor = true;
            this.btnUninstall.Click += new System.EventHandler(this.btnServiceAction_Click);
            // 
            // ManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 139);
            this.Controls.Add(this.btnUninstall);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnInstall);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManagementForm";
            this.Text = "AlarmWorkflow Service Manager";
            this.Load += new System.EventHandler(this.ManagementForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnInstall;
        private System.Windows.Forms.Timer tmrUpdateUI;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnUninstall;
    }
}