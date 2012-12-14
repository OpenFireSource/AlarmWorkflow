using System;
using System.Collections.Generic;
using AlarmWorkflow.Tools.MakeUpdatePackage.Tasks;

namespace AlarmWorkflow.Tools.MakeUpdatePackage
{
    class Program
    {
        private static Context _context;
        private static List<ITask> _tasks;

        static void Main(string[] args)
        {
            Console.WriteLine(Properties.Resources.EnterVersionStringMessage);

            string versionString = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(versionString))
            {
                return;
            }

            Version version = new Version(versionString);

            _context = Context.CreateContext(version);
            InitializeTasks();
            ExecuteTasks();

            Console.WriteLine();
            Console.WriteLine("Done. Press any key to exit.");
            Console.ReadKey();
        }

        static void InitializeTasks()
        {
            _tasks = new List<ITask>();

            _tasks.Add(new UpdateVersionsTask());
            _tasks.Add(new BuildAllProjectsInRelease());
            _tasks.Add(new CopyFilesToInstallerTempTask());
            _tasks.Add(new ZipFolderTask());
            _tasks.Add(new CleanupTask());
        }

        static void ExecuteTasks()
        {
            foreach (ITask task in _tasks)
            {
                Console.WriteLine();
                Console.WriteLine("Executing task '{0}'...", task.GetType().Name);

                task.Execute(_context);

                Console.WriteLine("Task finished.");
            }
        }
    }
}
