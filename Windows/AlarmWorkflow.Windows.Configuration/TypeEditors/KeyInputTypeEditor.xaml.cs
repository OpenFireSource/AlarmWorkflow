using System;
using System.Windows.Controls;
using System.Windows.Input;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    /// <summary>
    /// Interaction logic for KeyInputTypeEditor.xaml
    /// </summary>
    [Export("KeyInputTypeEditor", typeof(ITypeEditor))]
    public partial class KeyInputTypeEditor : UserControl, ITypeEditor
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyInputTypeEditor"/> class.
        /// </summary>
        public KeyInputTypeEditor()
        {
            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void txtKeyInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            if (e.Key == Key.Escape ||
                e.Key == Key.Tab ||
                e.Key == Key.Return)
            {
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                return;
            }

            txtKeyInput.Text = Enum.GetName(typeof(Key), e.Key);
        }

        #endregion

        #region ITypeEditor Members

        /// <summary>
        /// Gets/sets the value that is edited.
        /// </summary>
        public object Value
        {
            get { return txtKeyInput.Text; }
            set { txtKeyInput.Text = (string)value; }
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
