using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Windows.Forms;

namespace AlarmWorkflow.Tools.AutoUpdater.Tasks
{
    class StartStopServiceTask : ITask
    {
        #region ITask Members

        void ITask.Execute(TaskArgs args)
        {
            switch (args.Action)
            {
                case TaskArgs.TaskAction.Pre:
                    StopSerivce();
                    break;
                case TaskArgs.TaskAction.Post:
                    InstallService();
                    break;
                default:
                    break;
            }
        }

        private void StopSerivce()
        {
            StartStopService(false);
        }

        private void InstallService()
        {
            // TODO: This will install the service, but does not start it!
            ProcessStartInfo serviceInstall = new ProcessStartInfo
            {
                CreateNoWindow = false,
                FileName = Path.Combine(Application.StartupPath, "AlarmWorkflow.Windows.Service.exe"),
                Arguments = "--install"
            };
            Process.Start(serviceInstall).WaitForExit();

            StartStopService(true);
        }

        private void StartStopService(bool state)
        {
            ServiceController service = new ServiceController("AlarmworkflowService");
            try
            {
                if (state)
                {
                    Log.Write("Starting service...");

                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);

                    Log.Write("Service started.");
                }
                else
                {
                    Log.Write("Stopping service...");

                    if (service.Status == ServiceControllerStatus.Running)
                    {
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped);
                    }
                    service.Close();

                    Log.Write("Service stopped.");
                }
            }
            catch (InvalidOperationException)
            {
                // This exception is ok - it occurs if the service does not exist
            }
        }

        #endregion
    }
}
