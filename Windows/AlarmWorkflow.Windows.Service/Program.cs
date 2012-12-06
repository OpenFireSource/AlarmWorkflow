using System;
using System.Configuration.Install;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;

namespace AlarmWorkflow.Windows.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (Debugger.IsAttached)
            {
                AlarmWorkflowService service = new AlarmWorkflowService();
                service.OnStart();
                service.Stop();
                service.Dispose();
                return;
            }

            if (Environment.UserInteractive)
            {
                string parameter = string.Concat(args);
                switch (parameter)
                {
                    case "--install":
                        InstallService();
                        break;
                    case "--uninstall":
                        UnInstallService();
                        break;
                }
            }
            else
            {
                ServiceBase.Run(new AlarmWorkflowService());
            }
        }

        private static void InstallService()
        {
            UnInstallService();
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format(Properties.Resources.InstallServiceError_Message, ex.Message));
            }
        }

        private static void UnInstallService()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format(Properties.Resources.UnInstallServiceError_Message, ex.Message));
            }
        }
    }
}
