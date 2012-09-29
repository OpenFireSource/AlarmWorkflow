using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Job.MySqlDatabaseJob
{
    [Export("MySqlDatabaseJob", typeof(IExtension))]
    class Extension : IExtension
    {
        #region IExtension Members

        void IExtension.Initialize(IExtensionHost host)
        {
            host.RegisterJob(new DatabaseJob());
        }

        void IExtension.Shutdown()
        {
        }

        #endregion
    }
}
