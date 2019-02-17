using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace TimeTrackerApp
{
    class Program
    {
        /*
         * Shaun Wehe
         * Time Tracker App
         * Project and Portfolio 2
         * February 17, 2019
         */
        static string cs = @"server=10.0.0.101;userid=dbremoteuser;password=password;database=ShaunWehe_MDV229_Database_201902;port=8889";

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
                Console.WriteLine($"Hello {GetUserName()}\n\nWhat would you like to do today?\n\n{menu}");
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

        static string GetUserName()
        {
            string userName = "";
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(cs);
                conn.Open();
                MySqlDataReader rdr = null;

                string stm = "SELECT user_firstname FROM time_tracker_users WHERE user_id = @userid LIMIT 1";
                MySqlCommand cmd = new MySqlCommand(stm, conn);

                cmd.Parameters.AddWithValue("@userid", "1");
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    userName = rdr["user_firstname"] as string;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error: {ex.ToString()}");
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            return userName;
        }

        static void EnterActivity()
        {
            MySqlConnection conn = null;
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
