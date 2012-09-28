using System;
using System.Threading;
using AlarmWorkflow.Shared;
using AlarmWorkflow.Windows.Service.WcfServices;

namespace AlarmWorkflow.Windows.ServiceConsole
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
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

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            using (AlarmworkflowClass ac = new AlarmworkflowClass())
            {
                WcfServicesHostManager shm = new WcfServicesHostManager();


                try
                {
                    ac.Start();
                    Console.WriteLine("Service started.");

                    shm.Initialize();
                    Console.WriteLine("Web Services hosted.");

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
                }
                catch (Exception ex)
                {
                    WriteExceptionInformation(ex);
                    Console.ReadLine();
                }
                finally
                {
                    Console.WriteLine("Shutting down service...");
                    ac.Stop();
                    Console.WriteLine("Shutting down Web Services...");
                    shm.Shutdown();
                }
            }

            Console.WriteLine("Shutting down complete. Quitting...");
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
    }
}
