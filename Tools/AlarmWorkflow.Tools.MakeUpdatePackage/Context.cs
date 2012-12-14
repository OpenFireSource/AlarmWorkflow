using System;
using System.IO;
using System.Reflection;

namespace AlarmWorkflow.Tools.MakeUpdatePackage
{
    class Context
    {
        internal DirectoryInfo ProjectRootDirectory { get; set; }
        internal DirectoryInfo InstallerTempDirectory { get; set; }
        internal Version NewVersion { get; private set; }

        internal static Context CreateContext(Version version)
        {
            Context context = new Context();
            context.NewVersion = version;
            context.ProjectRootDirectory = FindProjectDirectoryFromCurrentDirectoryUp();
            context.InstallerTempDirectory = context.ProjectRootDirectory.CreateSubdirectory("InstallerTemp\\" + version.ToString());

            return context;
        }

        private static DirectoryInfo FindProjectDirectoryFromCurrentDirectoryUp()
        {
            DirectoryInfo current = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            // TODO: Fuzzy algorithm, make better.
            while (current.Parent != null && current.Name != "AlarmWorkflow")
            {
                current = current.Parent;
            }
            return current;
        }

    }
}
