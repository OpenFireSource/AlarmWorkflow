using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AlarmWorkflow.Tools.MakeUpdatePackage.Tasks
{
    class CopyFilesToInstallerTempTask : ITask
    {
        #region Constants

        private const string CopyBatchFileName = "copy.bat";

        #endregion

        #region ITask Members

        void ITask.Execute(Context context)
        {
            CreateDynamicBatchFileAndExecute(context);
        }

        private void CreateDynamicBatchFileAndExecute(Context context)
        {
            Dictionary<string, string> macros = new Dictionary<string, string>();
            macros["Root"] = context.ProjectRootDirectory.FullName;
            macros["ITemp"] = context.InstallerTempDirectory.FullName;

            string source = Properties.Resources.FileCopyScript;
            foreach (var pair in macros)
            {
                string macro = "{" + pair.Key + "}";
                source = source.Replace(macro, pair.Value);
            }

            string destinationFileName = Path.Combine(context.InstallerTempDirectory.Parent.FullName, CopyBatchFileName);
            File.WriteAllText(destinationFileName, source);

            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = destinationFileName;

            process.Start();
            process.WaitForExit();
        }

        #endregion
    }
}
