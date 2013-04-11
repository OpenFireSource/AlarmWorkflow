using System;
using System.Threading;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Diagnostics.Reports;

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
            Console.WriteLine("Starting service...");

            // Catch all unhandled exceptions and display them.
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Logger.Instance.Initialize(ComponentName);

            // Initialize the service
            using (var service = new AlarmWorkflow.Windows.Service.AlarmWorkflowServiceManager())
            {
                // Start the service
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

                // Stop service
                service.OnStop();
            }

            Console.WriteLine("Shutting down complete. Press any key to exit.");
            Console.ReadKey();
        }

        private static void WriteExceptionInformation(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("An error occurred while running the service. Error details:");
            Console.WriteLine("Type = {0}", exception.GetType().FullName);
            Console.WriteLine("Message = {0}", exception.Message);
            Console.WriteLine("StackTrace = {0}", exception.StackTrace);
            Console.WriteLine();

            Console.ResetColor();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            WriteExceptionInformation((Exception)e.ExceptionObject);
        }

        #region Event handlers

        // TODO: Use within log4net
        private static void LoggingListener(LogEntry entry)
        {
            ConsoleColor back = ConsoleColor.Black;
            ConsoleColor fore = ConsoleColor.White;

            switch (entry.MessageType)
            {
                case LogType.None:
                case LogType.Info:
                    break;
                case LogType.Console:
                    fore = ConsoleColor.Blue;
                    break;
                case LogType.Debug:
                    fore = ConsoleColor.Cyan;
                    break;
                case LogType.Error:
                    fore = ConsoleColor.DarkYellow;
                    break;
                case LogType.Exception:
                    fore = ConsoleColor.Red;
                    break;
                case LogType.Trace:
                    fore = ConsoleColor.Gray;
                    break;
                case LogType.Warning:
                    fore = ConsoleColor.Yellow;
                    break;
                default:
                    break;
            }

            Console.BackgroundColor = back;
            Console.ForegroundColor = fore;
            Console.WriteLine("{0}", entry.Message);
        }

        #endregion
    }
}
