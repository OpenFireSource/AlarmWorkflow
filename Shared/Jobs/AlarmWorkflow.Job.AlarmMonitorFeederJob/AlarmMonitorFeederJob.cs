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

using System.IO;
using System.Text;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Engine;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Job.AlarmMonitorFeederJob
{
    [Export("AlarmMonitorFeederJob", typeof(IJob))]
    [Information(DisplayName = "ExportJobDisplayName", Description = "ExportJobDescription")]
    class AlarmMonitorFeederJob : IJob
    {
        #region Fields

        private string _alarmTextFileName;

        #endregion

        #region IJob Members

        void IJob.Execute(IJobContext context, Operation operation)
        {
            if (context.Phase != JobPhase.AfterOperationStored)
            {
                return;
            }

            UTF8Encoding encoding = new UTF8Encoding(false);
            using (StreamWriter sw = new StreamWriter(_alarmTextFileName, false, encoding))
            {
                sw.WriteLine(operation.OperationNumber);
                sw.WriteLine(operation.Einsatzort.City);
                sw.WriteLine(operation.Einsatzort.Street);
                sw.WriteLine(operation.Keywords.EmergencyKeyword);
                sw.WriteLine(operation.Keywords.Keyword);                
                sw.WriteLine(operation.Picture);
                sw.WriteLine(operation.Comment);
                sw.WriteLine(operation.OperationPlan);
                sw.WriteLine(operation.Einsatzort.Location);
                sw.WriteLine(operation.Einsatzort.Intersection);                
                sw.WriteLine(operation.Messenger);
                sw.WriteLine(operation.Einsatzort.Property);                
                sw.WriteLine(operation.Resources.ToString("{FullName} | {RequestedEquipment}", null));                
            }
        }

        bool IJob.Initialize()
        {
            _alarmTextFileName = SettingsManager.Instance.GetSetting("AlarmMonitorFeederJob", "DestinationFileName").GetString();
            return true;
        }
        
        bool IJob.IsAsync
        {
            get { return false; }
        }

        #endregion

        #region IDisposable Members

        void System.IDisposable.Dispose()
        {

        }

        #endregion
    }
}