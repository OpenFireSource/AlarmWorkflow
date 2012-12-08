using System;
using System.IO;

namespace AlarmWorkflow.Tools.AutoUpdater.Tasks
{
    /// <summary>
    /// Special task that runs prior to anything and moves legacy files to their new location (such as the log files).
    /// </summary>
    class LegacyFilesUpdaterTask : ITask
    {
        #region ITask Members

        void ITask.Execute(TaskArgs args)
        {
            switch (args.Action)
            {
                case TaskArgs.TaskAction.Pre:
                    CheckAndMoveLegacyFiles();
                    break;
                default:
                    break;
            }
        }

        private void CheckAndMoveLegacyFiles()
        {
            // Prior to 0.7.1.0, the log file and settings file were under "C:\Users\<USER>\AppData" which was malicious
            // Move them to the better-suited directory
            string oldPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenFireSource");
            if (!Directory.Exists(oldPath))
            {
                Log.Write("Nothing to do, legacy path does not exist.", oldPath);
                return;
            }

            // Move the directory, if it does not exist yet
            string newPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "OpenFireSource");
            if (Directory.Exists(newPath))
            {
                Log.Write("Skipping moving old path to new path (does already exist).");
                return;
            }

            try
            {
                Directory.Move(oldPath, newPath);
                Log.Write("Successfully moved legacy directory from '{0}' to '{1}'.", oldPath, newPath);
            }
            catch (IOException)
            {
                string message = string.Format(Properties.Resources.LegacyTaskMoveFailedMessage, oldPath, newPath);
                Utilities.ShowMessageBox(System.Windows.Forms.MessageBoxIcon.Warning, message);
                Log.Write(message);
            }
        }

        #endregion
    }
}
