using System;
using System.Net;
using System.Windows.Forms;
using Ionic.Zip;

namespace AlarmWorkflow.Tools.AutoUpdater.Tasks
{
    class DownloadOcrSoftwareTask : ITask
    {
        #region Constants

        private static readonly Uri OcrSoftwarePackageWebUri;

        #endregion

        #region Constructors

        static DownloadOcrSoftwareTask()
        {
            OcrSoftwarePackageWebUri = new Uri(string.Format("{0}/{1}/{2}",
                Properties.Settings.Default.UpdateServerName,
                Properties.Settings.Default.UpdateFilesDirectory,
                Properties.Settings.Default.OcrSoftwarePackageFileName));
        }

        #endregion

        #region ITask Members

        void ITask.Execute(TaskArgs args)
        {
            switch (args.Action)
            {
                case TaskArgs.TaskAction.Post:
                    DownloadOcrSoftwarePackage();
                    break;
                default:
                    break;
            }
        }

        private void DownloadOcrSoftwarePackage()
        {
            using (WebClient client = new WebClient())
            {
                Log.Write("Downloading OCR software package, this may take several minutes!");

                byte[] buffer = client.DownloadData(OcrSoftwarePackageWebUri);

                Log.Write("OCR software package downloaded, unpacking...");

                ZipFile zipFile = ZipFile.Read(buffer);
                zipFile.ExtractAll(Application.StartupPath, ExtractExistingFileAction.OverwriteSilently);

                Log.Write("OCR software unpacking succeeded.");
            }
        }

        #endregion
    }
}
