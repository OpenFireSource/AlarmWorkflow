using System.Windows;

namespace AlarmWorkflow.Windows.UI.Security
{
    /// <summary>
    /// Interaction logic for CredentialConfirmationDialog.xaml
    /// </summary>
    internal partial class CredentialConfirmationDialog : Window
    {
        #region Properties

        /// <summary>
        /// Gets whether or not the authorization based on the current <see cref="AuthorizationMode"/> is successful or not.
        /// </summary>
        public bool IsAuthorizationSuccessful { get; private set; }

        /// <summary>
        /// Gets/sets the used authorization mode.
        /// </summary>
        public AuthorizationMode AuthorizationMode
        {
            get { return (AuthorizationMode)GetValue(AuthorizationModeProperty); }
            set { SetValue(AuthorizationModeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AuthorizationMode"/>-property.
        /// </summary>
        public static readonly DependencyProperty AuthorizationModeProperty = DependencyProperty.Register("AuthorizationMode", typeof(AuthorizationMode), typeof(CredentialConfirmationDialog), new UIPropertyMetadata(AuthorizationMode.SimpleConfirmation));

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialConfirmationDialog"/> class.
        /// </summary>
        public CredentialConfirmationDialog()
        {
            InitializeComponent();
            if (App.Current.MainWindow != null)
            {
                this.Owner = App.Current.MainWindow;
            }
            this.Loaded += CredentialConfirmationDialog_Loaded;
        }

        #endregion

        #region Event handlers

        private void CredentialConfirmationDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= CredentialConfirmationDialog_Loaded;

            pbPassword.Focus();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            switch (AuthorizationMode)
            {
                case AuthorizationMode.SimpleConfirmation:
                    IsAuthorizationSuccessful = true;
                    break;
                case AuthorizationMode.Password:
                    // TODO
                    IsAuthorizationSuccessful = true;
                    break;
                default:
                    break;
            }
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            IsAuthorizationSuccessful = false;
            this.DialogResult = false;
        }

        #endregion
    }
}
