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
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.ObjectExpressions;
using AlarmWorkflow.Windows.ConfigurationContracts;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    /// <summary>
    /// Interaction logic for ObjectExpressionTypeEditor.xaml
    /// </summary>
    [Export("ObjectExpressionTypeEditor", typeof(ITypeEditor))]
    public partial class ObjectExpressionTypeEditor : UserControl, ITypeEditor
    {
        #region Properties

        public IList<string> ObjectPropertiesHelp { get; private set; }

        #endregion

        #region Commands

        #region Command "InsertPropertyCommand"

        /// <summary>
        /// The InsertPropertyCommand command.
        /// </summary>
        public ICommand InsertPropertyCommand { get; private set; }

        private void InsertPropertyCommand_Execute(object parameter)
        {
            // Sanity check
            string propertyName = parameter as string;
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return;
            }

            string textToInsert = "{" + propertyName + "}";
            txtText.Text = txtText.Text.Insert(txtText.CaretIndex, textToInsert);
        }

        #endregion

        #endregion

        #region Constructors

        public ObjectExpressionTypeEditor()
        {
            InitializeComponent();

            CommandHelper.WireupRelayCommands(this);
            this.DataContext = this;
        }

        #endregion

        #region Methods

        private void PART_OpenPopup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!PART_Popup.IsOpen)
            {
                PART_Popup.IsOpen = true;
                PART_Popup.Focus();
            }
            else
            {
                PART_Popup.IsOpen = false;
            }
        }

        private void PART_Popup_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (e.OriginalSource != PART_Popup)
            {
                return;
            }
            PART_Popup.IsOpen = false;
        }

        #endregion

        #region ITypeEditor Members

        /// <summary>
        /// Gets/sets the value that is edited.
        /// </summary>
        public object Value
        {
            get { return txtText.Text; }
            set { txtText.Text = (string)value; }
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

            // Find out the type - if the type could not be found, go out.
            Type type = Type.GetType(editorParameter);
            this.ObjectPropertiesHelp = ObjectExpressionTools.GetPropertyNames(type, null, false);
        }

        #endregion
    }
}