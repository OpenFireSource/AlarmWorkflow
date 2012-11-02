using System.Windows.Controls;
using AlarmWorkflow.Windows.ConfigurationContracts;
using System.IO;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    /// <summary>
    /// Interaction logic for StringArrayTypeEditor.xaml
    /// </summary>
    public partial class StringArrayTypeEditor : UserControl, ITypeEditor
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StringArrayTypeEditor"/> class.
        /// </summary>
        public StringArrayTypeEditor()
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
            get { return txtLines.Text; }
            set { txtLines.Text = (string)value; }
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
        }

        #endregion
    }
}
