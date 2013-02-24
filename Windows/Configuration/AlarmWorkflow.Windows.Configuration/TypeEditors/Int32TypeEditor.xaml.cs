using System;
using System.Globalization;
using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    /// <summary>
    /// Interaction logic for Int32TypeEditor.xaml
    /// </summary>
    [Export("Int32TypeEditor", typeof(ITypeEditor))]
    [ConfigurationTypeEditor(typeof(System.Int32))]
    public partial class Int32TypeEditor : UserControl, ITypeEditor
    {
        #region Fields

        private int _minValue = int.MinValue;
        private int _maxValue = int.MaxValue;

        #endregion

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
                if (int.TryParse(txtValue.Text, out value)
                    && (value >= _minValue && value <= _maxValue))
                {
                    return value;
                }
                throw new ValueException(Properties.Resources.NumberTypeEditorValueNotValidMessage, Properties.Resources.NumberTypeEditorValueNotValidHint, _minValue, _maxValue);
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
            if (string.IsNullOrWhiteSpace(editorParameter))
            {
                return;
            }

            string[] tokens = editorParameter.Split(';');
            if (tokens.Length != 2)
            {
                return;
            }

            int vMin = 0;
            int vMax = 0;
            if (!int.TryParse(tokens[0], out vMin) ||
                !int.TryParse(tokens[1], out vMax))
            {
                return;
            }

            if (vMin > vMax)
            {
                return;
            }

            _minValue = vMin;
            _maxValue = vMax;
        }

        #endregion
    }
}
