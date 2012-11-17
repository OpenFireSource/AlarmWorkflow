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
            this.trvHierarchy.Nodes.Add(rootNode);
            this.trvHierarchy.SelectedNode = rootNode;
        }

        #endregion

        #region Event handlers

        #region File-menu

        private void tsmOpen_Click(object sender, System.EventArgs e)
        {

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
            node.InvokeAdd();
        }

        private void tsbFaxHierarchyRemove_Click(object sender, System.EventArgs e)
        {
            if (trvHierarchy.SelectedNode == null)
            {
                return;
            }

            FaxHierarchyTreeNode node = (FaxHierarchyTreeNode)trvHierarchy.SelectedNode;
            node.Remove();
        }

        #endregion

        private void trvHierarchy_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (trvHierarchy.SelectedNode == null)
            {
                return;
            }

            FaxHierarchyTreeNode node = (FaxHierarchyTreeNode)trvHierarchy.SelectedNode;
            tsbFaxHierarchyAdd.Enabled = node.IsAddAllowed();
            tsbFaxHierarchyRemove.Enabled = node.IsDeleteAllowed();
        }

        #endregion
    }
}
