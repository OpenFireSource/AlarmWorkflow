using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AlarmWorkflow.Tools.AutoUpdater.Tasks
{
    class StopProcessesTask : ITask
    {

        #region ITask Members

        void ITask.Execute(TaskArgs args)
        {
            switch (args.Action)
            {
                case TaskArgs.TaskAction.Pre:
                    StopProcesses();
                    break;
                default:
                    break;
            }
        }

        private void StopProcesses()
        {
            IEnumerable<Process> runningProccess = GetRunningAlarmProccess();
            foreach (Process p in runningProccess)
            {
                p.Close();
            }
        }

        private IEnumerable<Process> GetRunningAlarmProccess()
        {
            return Process.GetProcesses()
                .Where(p => p.ProcessName.ToLower().Contains("alarmworkflow") && p.ProcessName != Process.GetCurrentProcess().ProcessName);
        }

        #endregion

    }
}
