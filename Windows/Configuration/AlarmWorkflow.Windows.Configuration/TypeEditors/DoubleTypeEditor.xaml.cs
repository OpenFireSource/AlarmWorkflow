using System;
using System.Globalization;
using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    /// <summary>
    /// Interaction logic for DoubleTypeEditor.xaml
    /// </summary>
    [Export("DoubleTypeEditor", typeof(ITypeEditor))]
    [ConfigurationTypeEditor(typeof(System.Double))]
    public partial class DoubleTypeEditor : UserControl, ITypeEditor
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleTypeEditor"/> class.
        /// </summary>
        public DoubleTypeEditor()
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
                double value = double.NaN;
                if (!double.TryParse(txtValue.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                {
                    throw new ValueException(Properties.Resources.NumberTypeEditorValueNotValidMessage, Properties.Resources.NumberTypeEditorValueNotValidHint, double.MinValue, double.MaxValue);
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
