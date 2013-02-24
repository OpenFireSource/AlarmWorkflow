using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ConfigurationContracts;
using System.Globalization;
using System;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    /// <summary>
    /// Interaction logic for Int32TypeEditor.xaml
    /// </summary>
    [Export("Int32TypeEditor", typeof(ITypeEditor))]
    [ConfigurationTypeEditor(typeof(System.Int32))]
    public partial class Int32TypeEditor : UserControl, ITypeEditor
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Int32TypeEditor"/> class.
        /// </summary>
        public Int32TypeEditor()
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
                int value = 0;
                if (!int.TryParse(txtValue.Text, out value))
                {
                    throw new ValueException(Properties.Resources.NumberTypeEditorValueNotValidMessage, Properties.Resources.NumberTypeEditorValueNotValidHint, int.MinValue, int.MaxValue);
                }
                return value;
            }
            set
            {
                txtValue.Text = Convert.ToString(value, CultureInfo.InvariantCulture);
            }
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
