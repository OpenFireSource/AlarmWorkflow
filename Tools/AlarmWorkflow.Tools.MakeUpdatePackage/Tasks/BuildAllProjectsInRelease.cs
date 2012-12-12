using System;
using System.IO;
using System.Diagnostics;

namespace AlarmWorkflow.Tools.MakeUpdatePackage.Tasks
{
    class BuildAllProjectsInRelease : ITask
    {
        #region Constants

        private static readonly string MsBuildSwitch = "/p:Configuration=Release /verbosity:minimal";

        private readonly string MsBuildPath;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildAllProjectsInRelease"/> class.
        /// </summary>
        public BuildAllProjectsInRelease()
        {
            MsBuildPath = ConstructMsBuildPath();
        }

        private string ConstructMsBuildPath()
        {
            string x64Switch = Environment.Is64BitOperatingSystem ? "64" : "";
            return string.Format("C:\\Windows\\Microsoft.NET\\Framework{0}\\v4.0.30319\\MSBuild.exe", x64Switch);
        }

        #endregion

        #region ITask Members

        void ITask.Execute(Context context)
        {
            BuildProjectsInBuildOrder(context);
        }

        private void BuildProjectsInBuildOrder(Context context)
        {
            using (StringReader reader = new StringReader(Properties.Resources.BuildOrder))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    FileInfo asfn = new FileInfo(Path.Combine(context.ProjectRootDirectory.FullName, line));
                    if (!asfn.Exists)
                    {
                        // TODO: Warning
                        continue;
                    }

                    BuildProject(context, asfn);
                }
            }
        }

        private void BuildProject(Context context, FileInfo solutionFile)
        {
            try
            {
                Process msBuild = new Process();
                msBuild.StartInfo.UseShellExecute = false;

                string arguments = string.Format("{0} {1}", solutionFile.FullName, MsBuildSwitch);
                msBuild.StartInfo.FileName = MsBuildPath;
                msBuild.StartInfo.Arguments = arguments;
                
                msBuild.Start();
                msBuild.WaitForExit();
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        #endregion
    }
}
