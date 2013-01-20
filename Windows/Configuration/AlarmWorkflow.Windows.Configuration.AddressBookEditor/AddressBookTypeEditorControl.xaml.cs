using System.Windows;
using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.Configuration.AddressBookEditor.Views;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.AddressBookEditor
{
    /// <summary>
    /// Interaction logic for AddressBookTypeEditorControl.xaml
    /// </summary>
    [Export("AddressBookEditor", typeof(ITypeEditor))]
    public partial class AddressBookTypeEditorControl : UserControl, ITypeEditor
    {
        #region Fields

        private object _valueRaw;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressBookTypeEditorControl"/> class.
        /// </summary>
        public AddressBookTypeEditorControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void EditAddressBook_Click(object sender, RoutedEventArgs e)
        {
            // Manually create window
            Window wnd = new Window();
            wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            wnd.Owner = Application.Current.MainWindow;
            wnd.Title = Properties.Resources.AddressBookWindowTitle;
            wnd.Closing += (a, b) =>
            {
                var result = MessageBox.Show(Properties.Resources.AddressBookWindowLeaveConfirmation_MSG, Properties.Resources.AddressBookWindowLeaveConfirmation_CAP, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Cancel)
                {
                    b.Cancel = true;
                    return;
                }

                wnd.DialogResult = (result == MessageBoxResult.Yes) ? true : false;
            };

            AddressBookEditorControl editorctrl = new AddressBookEditorControl();
            editorctrl.ValueWrapper = _valueRaw;
            wnd.Content = editorctrl;

            if (wnd.ShowDialog() == true)
            {
                _valueRaw = editorctrl.ValueWrapper;
            }
        }

        #endregion

        #region ITypeEditor Members

        object ITypeEditor.Value
        {
            get { return _valueRaw; }
            set { _valueRaw = value; }
        }

        /// <summary>
        /// 
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
