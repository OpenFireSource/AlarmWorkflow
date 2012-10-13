using System.Windows;

namespace AlarmWorkflow.Windows.UI.Views
{
    /// <summary>
    /// Interaction logic for CredentialConfirmationDialog.xaml
    /// </summary>
    internal partial class CredentialConfirmationDialog : Window
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialConfirmationDialog"/> class.
        /// </summary>
        public CredentialConfirmationDialog()
        {
            InitializeComponent();

            this.Loaded += CredentialConfirmationDialog_Loaded;
        }

        #endregion

        #region Event handlers

        private void CredentialConfirmationDialog_Loaded(object sender, RoutedEventArgs e)
        {
            pbPassword.Focus();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #endregion
    }
}
