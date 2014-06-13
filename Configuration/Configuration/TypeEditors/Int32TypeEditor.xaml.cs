// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

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