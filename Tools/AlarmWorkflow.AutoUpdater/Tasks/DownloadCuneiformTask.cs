using System;
using System.Net;
using System.Windows.Forms;
using Ionic.Zip;

namespace AlarmWorkflow.Tools.AutoUpdater.Tasks
{
    class DownloadCuneiformTask : ITask
    {
        #region Constants

        private static readonly Uri CuneiformWebUri;

        #endregion

        #region Constructors

        static DownloadCuneiformTask()
        {
            CuneiformWebUri = new Uri(string.Format("{0}/{1}/{2}",
                Properties.Settings.Default.UpdateServerName,
                Properties.Settings.Default.UpdateFilesDirectory,
                Properties.Settings.Default.CuneiformFileName));
        }

        #endregion

        #region ITask Members

        void ITask.Execute(TaskArgs args)
        {
            switch (args.Action)
            {
                case TaskArgs.TaskAction.Post:
                    DownloadCuneiform();
                    break;
                default:
                    break;
            }
        }

        private void DownloadCuneiform()
        {
            using (WebClient client = new WebClient())
            {
                Log.Write("Downloading cuneiform package, this may take several minutes!");

                byte[] buffer = client.DownloadData(CuneiformWebUri);

                Log.Write("Cuneiform package downloaded, unpacking...");

                ZipFile zipFile = ZipFile.Read(buffer);
                zipFile.ExtractAll(Application.StartupPath, ExtractExistingFileAction.OverwriteSilently);

                Log.Write("Cuneiform unpacking succeeded.");
            }
        }

        #endregion
    }
}
