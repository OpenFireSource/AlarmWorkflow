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

using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    /// <summary>
    /// Interaction logic for StringTypeEditor.xaml
    /// </summary>
    [Export("StringTypeEditor", typeof(ITypeEditor))]
    [ConfigurationTypeEditor(typeof(System.String))]
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

        void ITypeEditor.Initialize(string editorParameter)
        {
        }

        #endregion
    }
}