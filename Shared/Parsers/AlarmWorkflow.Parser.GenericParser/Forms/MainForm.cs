using System.Drawing;
using System.Windows.Forms;
using AlarmWorkflow.Parser.GenericParser.Misc;

namespace AlarmWorkflow.Parser.GenericParser.Forms
{
    /// <summary>
    /// Logic for MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Fields

        private ControlInformation _controlInformation;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            _controlInformation = new ControlInformation();

            PrepareForm();
        }

        #endregion

        #region Methods

        private void PrepareForm()
        {
            PrepareFaxHierarchy();
        }

        private void PrepareFaxHierarchy()
        {
            RootTreeNode rootNode = new RootTreeNode();
            rootNode.ControlInformation = _controlInformation;
            rootNode.Tag = _controlInformation;
            this.trvHierarchy.Nodes.Add(rootNode);
            this.trvHierarchy.SelectedNode = rootNode;
        }

        /// <summary>
        /// Updates the form by reading the current control information.
        /// </summary>
        private void UpdateFromControlInformation()
        {
            this.trvHierarchy.Nodes.Clear();
            // Create root node
            RootTreeNode root = new RootTreeNode();
            root.ControlInformation = _controlInformation;
            root.Tag = _controlInformation;
            root.Text = _controlInformation.FaxName;

            // Create sections
            foreach (SectionDefinition sd in _controlInformation.Sections)
            {
                SectionTreeNode section = new SectionTreeNode();
                section.Tag = sd;
                section.Definition = sd;

                foreach (AreaDefinition ad in sd.Areas)
                {
                    AreaTreeNode area = new AreaTreeNode();
                    area.Tag = ad;
                    area.Definition = ad;

                    section.Nodes.Add(area);
                }

                root.Nodes.Add(section);
            }

            this.trvHierarchy.Nodes.Add(root);
            this.trvHierarchy.SelectedNode = root;
            this.trvHierarchy.ExpandAll();
        }

        #endregion

        #region Event handlers

        #region File-menu

        private void tsmOpen_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = Properties.Resources.Controlfile_FilterText;
            ofd.InitialDirectory = Application.StartupPath;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _controlInformation = ControlInformation.Load(ofd.FileName);
                UpdateFromControlInformation();
            }
        }

        private void tsmSave_Click(object sender, System.EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Properties.Resources.Controlfile_FilterText;
            sfd.InitialDirectory = Application.StartupPath;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _controlInformation.Save(sfd.FileName);
            }
        }

        private void tsmExit_Click(object sender, System.EventArgs e)
        {
        }

        private void tmsFax_Open_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Application.StartupPath;
            ofd.Filter = "Fax images (*.tif;*.tiff;*.bmp)|*.tif;*.tiff;*.bmp";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                picFax.Image = Image.FromFile(ofd.FileName);
            }
        }

        #endregion

        #region Fax hierarchy-toolbar

        private void tsbFaxHierarchyAdd_Click(object sender, System.EventArgs e)
        {
            if (trvHierarchy.SelectedNode == null)
            {
                return;
            }

            FaxHierarchyTreeNode node = (FaxHierarchyTreeNode)trvHierarchy.SelectedNode;
            FaxHierarchyTreeNode addedNode = node.InvokeAdd();
            this.trvHierarchy.SelectedNode = addedNode;
        }

        private void tsbFaxHierarchyRemove_Click(object sender, System.EventArgs e)
        {
            if (trvHierarchy.SelectedNode == null)
            {
                return;
            }

            FaxHierarchyTreeNode node = (FaxHierarchyTreeNode)trvHierarchy.SelectedNode;
            node.InvokeDelete();
            node.Remove();
        }

        #endregion

        private void trvHierarchy_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (trvHierarchy.SelectedNode == null)
            {
                pgSelectedNode.SelectedObject = null;
                return;
            }

            FaxHierarchyTreeNode node = (FaxHierarchyTreeNode)trvHierarchy.SelectedNode;
            tsbFaxHierarchyAdd.Enabled = node.IsAddAllowed();
            tsbFaxHierarchyRemove.Enabled = node.IsDeleteAllowed();

            pgSelectedNode.SelectedObject = node.Tag;
        }

        #endregion
    }
}
