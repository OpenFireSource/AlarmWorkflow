using System;
using System.Windows.Forms;
using AlarmWorkflow.Parser.GenericParser.Forms;

namespace AlarmWorkflow.Parser.GenericParser.Misc
{
    abstract class FaxHierarchyTreeNode : TreeNode
    {
        #region Properties

        public FaxHierarchyTreeNodeType NodeType { get; protected set; }

        #endregion

        #region Methods

        public abstract bool IsDeleteAllowed();
        public abstract bool IsAddAllowed();
        public abstract void InvokeAdd();

        #endregion
    }

    class RootTreeNode : FaxHierarchyTreeNode
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RootTreeNode"/> class.
        /// </summary>
        public RootTreeNode()
            : base()
        {
            NodeType = FaxHierarchyTreeNodeType.Root;
            this.Text = "(Oberste Ebene)";
        }

        #endregion

        #region Methods

        public override bool IsAddAllowed() { return true; }
        public override bool IsDeleteAllowed() { return false; }

        public override void InvokeAdd()
        {
            AddSectionOrAreaForm form = new AddSectionOrAreaForm(new[] { FaxHierarchyTreeNodeType.Area, FaxHierarchyTreeNodeType.Section });
            if (form.ShowDialog() == DialogResult.OK)
            {
                TreeNode node = CreateNode(form.NodeType, form.IntroductoryText, form.IntroductoryTextIsContained);
                this.Nodes.Add(node);
                this.ExpandAll();
            }
        }

        private FaxHierarchyTreeNode CreateNode(FaxHierarchyTreeNodeType type, string text, bool isContained)
        {
            GenericParserString gpString = new GenericParserString(text, isContained);
            switch (type)
            {
                case FaxHierarchyTreeNodeType.Section:
                    return new SectionTreeNode()
                    {
                        Definition = new SectionDefinition()
                        {
                            SectionString = gpString
                        }
                    };
                case FaxHierarchyTreeNodeType.Area:
                    return new AreaTreeNode()
                    {
                        Definition = new AreaDefinition()
                        {
                            AreaString = gpString
                        }
                    };
                case FaxHierarchyTreeNodeType.Root:
                default:
                    throw new NotSupportedException();
            }
        }

        #endregion
    }

    class SectionTreeNode : FaxHierarchyTreeNode
    {
        #region Properties

        public SectionDefinition Definition { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionTreeNode"/> class.
        /// </summary>
        public SectionTreeNode()
            : base()
        {
            NodeType = FaxHierarchyTreeNodeType.Section;
        }

        #endregion

        #region Methods

        public override bool IsAddAllowed() { return true; }
        public override bool IsDeleteAllowed() { return true; }

        public override void InvokeAdd()
        {
            AddSectionOrAreaForm form = new AddSectionOrAreaForm(new[] { FaxHierarchyTreeNodeType.Area });
            if (form.ShowDialog() == DialogResult.OK)
            {
            }
        }

        #endregion
    }

    class AreaTreeNode : FaxHierarchyTreeNode
    {
        #region Properties

        public AreaDefinition Definition { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaTreeNode"/> class.
        /// </summary>
        public AreaTreeNode()
            : base()
        {
            NodeType = FaxHierarchyTreeNodeType.Area;
        }

        #endregion

        #region Methods

        public override bool IsAddAllowed() { return false; }
        public override bool IsDeleteAllowed() { return true; }

        public override void InvokeAdd() { }

        #endregion
    }

}
