using System.Windows;

namespace AlarmWorkflow.Windows.UIContracts
{
    /// <summary>
    /// Provides helper functionality for UI development.
    /// </summary>
    public static class UIUtilities
    {
        /// <summary>
        /// Brings up a message box and asks for confirmation.
        /// </summary>
        /// <param name="icon">The icon for the message box.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>true if the user has confirmed the message box, otherwise false.</returns>
        public static bool ConfirmMessageBox(MessageBoxImage icon, string text, params object[] args)
        {
            string message = string.Format(text, args);
            return MessageBox.Show(message, "Bestätigung", MessageBoxButton.YesNo, icon) == MessageBoxResult.Yes;
        }
    }
}
