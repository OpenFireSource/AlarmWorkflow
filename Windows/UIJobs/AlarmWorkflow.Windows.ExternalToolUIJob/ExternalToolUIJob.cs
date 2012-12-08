using System;
using System.Diagnostics;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Settings;
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
            String[] programms =
                SettingsManager.Instance.GetSetting("ExternalToolUIJob", "ExternalTool")
                             .GetStringArray();
            foreach (var programm in programms)
            {
                Process.Start(programm);
            }

        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {

        }

        #endregion
    }
}
