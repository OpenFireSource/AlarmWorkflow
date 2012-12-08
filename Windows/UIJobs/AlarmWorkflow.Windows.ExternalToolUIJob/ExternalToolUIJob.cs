using System;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UIContracts.Extensibility;

namespace AlarmWorkflow.Windows.ExternalToolUIJob
{
    [Export("ExternalToolUIJob", typeof(IUIJob))]
    class ExternalToolUIJob : IUIJob
    {
        #region IUIJob Members

        bool IUIJob.Initialize()
        {
            return true;
        }

        bool IUIJob.IsAsync
        {
            get { return true; }
        }

        void IUIJob.OnNewOperation(IOperationViewer operationViewer, Operation operation)
        {

        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {

        }

        #endregion
    }
}
