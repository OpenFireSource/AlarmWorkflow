using System;
using System.Linq;
using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    /// <summary>
    /// Interaction logic for ExportTypeEditor.xaml
    /// </summary>
    [Export("ExportTypeEditor", typeof(ITypeEditor))]
    public partial class ExportTypeEditor : UserControl, ITypeEditor
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportTypeEditor"/> class.
        /// </summary>
        public ExportTypeEditor()
        {
            InitializeComponent();
        }

        #endregion

        #region ITypeEditor Members

        /// <summary>
        /// Gets/sets the value that is edited.
        /// </summary>
        public object Value
        {
            get { return cboExport.Text; }
            set { cboExport.Text = (string)value; }
        }

        /// <summary>
        /// Gets the visual element that is editing the value.
        /// </summary>
        public System.Windows.UIElement Visual
        {
            get { return this; }
        }

        void ITypeEditor.Initialize(string editorParameter)
        {
            if (string.IsNullOrWhiteSpace(editorParameter))
            {
                return;
            }

            // Find out the type - if the type could not be found, make the combobox editable
            Type type = Type.GetType(editorParameter);
            if (type == null)
            {
                cboExport.IsEditable = true;
                return;
            }

            // Otherwise list all exports
            foreach (ExportedType export in ExportedTypeLibrary.GetExports(type).OrderBy(et => et.Attribute.Alias))
            {
                cboExport.Items.Add(export.Attribute.Alias);
            }
        }

        #endregion
    }
}
