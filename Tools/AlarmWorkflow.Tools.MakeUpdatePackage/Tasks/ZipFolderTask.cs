using System;
using System.IO;
using Ionic.Zip;

namespace AlarmWorkflow.Tools.MakeUpdatePackage.Tasks
{
    class ZipFolderTask : ITask
    {
        #region ITask Members

        void ITask.Execute(Context context)
        {
            ZipFile zip = new ZipFile();

            foreach (FileInfo file in context.InstallerTempDirectory.GetFiles("*.*", SearchOption.AllDirectories))
            {
                string pathInArchive = file.DirectoryName.Replace(context.InstallerTempDirectory.FullName, "");
                zip.AddFile(file.FullName, pathInArchive);
            }

            string zipFileName = Path.Combine(context.ProjectRootDirectory.FullName, string.Format("{0}.zip", context.NewVersion.ToString()));
            zip.Save(zipFileName);

            Console.WriteLine(Properties.Resources.ZipFileCreatedUnder, zipFileName);
        }

        #endregion
    }
}
