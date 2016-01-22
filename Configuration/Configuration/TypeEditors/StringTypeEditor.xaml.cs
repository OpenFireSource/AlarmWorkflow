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
        #region Constants

        private const string PasswordEditorParameter = "Password";

        #endregion

        #region Fields

        private bool _isNull;
        private ITextEditor _editor;

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
                if (_isNull && string.IsNullOrEmpty(_editor.Value))
                {
                    return null;
                }

                return _editor.Value;
            }
            set
            {
                string v = (string)value;
                // We need to remember null since the textbox turns it into ""
                _isNull = v == null;

                _editor.Value = v;
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
            bool isPassword = string.Equals(editorParameter, PasswordEditorParameter, StringComparison.Ordinal);

            if (isPassword)
            {
                _editor = new PasswordEditor();
            }
            else
            {
                _editor = new StringEditor();
            }

            text.Content = _editor.Visual;
        }

        #endregion

        #region Nested types

        interface ITextEditor
        {
            Control Visual { get; }
            string Value { get; set; }
        }

        class StringEditor : ITextEditor
        {
            #region Fields

            private TextBox _control = new TextBox();

            #endregion

            #region ITextEditor Members

            Control ITextEditor.Visual
            {
                get { return _control; }
            }

            string ITextEditor.Value
            {
                get { return _control.Text; }
                set { _control.Text = value; }
            }

            #endregion
        }

        class PasswordEditor : ITextEditor
        {
            #region Fields

            private PasswordBox _control = new PasswordBox();

            #endregion

            #region ITextEditor Members

            Control ITextEditor.Visual
            {
                get { return _control; }
            }

            string ITextEditor.Value
            {
                get { return _control.Password; }
                set { _control.Password = value; }
            }

            #endregion
        }

        #endregion
    }
}