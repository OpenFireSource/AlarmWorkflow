using System.Windows.Controls;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    /// <summary>
    /// Interaction logic for StringTypeEditor.xaml
    /// </summary>
    public partial class StringTypeEditor : UserControl, ITypeEditor
    {
        #region Fields

        private bool _isNull;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StringTypeEditor"/> class.
        /// </summary>
        public StringTypeEditor()
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
            get
            {
                if (_isNull && string.IsNullOrEmpty(txtValue.Text))
                {
                    return null;
                }

                return txtValue.Text;
            }
            set
            {
                string v = (string)value;
                // We need to remember null since the textbox turns it into ""
                _isNull = v == null;

                txtValue.Text = v;
            }
        }

        /// <summary>
        /// Gets the visual element that is editing the value.
        /// </summary>
        public System.Windows.UIElement Visual
        {
            get { return this; }
        }

        #endregion
    }
}
