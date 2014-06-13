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
using System.Threading.Tasks;
using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.ExternalToolUIJob.Properties;
using AlarmWorkflow.Windows.UIContracts.Extensibility;

namespace AlarmWorkflow.Windows.ExternalToolUIJob
{
    [Export("ExternalToolUIIdleJob", typeof(IIdleUIJob))]
    class ExternalToolUIIdleJob : IIdleUIJob
    {
        #region IUIJob Members

        bool IIdleUIJob.Initialize()
        {
            return true;
        }

        bool IIdleUIJob.IsAsync
        {
            get { return true; }
        }

        void IIdleUIJob.Run()
        {
            string[] programs;
            using (var service = ServiceFactory.GetCallbackServiceWrapper<ISettingsService>(new SettingsServiceCallback()))
            {
                programs = service.Instance.GetSetting(SettingKeys.ExternalToolIdle).GetStringArray();
            }

            foreach (string program in programs)
            {
                try
                {
                    Task.Factory.StartNew(() => Starter.StartProgramTask(program, this));
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, this, Resources.CreatingProgramFailed, program);
                    Logger.Instance.LogException(this, ex);
                }
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
