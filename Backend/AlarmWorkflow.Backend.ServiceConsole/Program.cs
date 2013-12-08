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

using System;
using System.Threading;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Diagnostics.Reports;
using AlarmWorkflow.Backend.Service;

namespace AlarmWorkflow.Backend.ServiceConsole
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
                    service.OnStart(args);

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