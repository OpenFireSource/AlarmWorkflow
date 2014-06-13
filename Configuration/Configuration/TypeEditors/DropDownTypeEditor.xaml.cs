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

using System.Collections.Generic;
using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    /// <summary>
    /// Interaction logic for DropDownTypeEditor.xaml
    /// </summary>
    [Export("DropDownTypeEditor", typeof(ITypeEditor))]
    public partial class DropDownTypeEditor : UserControl, ITypeEditor
    {
        #region Properties

        /// <summary>
        /// Gets the list of items that are shown in the combobox/dropdownlist.
        /// </summary>
        public IList<string> Items { get; private set; }
        /// <summary>
        /// Gets whether or not this list allows a selection of existing values
        /// or the user is free to enter any value.
        /// </summary>
        public bool IsEditable { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DropDownTypeEditor"/> class.
        /// </summary>
        public DropDownTypeEditor()
        {
            InitializeComponent();

            this.IsEditable = true;
            this.DataContext = this;
        }

        #endregion

        #region ITypeEditor Members

        /// <summary>
        /// Gets/sets the value that is edited.
        /// </summary>
        public object Value
        {
            get { return cboValues.SelectedValue; }
            set { cboValues.SelectedValue = (string)value; }
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

            string[] items = editorParameter.Split(';');
            this.Items = new List<string>(items);

            this.IsEditable = items.Length == 0;
        }

        #endregion
    }
}