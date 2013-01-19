using System.Diagnostics;
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

            ServiceBase.Run(new AlarmWorkflowService());
        }
    }
}
