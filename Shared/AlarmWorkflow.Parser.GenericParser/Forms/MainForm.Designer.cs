namespace AlarmWorkflow.Parser.GenericParser.Forms
{
    partial class MainForm
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
            System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tmsFax_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmExit = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.picFax = new System.Windows.Forms.PictureBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.trvHierarchy = new System.Windows.Forms.TreeView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbFaxHierarchyAdd = new System.Windows.Forms.ToolStripButton();
            this.tsbFaxHierarchyRemove = new System.Windows.Forms.ToolStripButton();
            this.tsbFaxHierarchyUp = new System.Windows.Forms.ToolStripButton();
            this.tsbFaxHierarchyDown = new System.Windows.Forms.ToolStripButton();
            this.pgSelectedNode = new System.Windows.Forms.PropertyGrid();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(737, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmsFax_Open,
            this.toolStripSeparator3,
            this.tsmOpen,
            this.tsmSave,
            this.toolStripSeparator1,
            this.tsmExit});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(46, 20);
            this.toolStripMenuItem1.Text = "&Datei";
            // 
            // tmsFax_Open
            // 
            this.tmsFax_Open.Name = "tmsFax_Open";
            this.tmsFax_Open.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.tmsFax_Open.Size = new System.Drawing.Size(262, 22);
            this.tmsFax_Open.Text = "&Bild öffnen...";
            this.tmsFax_Open.Click += new System.EventHandler(this.tmsFax_Open_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(259, 6);
            // 
            // tsmOpen
            // 
            this.tsmOpen.Name = "tsmOpen";
            this.tsmOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.tsmOpen.Size = new System.Drawing.Size(262, 22);
            this.tsmOpen.Text = "Steuerungsdatei &laden...";
            this.tsmOpen.Click += new System.EventHandler(this.tsmOpen_Click);
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.tsmSave.Size = new System.Drawing.Size(262, 22);
            this.tsmSave.Text = "Steuerungsdatei &speichern...";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(259, 6);
            // 
            // tsmExit
            // 
            this.tsmExit.Name = "tsmExit";
            this.tsmExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.tsmExit.Size = new System.Drawing.Size(262, 22);
            this.tsmExit.Text = "&Beenden";
            this.tsmExit.Click += new System.EventHandler(this.tsmExit_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(737, 500);
            this.splitContainer1.SplitterDistance = 474;
            this.splitContainer1.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(259, 500);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Fax";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.picFax);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(253, 481);
            this.panel1.TabIndex = 1;
            // 
            // picFax
            // 
            this.picFax.Location = new System.Drawing.Point(0, 0);
            this.picFax.Name = "picFax";
            this.picFax.Size = new System.Drawing.Size(100, 50);
            this.picFax.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picFax.TabIndex = 0;
            this.picFax.TabStop = false;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.toolStripContainer1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pgSelectedNode);
            this.splitContainer2.Size = new System.Drawing.Size(474, 500);
            this.splitContainer2.SplitterDistance = 265;
            this.splitContainer2.TabIndex = 0;
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.trvHierarchy);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(474, 240);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(474, 265);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // trvHierarchy
            // 
            this.trvHierarchy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvHierarchy.Location = new System.Drawing.Point(0, 0);
            this.trvHierarchy.Name = "trvHierarchy";
            this.trvHierarchy.Size = new System.Drawing.Size(474, 240);
            this.trvHierarchy.TabIndex = 0;
            this.trvHierarchy.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvHierarchy_AfterSelect);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbFaxHierarchyAdd,
            this.tsbFaxHierarchyRemove,
            toolStripSeparator2,
            this.tsbFaxHierarchyUp,
            this.tsbFaxHierarchyDown});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(474, 25);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 0;
            // 
            // tsbFaxHierarchyAdd
            // 
            this.tsbFaxHierarchyAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbFaxHierarchyAdd.Image = global::AlarmWorkflow.Parser.GenericParser.Properties.Resources.Plus;
            this.tsbFaxHierarchyAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFaxHierarchyAdd.Name = "tsbFaxHierarchyAdd";
            this.tsbFaxHierarchyAdd.Size = new System.Drawing.Size(23, 22);
            this.tsbFaxHierarchyAdd.Text = "Knoten hinzufügen";
            this.tsbFaxHierarchyAdd.Click += new System.EventHandler(this.tsbFaxHierarchyAdd_Click);
            // 
            // tsbFaxHierarchyRemove
            // 
            this.tsbFaxHierarchyRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbFaxHierarchyRemove.Image = global::AlarmWorkflow.Parser.GenericParser.Properties.Resources.Minus;
            this.tsbFaxHierarchyRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFaxHierarchyRemove.Name = "tsbFaxHierarchyRemove";
            this.tsbFaxHierarchyRemove.Size = new System.Drawing.Size(23, 22);
            this.tsbFaxHierarchyRemove.Text = "Knoten entfernen";
            this.tsbFaxHierarchyRemove.Click += new System.EventHandler(this.tsbFaxHierarchyRemove_Click);
            // 
            // tsbFaxHierarchyUp
            // 
            this.tsbFaxHierarchyUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbFaxHierarchyUp.Enabled = false;
            this.tsbFaxHierarchyUp.Image = global::AlarmWorkflow.Parser.GenericParser.Properties.Resources.UpArrow;
            this.tsbFaxHierarchyUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFaxHierarchyUp.Name = "tsbFaxHierarchyUp";
            this.tsbFaxHierarchyUp.Size = new System.Drawing.Size(23, 22);
            this.tsbFaxHierarchyUp.Text = "Knoten logisch nach oben";
            // 
            // tsbFaxHierarchyDown
            // 
            this.tsbFaxHierarchyDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbFaxHierarchyDown.Enabled = false;
            this.tsbFaxHierarchyDown.Image = global::AlarmWorkflow.Parser.GenericParser.Properties.Resources.DownArrow;
            this.tsbFaxHierarchyDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFaxHierarchyDown.Name = "tsbFaxHierarchyDown";
            this.tsbFaxHierarchyDown.Size = new System.Drawing.Size(23, 22);
            this.tsbFaxHierarchyDown.Text = "Knoten logisch nach unten";
            // 
            // pgSelectedNode
            // 
            this.pgSelectedNode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgSelectedNode.Location = new System.Drawing.Point(0, 0);
            this.pgSelectedNode.Name = "pgSelectedNode";
            this.pgSelectedNode.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgSelectedNode.Size = new System.Drawing.Size(474, 231);
            this.pgSelectedNode.TabIndex = 0;
            this.pgSelectedNode.ToolbarVisible = false;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(737, 524);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Generic-Parser Steuerungsdateieditor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFax)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem tsmOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmExit;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox picFax;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem tmsFax_Open;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.TreeView trvHierarchy;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbFaxHierarchyAdd;
        private System.Windows.Forms.ToolStripButton tsbFaxHierarchyRemove;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.ToolStripButton tsbFaxHierarchyUp;
        private System.Windows.Forms.ToolStripButton tsbFaxHierarchyDown;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.PropertyGrid pgSelectedNode;
    }
}