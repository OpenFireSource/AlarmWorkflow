
namespace AlarmWorkflow.Tools.MakeUpdatePackage.Tasks
{
    class CleanupTask : ITask
    {
        #region ITask Members

        void ITask.Execute(Context context)
        {
            context.InstallerTempDirectory.Delete(true);
        }

        #endregion
    }
}
