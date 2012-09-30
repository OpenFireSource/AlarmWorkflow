using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Job.SQLCEDatabaseJob
{
    [Export("Extension", typeof(IExtension))]
    class Extension : IExtension
    {
        #region IExtension Members

        void IExtension.Initialize(IExtensionHost host)
        {
            // We use the same type for both
            SQLCEDatabaseJob job = new SQLCEDatabaseJob();
            host.RegisterJob(job);
            host.RegisterOperationStore(job);
        }

        void IExtension.Shutdown()
        {

        }

        #endregion
    }
}
