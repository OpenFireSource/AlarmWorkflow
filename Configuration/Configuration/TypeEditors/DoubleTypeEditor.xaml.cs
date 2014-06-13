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
    /// Interaction logic for DoubleTypeEditor.xaml
    /// </summary>
    [Export("DoubleTypeEditor", typeof(ITypeEditor))]
    [ConfigurationTypeEditor(typeof(System.Double))]
    public partial class DoubleTypeEditor : UserControl, ITypeEditor
    {
        #region Fields

        private double _minValue = double.MinValue;
        private double _maxValue = double.MaxValue;

        #endregion

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
                if (double.TryParse(txtValue.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out value)
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

            double vMin = 0;
            double vMax = 0;
            if (!double.TryParse(tokens[0], NumberStyles.Number, CultureInfo.InvariantCulture, out vMin) ||
                !double.TryParse(tokens[1], NumberStyles.Number, CultureInfo.InvariantCulture, out vMax))
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