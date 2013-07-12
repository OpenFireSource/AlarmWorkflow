using System;
using System.Threading;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Diagnostics.Reports;
using AlarmWorkflow.Windows.Service;

namespace AlarmWorkflow.Windows.ServiceConsole
{
    class Program
    {
        private const string ComponentName = "ServiceConsole";

        static void Main(string[] args)
        {
            ErrorReportManager.RegisterAppDomainUnhandledExceptionListener(ComponentName);

            // Print welcome information :-)
            Console.WriteLine("********************************************************");
            Console.WriteLine("*                                                      *");
            Console.WriteLine("*   AlarmWorkflow Service Console                      *");
            Console.WriteLine("*                             FOR DEBUGGING ONLY!      *");
            Console.WriteLine("*                                                      *");
            Console.WriteLine("*        !!! Press ESCAPE to quit safely !!!           *");
            Console.WriteLine("*                                                      *");
            Console.WriteLine("********************************************************");
            Console.WriteLine();
            Console.WriteLine(Properties.Resources.MainStartingService);

            // Catch all unhandled exceptions and display them.
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Logger.Instance.Initialize(ComponentName);

            try
            {
                using (AlarmWorkflowServiceManager service = new AlarmWorkflowServiceManager())
                {
                    service.OnStart();

                    // Wait for user exit
                    while (true)
                    {
                        if (Console.KeyAvailable)
                        {
                            if (Console.ReadKey().Key == ConsoleKey.Escape)
                            {
                                break;
                            }
                        }

                        Thread.Sleep(1);
                    }

                    Console.WriteLine(Properties.Resources.MainShuttingDownService);
                    service.OnStop();
                }
            }
            catch (Exception ex)
            {
                WriteExceptionInformation(ex);

                Console.WriteLine(Properties.Resources.MainFailStartingServiceException);
                Console.ReadKey();
            }
        }

        private static void WriteExceptionInformation(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Properties.Resources.UnhandledExceptionTrace, exception.GetType().FullName, exception.Message, exception.StackTrace);

            Console.ResetColor();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            WriteExceptionInformation((Exception)e.ExceptionObject);
        }
    }
}
