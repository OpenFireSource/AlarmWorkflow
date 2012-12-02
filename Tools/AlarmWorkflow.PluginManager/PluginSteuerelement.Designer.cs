namespace AlarmWorkflow.Tools.PluginManager
{
    partial class PluginSteuerelement
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.L_PluginName = new System.Windows.Forms.Label();
            this.RTB_Description = new System.Windows.Forms.RichTextBox();
            this.B_Download = new System.Windows.Forms.Button();
            this.B_Update = new System.Windows.Forms.Button();
            this.L_Name = new System.Windows.Forms.Label();
            this.L_PluginId = new System.Windows.Forms.Label();
            this.L_ID = new System.Windows.Forms.Label();
            this.L_PluginVersion = new System.Windows.Forms.Label();
            this.L_Version = new System.Windows.Forms.Label();
            this.L_PluginVersionInstalled = new System.Windows.Forms.Label();
            this.L_VersionInstalled = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // L_PluginName
            // 
            this.L_PluginName.AutoSize = true;
            this.L_PluginName.Location = new System.Drawing.Point(4, 10);
            this.L_PluginName.Name = "L_PluginName";
            this.L_PluginName.Size = new System.Drawing.Size(42, 13);
            this.L_PluginName.TabIndex = 0;
            this.L_PluginName.Text = "Plugin: ";
            // 
            // RTB_Description
            // 
            this.RTB_Description.Location = new System.Drawing.Point(7, 31);
            this.RTB_Description.Name = "RTB_Description";
            this.RTB_Description.ReadOnly = true;
            this.RTB_Description.Size = new System.Drawing.Size(639, 118);
            this.RTB_Description.TabIndex = 1;
            this.RTB_Description.Text = "";
            // 
            // B_Download
            // 
            this.B_Download.Location = new System.Drawing.Point(7, 155);
            this.B_Download.Name = "B_Download";
            this.B_Download.Size = new System.Drawing.Size(320, 22);
            this.B_Download.TabIndex = 2;
            this.B_Download.Text = "Download";
            this.B_Download.UseVisualStyleBackColor = true;
            this.B_Download.Click += new System.EventHandler(this.B_Download_Click);
            // 
            // B_Update
            // 
            this.B_Update.Location = new System.Drawing.Point(333, 155);
            this.B_Update.Name = "B_Update";
            this.B_Update.Size = new System.Drawing.Size(313, 22);
            this.B_Update.TabIndex = 3;
            this.B_Update.Text = "Update";
            this.B_Update.UseVisualStyleBackColor = true;
            this.B_Update.Click += new System.EventHandler(this.B_Update_Click);
            // 
            // L_Name
            // 
            this.L_Name.AutoSize = true;
            this.L_Name.Location = new System.Drawing.Point(43, 10);
            this.L_Name.Name = "L_Name";
            this.L_Name.Size = new System.Drawing.Size(73, 13);
            this.L_Name.TabIndex = 4;
            this.L_Name.Text = "[Plugin-Name]";
            // 
            // L_PluginId
            // 
            this.L_PluginId.AutoSize = true;
            this.L_PluginId.Location = new System.Drawing.Point(579, 10);
            this.L_PluginId.Name = "L_PluginId";
            this.L_PluginId.Size = new System.Drawing.Size(24, 13);
            this.L_PluginId.TabIndex = 5;
            this.L_PluginId.Text = "ID: ";
            // 
            // L_ID
            // 
            this.L_ID.AutoSize = true;
            this.L_ID.Location = new System.Drawing.Point(600, 10);
            this.L_ID.Name = "L_ID";
            this.L_ID.Size = new System.Drawing.Size(24, 13);
            this.L_ID.TabIndex = 6;
            this.L_ID.Text = "[ID]";
            // 
            // L_PluginVersion
            // 
            this.L_PluginVersion.AutoSize = true;
            this.L_PluginVersion.Location = new System.Drawing.Point(478, 10);
            this.L_PluginVersion.Name = "L_PluginVersion";
            this.L_PluginVersion.Size = new System.Drawing.Size(48, 13);
            this.L_PluginVersion.TabIndex = 7;
            this.L_PluginVersion.Text = "Version: ";
            // 
            // L_Version
            // 
            this.L_Version.AutoSize = true;
            this.L_Version.Location = new System.Drawing.Point(522, 10);
            this.L_Version.Name = "L_Version";
            this.L_Version.Size = new System.Drawing.Size(48, 13);
            this.L_Version.TabIndex = 8;
            this.L_Version.Text = "[Version]";
            // 
            // L_PluginVersionInstalled
            // 
            this.L_PluginVersionInstalled.AutoSize = true;
            this.L_PluginVersionInstalled.Location = new System.Drawing.Point(238, 10);
            this.L_PluginVersionInstalled.Name = "L_PluginVersionInstalled";
            this.L_PluginVersionInstalled.Size = new System.Drawing.Size(98, 13);
            this.L_PluginVersionInstalled.TabIndex = 9;
            this.L_PluginVersionInstalled.Text = "Installierte Version: ";
            // 
            // L_VersionInstalled
            // 
            this.L_VersionInstalled.AutoSize = true;
            this.L_VersionInstalled.Location = new System.Drawing.Point(332, 10);
            this.L_VersionInstalled.Name = "L_VersionInstalled";
            this.L_VersionInstalled.Size = new System.Drawing.Size(48, 13);
            this.L_VersionInstalled.TabIndex = 10;
            this.L_VersionInstalled.Text = "[Version]";
            // 
            // PluginSteuerelement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.L_VersionInstalled);
            this.Controls.Add(this.L_PluginVersionInstalled);
            this.Controls.Add(this.L_Version);
            this.Controls.Add(this.L_PluginVersion);
            this.Controls.Add(this.L_ID);
            this.Controls.Add(this.L_PluginId);
            this.Controls.Add(this.L_Name);
            this.Controls.Add(this.B_Update);
            this.Controls.Add(this.B_Download);
            this.Controls.Add(this.RTB_Description);
            this.Controls.Add(this.L_PluginName);
            this.Name = "PluginSteuerelement";
            this.Size = new System.Drawing.Size(650, 185);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label L_PluginName;
        private System.Windows.Forms.RichTextBox RTB_Description;
        private System.Windows.Forms.Button B_Download;
        private System.Windows.Forms.Button B_Update;
        private System.Windows.Forms.Label L_Name;
        private System.Windows.Forms.Label L_PluginId;
        private System.Windows.Forms.Label L_ID;
        private System.Windows.Forms.Label L_PluginVersion;
        private System.Windows.Forms.Label L_Version;
        private System.Windows.Forms.Label L_PluginVersionInstalled;
        private System.Windows.Forms.Label L_VersionInstalled;
    }
}
