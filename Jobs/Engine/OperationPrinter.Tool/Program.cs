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
using System.Linq;
using System.Threading;
using AlarmWorkflow.BackendService.EngineContracts;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Job.OperationPrinter.Tool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("".PadRight(16, '*'));
            Console.WriteLine($"{Environment.NewLine}This tool is only for test purpose!{Environment.NewLine}It generates a 'test operation' and uses the the default printer for printing the template located under 'Resources\\OperationPrintTemplate_NonStatic.htm'{Environment.NewLine}{Environment.NewLine}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($" --> Press 'enter' to continue. <-- {Environment.NewLine}");
            Console.ResetColor();
            Console.WriteLine("".PadRight(16, '*'));
            ConsoleKeyInfo key = Console.ReadKey(false);

            if (key.Key != ConsoleKey.Enter)
            {
                return;
            }

            Console.WriteLine($"Please be patient...{Environment.NewLine}Job is getting executed! It can take a few seconds until something happens.");

            IJob job = ExportedTypeLibrary.GetExports(typeof(IJob)).SingleOrDefault(x => x.Attribute.Alias == "OperationPrinterJob").CreateInstance<IJob>();
            job.Initialize(new FakeSettingsService());
            FakeContext c = new FakeContext { Phase = JobPhase.AfterOperationStored };
            job.Execute(c, new Operation
            {
                Comment = "Testeinsatz für die Feuerwehr Musterstadt",
                Picture = "Brand Einfamilienhaus",
                Einsatzort = new PropertyLocation
                {
                    Street = "Karlstraße",
                    StreetNumber = "5",
                    City = "München",
                    ZipCode = "80335",
                    GeoLatitude = 48.142792,
                    GeoLongitude = 11.567208
                },
                Messenger = "Donald Duck",
                OperationNumber = "B 1.0 123456 1234",
                Timestamp = DateTime.Now,
                TimestampIncome = DateTime.Now,
                OperationPlan = "Plan 42",
                Priority = "Prio 1",
                Resources = { new OperationResource { FullName = "Entenhausen 11/1" }, new OperationResource { FullName = "Musterstadt 40/1", RequestedEquipment = { "Geräteträger" }, Timestamp = DateTime.Now.ToString("HH:mm:ss") } },
                Keywords = new OperationKeywords { B = "B3", Keyword = "Brand Wohnhaus", EmergencyKeyword = "Brand Person in Gefahr" }

            });
            Thread.Sleep(1000);
        }
    }
}
