using System;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Job.ComponentNotificator
{
    [Export("ComponentNotificatorJob", typeof(IExtension))]
    class Extension : IExtension
    {
        #region IExtension Members

        void IExtension.Initialize(IExtensionHost host)
        {
            host.RegisterJob(new ComponentNotificatorJob());
        }

        void IExtension.Shutdown()
        {
        }

        #endregion
    }
}
