using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Job.MailingJob
{
    [Export("MailingJob", typeof(IExtension))]
    class Extension : IExtension
    {
        #region IExtension Members

        void IExtension.Initialize(IExtensionHost host)
        {
            host.RegisterJob(new MailingJob());
        }

        void IExtension.Shutdown()
        {
        }

        #endregion
    }
}
