using System;
using System.Windows;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.UIContracts.Properties;

namespace AlarmWorkflow.Windows.UIContracts
{
    /// <summary>
    /// Provides helper functionality for UI development.
    /// </summary>
    public static class UIUtilities
    {
        #region Constants

        /// <summary>
        /// Defines the format that should be used for german formatting of <see cref="System.DateTime"/> instances.
        /// </summary>
        public static readonly string DateTimeFormatGermany = "dd.MM.yyyy HH:mm:ss";
        
        #endregion

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

        /// <summary>
        /// Copies some text to the clipboard and gives the user feedback on success or failure.
        /// </summary>
        /// <param name="text">The text to copy to the clipboard.</param>
        public static void CopyToClipboardInteractive(string text)
        {
            try
            {
                Clipboard.SetText(text);
                UIUtilities.ShowInfo(Resources.CopyToClipboardDoneMessage);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(typeof(UIUtilities), ex);
                UIUtilities.ShowWarning(Resources.CopyToClipboardFailedMessage);
            }
        }
    }
}
