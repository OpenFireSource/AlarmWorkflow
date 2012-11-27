using System;
using System.Security.Principal;
using System.Windows.Forms;

namespace AlarmWorkflow.Tools.AutoUpdater
{
    static class Utilities
    {
        internal static void ShowMessageBox(MessageBoxIcon icon, string format, params object[] args)
        {
            string msg = string.Format(format, args);
            MessageBox.Show(msg, "", MessageBoxButtons.OK, icon);
        }

        internal static bool ConfirmMessageBox(string text)
        {
            DialogResult dr = MessageBox.Show(text, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return dr == DialogResult.Yes;
        }
        internal static bool IsElevated()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            if (id != null)
            {
                WindowsPrincipal p = new WindowsPrincipal(id);
                return p.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return false;
        }
    }
}
