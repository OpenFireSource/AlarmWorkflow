using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Job.SmsJob
{
    [Export("SmsJob", typeof(IExtension))]
    class Extension : IExtension
    {
        #region IExtension Members

        void IExtension.Initialize(IExtensionHost host)
        {
            host.RegisterJob(new SmsJob());
        }

        void IExtension.Shutdown()
        {
        }

        #endregion
    }
}
