using System;

namespace AlarmWorkflow.Windows.UI.Contracts.Security
{
    /// <summary>
    /// Specifies the supported authorization modes.
    /// </summary>
    public enum AuthorizationMode
    {
        /// <summary>
        /// Authorization requires the user to confirm the dialog.
        /// </summary>
        SimpleConfirmation,
        /// <summary>
        /// Authorization uses a password-system.
        /// </summary>
        Password,
    }
}
