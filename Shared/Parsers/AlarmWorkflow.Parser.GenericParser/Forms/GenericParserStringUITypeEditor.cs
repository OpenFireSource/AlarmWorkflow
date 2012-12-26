using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using AlarmWorkflow.Parser.GenericParser.Misc;

namespace AlarmWorkflow.Parser.GenericParser.Forms
{
    /// <summary>
    /// Logic of the UITypeEditor.
    /// </summary>
    internal partial class GenericParserStringUITypeEditor : UserControl
    {
        #region Properties

        /// <summary>
        /// Gets the edited value.
        /// </summary>
        public GenericParserString Value
        {
            get
            {
                return new GenericParserString(this.txtString.Text, chkIsContained.Checked);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericParserStringUITypeEditor"/> class.
        /// </summary>
        public GenericParserStringUITypeEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericParserStringUITypeEditor"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public GenericParserStringUITypeEditor(object value)
            : this()
        {
            SetValue(value);

            this.txtString.Focus();
        }

        #endregion

        #region Methods

        private void SetValue(object value)
        {
            if (value == null)
            {
                return;
            }

            GenericParserString v = (GenericParserString)value;
            this.txtString.Text = v.String;
            this.chkIsContained.Checked = v.IsContained;
        }

        #endregion
    }

    class GenericParserStringUITypeEditorImpl : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            if (context != null
               && context.Instance != null
               && provider != null)
            {

                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (edSvc != null)
                {
                    GenericParserStringUITypeEditor control = new GenericParserStringUITypeEditor(value);
                    edSvc.DropDownControl(control);
                    return control.Value;
                }
            }

            return value;
        }
    }
}
