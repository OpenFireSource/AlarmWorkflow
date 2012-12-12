using System;
using System.IO;
using System.Reflection;

namespace AlarmWorkflow.Tools.MakeUpdatePackage
{
    class Context
    {
        internal DirectoryInfo ProjectRootDirectory { get; set; }
        internal Version NewVersion { get; set; }

        internal static Context CreateContext()
        {
            Context context = new Context();
            context.ProjectRootDirectory = FindProjectDirectoryFromCurrentDirectoryUp();

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
