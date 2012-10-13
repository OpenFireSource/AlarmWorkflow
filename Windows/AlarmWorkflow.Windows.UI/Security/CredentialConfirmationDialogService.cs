
namespace AlarmWorkflow.Windows.UI.Security
{
    class CredentialConfirmationDialogService : ICredentialConfirmationDialogService
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialConfirmationDialogService"/> class.
        /// </summary>
        public CredentialConfirmationDialogService()
        {

        }

        #endregion

        #region ICredentialConfirmationDialogService Members

        bool ICredentialConfirmationDialogService.Invoke(string functionName, AuthorizationMode authorizationMode)
        {
            CredentialConfirmationDialog dialog = new CredentialConfirmationDialog();
            dialog.AuthorizationMode = authorizationMode;
            dialog.txtFunctionName.Text = functionName;

            if (dialog.ShowDialog() == true)
            {
                // TODO: Validate password
                return true;
            }

            return false;
        }

        #endregion
    }
}
