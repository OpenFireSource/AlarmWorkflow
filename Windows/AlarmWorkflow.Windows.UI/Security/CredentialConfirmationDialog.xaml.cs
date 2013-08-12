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

using System.Windows;
using AlarmWorkflow.Windows.UIContracts.Security;

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