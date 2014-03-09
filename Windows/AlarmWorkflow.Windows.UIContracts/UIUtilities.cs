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
        /// Brings up a message box with "error" content.
        /// </summary>
        /// <param name="format">The text to display.</param>
        /// <param name="args">The arguments.</param>
        public static void ShowError(string format, params object[] args)
        {
            MessageBox.Show(string.Format(format, args), "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
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