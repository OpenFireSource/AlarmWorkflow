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
using System.Threading.Tasks;
using AlarmWorkflow.BackendService.EngineContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.BackendService.SettingsContracts;

namespace AlarmWorkflow.Job.ExternalTool
{
    [Export(nameof(ExternalTool), typeof(IJob))]
    public class ExternalTool : IJob
    {
        private const string CreatingProgramFailed = "Starting the program '{0}'  has failed. Please see log for further information.";
        private ISettingsServiceInternal _settings;

        #region Implementation of IDisposable

        public void Dispose()
        {
            _settings.Dispose();
        }

        #endregion

        #region Implementation of IJob

        public bool Initialize(IServiceProvider serviceProvider)
        {
            _settings = serviceProvider.GetService<ISettingsServiceInternal>();
            return false;
        }

        public void Execute(IJobContext context, Operation operation)
        {
            if (context.Phase == JobPhase.OnOperationSurfaced)
            {
                string[] programs = _settings.GetSetting(SettingKeys.ExternalTool).GetStringArray();
                foreach (string program in programs)
                {
                    try
                    {
                        Task.Factory.StartNew(() => ProgramStarter.StartProgramTask(program, this));
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.LogFormat(LogType.Error, this, CreatingProgramFailed, program);
                        Logger.Instance.LogException(this, ex);
                    }
                }
            }
        }

        public bool IsAsync => true;

        #endregion
    }
}
