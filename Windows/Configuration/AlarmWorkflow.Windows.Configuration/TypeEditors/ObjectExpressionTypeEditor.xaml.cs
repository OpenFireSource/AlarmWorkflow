using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using AlarmWorkflow.Shared.Core;
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
