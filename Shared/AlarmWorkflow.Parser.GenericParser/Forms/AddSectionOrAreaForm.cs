using System.Linq;
using System.Windows.Forms;
using AlarmWorkflow.Parser.GenericParser.Misc;

namespace AlarmWorkflow.Parser.GenericParser.Forms
{
    partial class AddSectionOrAreaForm : Form
    {
        #region Properties

        public FaxHierarchyTreeNodeType NodeType { get; set; }
        public string IntroductoryText
        {
            get { return txtIntroducedBy.Text; }
        }
        public bool IntroductoryTextIsContained
        {
            get { return chkIntroducedByIsContained.Checked; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddSectionOrAreaForm"/> class.
        /// </summary>
        private AddSectionOrAreaForm()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddSectionOrAreaForm"/> class.
        /// </summary>
        /// <param name="allowed">The allowed.</param>
        public AddSectionOrAreaForm(FaxHierarchyTreeNodeType[] allowed)
            : this()
        {
            InitializeComponent();

            rbArea.Enabled = allowed.Contains(FaxHierarchyTreeNodeType.Area);
            rbSection.Enabled = allowed.Contains(FaxHierarchyTreeNodeType.Section);

            foreach (RadioButton button in this.Controls.OfType<RadioButton>())
            {
                if (button.Enabled)
                {
                    button.Checked = true;
                    break;
                }
            }
        }

        #endregion

        #region Event handlers

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            if (rbArea.Checked)
            {
                NodeType = FaxHierarchyTreeNodeType.Area;
            }
            else
            {
                NodeType = FaxHierarchyTreeNodeType.Section;
            }
        }

        #endregion
    }
}
