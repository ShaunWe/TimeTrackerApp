using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTrackerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputLine;
            bool programRunnung = true;
            string menu = "1. Enter Activity\n" +
                "2. View Tracked Data\n" +
                "3. Run Calculations\n" +
                "4. Exit";
            

            while (programRunnung)
            {
                Console.Clear();
                Console.WriteLine($"Hello {programRunnung.ToString()}\n\nWhat would you like to do today?\n\n{menu}");
                Console.Write("Selection: ");
                inputLine = Console.ReadLine().ToLower();

                switch (inputLine)
                {
                    case "1":
                    case "enter activity":
                        EnterActivity();
                        break;
                    case "2":
                    case "view tracked data":
                        ViewTrackedData();
                        break;
                    case "3":
                    case "run calculations":
                        RunCalculations();
                        break;
                    case "4":
                    case "exit":
                        programRunnung = false;
                        break;
                    default:
                        Console.WriteLine("That is not a valid input.");
                        Utility.KeyToProceed();
                        break;
                }
            }
        }

        static void EnterActivity()
        {
            Console.Clear();
            Console.WriteLine("Enter Activity.");
            Utility.KeyToProceed();
        }

        static void ViewTrackedData()
        {
            Console.Clear();
            Console.WriteLine("View Tracked Data");
            Utility.KeyToProceed();
        }

        static void RunCalculations()
        {
            Console.Clear();
            Console.WriteLine("Run calculations.");
            Utility.KeyToProceed();
        }
    }
}
