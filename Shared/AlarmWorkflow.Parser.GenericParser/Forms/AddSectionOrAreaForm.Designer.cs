namespace AlarmWorkflow.Parser.GenericParser.Forms
{
    partial class AddSectionOrAreaForm
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
            this.rbSection = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.txtIntroducedBy = new System.Windows.Forms.TextBox();
            this.rbArea = new System.Windows.Forms.RadioButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.chkIntroducedByIsContained = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // rbSection
            // 
            this.rbSection.AutoSize = true;
            this.rbSection.Location = new System.Drawing.Point(12, 79);
            this.rbSection.Name = "rbSection";
            this.rbSection.Size = new System.Drawing.Size(69, 17);
            this.rbSection.TabIndex = 0;
            this.rbSection.TabStop = true;
            this.rbSection.Text = "Abschnitt";
            this.rbSection.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Eingeleitet durch:";
            // 
            // txtIntroducedBy
            // 
            this.txtIntroducedBy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIntroducedBy.Location = new System.Drawing.Point(128, 15);
            this.txtIntroducedBy.Name = "txtIntroducedBy";
            this.txtIntroducedBy.Size = new System.Drawing.Size(338, 20);
            this.txtIntroducedBy.TabIndex = 3;
            // 
            // rbArea
            // 
            this.rbArea.AutoSize = true;
            this.rbArea.Location = new System.Drawing.Point(12, 148);
            this.rbArea.Name = "rbArea";
            this.rbArea.Size = new System.Drawing.Size(61, 17);
            this.rbArea.TabIndex = 4;
            this.rbArea.TabStop = true;
            this.rbArea.Text = "Bereich";
            this.rbArea.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(391, 253);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "&Abbrechen";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(310, 253);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // chkIntroducedByIsContained
            // 
            this.chkIntroducedByIsContained.AutoSize = true;
            this.chkIntroducedByIsContained.Location = new System.Drawing.Point(128, 41);
            this.chkIntroducedByIsContained.Name = "chkIntroducedByIsContained";
            this.chkIntroducedByIsContained.Size = new System.Drawing.Size(84, 17);
            this.chkIntroducedByIsContained.TabIndex = 7;
            this.chkIntroducedByIsContained.Text = "Ist &enthalten";
            this.chkIntroducedByIsContained.UseVisualStyleBackColor = true;
            // 
            // AddSectionOrAreaForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(478, 288);
            this.Controls.Add(this.chkIntroducedByIsContained);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.rbArea);
            this.Controls.Add(this.txtIntroducedBy);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rbSection);
            this.Name = "AddSectionOrAreaForm";
            this.Text = "Hinzufügen";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbSection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtIntroducedBy;
        private System.Windows.Forms.RadioButton rbArea;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox chkIntroducedByIsContained;
    }
}