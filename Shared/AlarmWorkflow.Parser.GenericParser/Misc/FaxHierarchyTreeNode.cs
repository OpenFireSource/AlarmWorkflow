using System;
using System.Windows.Forms;

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
        public abstract FaxHierarchyTreeNode InvokeAdd();
        public virtual void InvokeDelete() { }

        #endregion
    }

    class RootTreeNode : FaxHierarchyTreeNode
    {
        #region Properties

        public ControlInformation ControlInformation { get; set; }

        #endregion

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

        public override FaxHierarchyTreeNode InvokeAdd()
        {
            SectionTreeNode sectionNode = new SectionTreeNode();
            this.Nodes.Add(sectionNode);
            this.ExpandAll();

            // Add to control information
            if (ControlInformation != null)
            {
                ControlInformation.Sections.Add(sectionNode.Definition);
            }

            return sectionNode;
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
            this.Text = "Abschnitt";

            Definition = new SectionDefinition();
            this.Tag = Definition;
        }

        #endregion

        #region Methods

        public override bool IsAddAllowed() { return true; }
        public override bool IsDeleteAllowed() { return true; }

        public override FaxHierarchyTreeNode InvokeAdd()
        {
            AreaTreeNode areaNode = new AreaTreeNode();
            this.Nodes.Add(areaNode);
            this.ExpandAll();

            // Add to section
            if (Definition != null)
            {
                Definition.Areas.Add(areaNode.Definition);
            }

            return areaNode;
        }

        public override void InvokeDelete()
        {
            RootTreeNode parent = (RootTreeNode)this.Parent;
            parent.ControlInformation.Sections.Remove(this.Definition);
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
            this.Text = "Bereich";

            this.Definition = new AreaDefinition();
            this.Tag = Definition;
        }

        #endregion

        #region Methods

        public override bool IsAddAllowed() { return false; }
        public override bool IsDeleteAllowed() { return true; }

        public override FaxHierarchyTreeNode InvokeAdd()
        {
            return null;
        }

        public override void InvokeDelete()
        {
            SectionTreeNode parent = (SectionTreeNode)this.Parent;
            parent.Definition.Areas.Remove(this.Definition);
        }

        #endregion
    }

}
