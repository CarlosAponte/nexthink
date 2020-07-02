using System;
using System.Collections.Generic;
using DataHandlerLib.Readers;
using DataHandlerLib.Models;

namespace DataHandlerConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            List<MonitorInfo> monitors = null;
            do
            {
                Console.Clear();
                Console.WriteLine("Please select your input data type:");
                Console.WriteLine("\n1 CVS \n2 XML \n3 Exit");
                var inputType = int.TryParse(Console.ReadLine(), out int option);
                switch (option)
                {
                    case 1:
                        var xmlReader = new XmlReader();
                        monitors = xmlReader.ReadData(@"Data\data.xml");
                        break;

                    case 2:
                        var csvReader = new CsvReader();
                        monitors = csvReader.ReadData(@"Data\data.csv");
                        break;

                    case 3:
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("\nError: Please enter a valid data type.");
                        monitors = null;
                        break;
                }
                if (monitors != null)
                {
                    foreach (var monitor in monitors)
                    {
                        Console.WriteLine($"Name: {monitor.Name} - IP Address: {monitor.IPAddress}");
                    }
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

            } while (true);

        }
    }
}
