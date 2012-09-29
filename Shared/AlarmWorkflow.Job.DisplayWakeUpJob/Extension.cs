using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Job.DisplayWakeUpJob
{
    [Export("DisplayWakeUpJob", typeof(IExtension))]
    class Extension : IExtension
    {
        #region IExtension Members

        void IExtension.Initialize(IExtensionHost host)
        {
            host.RegisterJob(new DisplayWakeUp());
        }

        void IExtension.Shutdown()
        {
        }

        #endregion
    }
}
