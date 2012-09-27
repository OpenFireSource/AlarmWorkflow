using System;
using System.Threading;
using AlarmWorkflow.Shared;

namespace AlarmWorkflow.Windows.ServiceConsole
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;

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

            AlarmworkflowClass ac = new AlarmworkflowClass();
            try
            {
                ac.Start();
                Console.WriteLine("Service started.");

                while (true)
                {
                    if (Console.KeyAvailable)
                    {
                        if (Console.ReadKey().Key == ConsoleKey.Escape)
                        {
                            Console.WriteLine("Shutting down service...");
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
                if (ac != null)
                {
                    ac.Stop();
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
