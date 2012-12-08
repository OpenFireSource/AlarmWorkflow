using System;
using System.Net;
using System.Windows.Forms;
using Ionic.Zip;

namespace AlarmWorkflow.Tools.AutoUpdater.Tasks
{
    class DownloadUpdatePackageTask : ITask
    {
        #region ITask Members

        void ITask.Execute(TaskArgs args)
        {
            if (args.Action == TaskArgs.TaskAction.Installation)
            {
                DownloadAndInstallUpdatePackage(args);
            }
        }

        private void DownloadAndInstallUpdatePackage(TaskArgs args)
        {
            Version serverVersion = (Version)args.Context["ServerVersion"];

            using (WebClient client = new WebClient())
            {
                string serverVersionUri = string.Format("{0}/{1}/{2}", Properties.Settings.Default.UpdateServerName, Properties.Settings.Default.UpdateFilesDirectory, serverVersion + ".zip");

                Log.Write("Downloading update package, this may take a while...");

                byte[] buffer = client.DownloadData(new Uri(serverVersionUri));

                Log.Write("Update package downloaded. Begin unpacking...");

                ZipFile zipFile = ZipFile.Read(buffer);
                zipFile.ExtractAll(Application.StartupPath, ExtractExistingFileAction.OverwriteSilently);

                Log.Write("Unpacking download package succeeded.");
            }
        }

        #endregion
    }
}
