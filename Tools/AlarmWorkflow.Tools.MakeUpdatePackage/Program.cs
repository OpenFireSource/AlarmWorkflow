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
            Version version = new Version(versionString);

            _context = Context.CreateContext();
            _context.NewVersion = version;
            InitializeTasks();
            ExecuteTasks();
        }

        static void InitializeTasks()
        {
            _tasks = new List<ITask>();

            _tasks.Add(new UpdateVersionsTask());
            _tasks.Add(new BuildAllProjectsInRelease());
        }

        static void ExecuteTasks()
        {
            foreach (ITask task in _tasks)
            {
                task.Execute(_context);
            }
        }
    }
}
