using System;

namespace AlarmWorkflow.Windows.UI.Services
{
    /// <summary>
    /// Defines a means for a window that asks the user for confirmation (including credentials) in order to execute a function.
    /// </summary>
    public interface ICredentialConfirmationDialogService
    {
        /// <summary>
        /// Invokes the credentials-dialog which asks the user for credentials input, and returns the success of that operation.
        /// </summary>
        /// <param name="functionName">The name of the function that is about being invoked.</param>
        /// <returns>The result of the credentials-dialog. This is true if the user is granted the rights, and false if not.</returns>
        bool Invoke(string functionName);
    }
}
