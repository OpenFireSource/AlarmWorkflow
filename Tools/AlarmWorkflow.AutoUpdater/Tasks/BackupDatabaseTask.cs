using System;
using System.IO;

namespace AlarmWorkflow.Tools.AutoUpdater.Tasks
{
    class BackupDatabaseTask : ITask
    {
        #region Constants

        private const string OperationStoreDatabaseName = "SQLCEDatabase.sdf";

        #endregion

        #region ITask Members

        void ITask.Execute(TaskArgs args)
        {
            switch (args.Action)
            {
                case TaskArgs.TaskAction.Pre:
                    BackupDatabase(args);
                    break;
                default: break;
            }
        }

        private void BackupDatabase(TaskArgs args)
        {
            string wd = (string)args.Context["WorkingDirectory"];

            string dbOldPath = Path.Combine(wd, OperationStoreDatabaseName);

            if (File.Exists(dbOldPath))
            {
                string dbNewPath = Path.Combine(wd, OperationStoreDatabaseName + "." + new Random(DateTime.Now.Millisecond).Next().ToString() + ".bak");

                Log.Write("Starting backup database '{0}' to '{1}'...", dbOldPath, dbNewPath);

                File.Copy(dbOldPath, dbNewPath);

                Log.Write("Finished backup database.");
            }
            else
            {
                Log.Write("Database file did not exist, no backup needed.");
            }
        }

        #endregion
    }
}
