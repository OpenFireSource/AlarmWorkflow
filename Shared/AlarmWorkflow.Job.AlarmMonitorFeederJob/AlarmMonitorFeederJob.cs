using System.IO;
using System.Text;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Job.AlarmMonitorFeederJob
{
    [Export("AlarmMonitorFeederJob", typeof(IJob))]
    class AlarmMonitorFeederJob : IJob
    {
        #region Fields

        private string _alarmTextFileName;

        #endregion

        #region IJob Members

        void IJob.DoJob(Operation operation)
        {
            UTF8Encoding encoding = new UTF8Encoding(false);
            using (StreamWriter sw = new StreamWriter(_alarmTextFileName, false, encoding))
            {
                sw.WriteLine(operation.OperationNumber);
                sw.WriteLine(operation.Location);
                sw.WriteLine(operation.OperationPlan);
                sw.WriteLine(operation.Comment);
                sw.WriteLine(operation.Intersection);
                sw.WriteLine(operation.Picture);
                sw.WriteLine(operation.Messenger);
                sw.WriteLine(operation.Property);
                sw.WriteLine(operation.City);
                sw.WriteLine(operation.Street);
                sw.WriteLine(operation.Vehicles);
                sw.WriteLine(operation.EmergencyKeyword);
                sw.WriteLine(operation.Keyword);
            }
        }

        bool IJob.Initialize()
        {
            _alarmTextFileName = SettingsManager.Instance.GetSetting("AlarmMonitorFeederJob", "DestinationFileName").GetString();
            return true;
        }

        #endregion
    }
}
