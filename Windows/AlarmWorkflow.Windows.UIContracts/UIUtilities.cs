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
        /// <param name="format">The text to display.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>true if the user has confirmed the message box, otherwise false.</returns>
        public static bool ConfirmMessageBox(MessageBoxImage icon, string format, params object[] args)
        {
            string message = string.Format(format, args);
            return MessageBox.Show(message, "Bestätigung", MessageBoxButton.YesNo, icon) == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Brings up a message box with "warning" content.
        /// </summary>
        /// <param name="format">The text to display.</param>
        /// <param name="args">The arguments.</param>
        public static void ShowWarning(string format, params object[] args)
        {
            MessageBox.Show(string.Format(format, args), "Warnung", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Brings up a message box with "information" content.
        /// </summary>
        /// <param name="format">The text to display.</param>
        /// <param name="args">The arguments.</param>
        public static void ShowInfo(string format, params object[] args)
        {
            MessageBox.Show(string.Format(format, args), "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
