// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

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