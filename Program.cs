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
                        while (EnterActivity())
                        {
                            //This is empty as the EnterActivity() call returns the boolean value for this
                        }
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

        static bool EnterActivity()
        {
            MySqlConnection conn = null;
            MySqlConnection conn2 = null;
            MySqlConnection conn3 = null;
            bool boolReturn = false;
            try
            {
                //Dictionary is used to ensure that the name corresponds to the activity
                Dictionary<int, string> categories = new Dictionary<int, string>();
                Dictionary<int, string> descriptions = new Dictionary<int, string>();
                Dictionary<int, string> dates = new Dictionary<int, string>();
                string inTransfer, inputLine;
                
                bool numberValid = false, newActivitySelectionMade = false;
                int idValue, categoryChoice, descriptionChoice, dayChoice;
                double timeValue = 0.0;
                conn = new MySqlConnection(cs);
                conn.Open();
                
                string stm = "SELECT activity_category_id, category_description FROM activity_categories";
                MySqlCommand cmd = new MySqlCommand(stm, conn);

                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var inTrans = rdr["activity_category_id"];
                    if (inTrans == null)
                    {
                        inTransfer = "unable to read data";
                    }
                    else
                    {
                        inTransfer = inTrans.ToString();
                    }
                    do
                    {
                        while (!int.TryParse(inTransfer, out idValue))
                        {
                            Console.Write($"Value not recognized as an whole number or already used.\nWhat is the closest whole number value of {inTransfer}: ");
                            inTransfer = Console.ReadLine();
                        }
                    } while (categories.ContainsKey(idValue));
                    categories.Add(idValue, rdr["category_description"] as string);
                }
                categoryChoice = MakeChoice(categories, "Pick a category of activity:\n");
                if (categoryChoice == -1) { return false; }

                //Reset values for new database read
                conn2 = new MySqlConnection(cs);
                conn2.Open();
                stm = "SELECT activity_descriptions_id, activity_description FROM activity_descriptions";
                MySqlCommand cmd2 = new MySqlCommand(stm, conn2);
                MySqlDataReader rdr2 = cmd2.ExecuteReader();
                
                while (rdr2.Read())
                {
                    var inTrans = rdr2["activity_descriptions_id"];
                    if (inTrans == null)
                    {
                        inTransfer = "unable to read data";
                    }
                    else
                    {
                        inTransfer = inTrans.ToString();
                    }
                    do
                    {
                        while (!int.TryParse(inTransfer, out idValue))
                        {
                            Console.Write($"Value not recognized as an whole number or already used.\nWhat is the closest whole number value of {inTransfer}: ");
                            inTransfer = Console.ReadLine();
                        }
                    } while (descriptions.ContainsKey(idValue));
                    descriptions.Add(idValue, rdr2["activity_description"] as string);
                }
                descriptionChoice = MakeChoice(descriptions, "Pick a description of the activity.\n");
                if (descriptionChoice == -1) { return false; }

                //Reset values for new database read
                conn3 = new MySqlConnection(cs);
                conn3.Open();
                stm = "SELECT calendar_date_id, calendar_date FROM tracked_calendar_dates";
                MySqlCommand cmd3 = new MySqlCommand(stm, conn3);
                MySqlDataReader rdr3 = cmd3.ExecuteReader();

                while (rdr3.Read())
                {
                    var inTrans = rdr3["calendar_date_id"];
                    if (inTrans == null)
                    {
                        inTransfer = "unable to read data";
                    }
                    else
                    {
                        inTransfer = inTrans.ToString();
                    }
                    do
                    {
                        while (!int.TryParse(inTransfer, out idValue))
                        {
                            Console.Write($"Value not recognized as an whole number or already used.\nWhat is the closest whole number value of {inTransfer}: ");
                            inTransfer = Console.ReadLine();
                        }
                    } while (dates.ContainsKey(idValue));
                    inTrans = rdr3["calendar_date"];
                    if (inTrans.GetType() == typeof(DateTime))
                    {
                        DateTime temp = (DateTime)inTrans;

                        inTransfer = temp.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        inTransfer = inTrans.ToString();
                    }
                    dates.Add(idValue, inTransfer);
                }
                dayChoice = MakeChoice(dates, "What date did you perform the activity: (Format: Year-Month-Day: 0000-00-00 or select number): ");
                if (dayChoice == -1) { return false; }

                //This section takes in a time value. It verifies that it is time value based on the quarter hour
                while (!numberValid)
                {
                    Console.Clear();
                    Console.WriteLine("How many hours did you perform that activity?\n" +
                        "(Keep in mind that every 15 minutes is 0.25. Must be in 15 minute increments. Format: 0.00)\n" +
                        "1. Back");
                    Console.Write("Time: ");
                    inputLine = Console.ReadLine();

                    if (inputLine == "1" || inputLine.ToLower() == "back")
                    {
                        return false;
                    }
                    while (!double.TryParse(inputLine, out timeValue))
                    {
                        Console.Write("That is not a number value. The value entered must be a number value.\nTime: ");
                        inputLine = Console.ReadLine();
                    }

                    if (Math.Abs((timeValue*4) - (int)(timeValue*4)) < (double.Epsilon * 4))
                    {
                        numberValid = true;
                    }
                    else
                    {
                        Console.WriteLine("The value must be in quarter value increments and entered as a decimal.\n" +
                            "Keep in mind that 15 minutes = 0.25, 30 minutes = 0.5, and 45 minutes = 0.75");
                        Utility.KeyToProceed();
                    }
                }

                Console.WriteLine($"Activity entered:\n" +
                    $"Activity Category: {categories[categoryChoice]}\n" +
                    $"Activity Description: {descriptions[descriptionChoice]}\n" +
                    $"Date of Activity: {dates[dayChoice]}\n" +
                    $"Day of Activity: {dayChoice}\n" +
                    $"Time Spent: {timeValue}");
                Utility.KeyToProceed();

                while (!newActivitySelectionMade)
                {
                    Console.Clear();
                    Console.WriteLine("1. Enter another activity\n" +
                        "2. Back to Main Menu\n");
                    Console.Write("Selection: ");
                    inputLine = Console.ReadLine().ToLower();

                    switch (inputLine)
                    {
                        case "1":
                        case "enter another activity":
                            boolReturn = true;
                            newActivitySelectionMade = true;
                            break;
                        case "2":
                        case "back to main menu":
                            newActivitySelectionMade = true;
                            break;
                        default:
                            Console.WriteLine("That is not a recognized selection.");
                            Utility.KeyToProceed();
                            break;
                    }
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
                if (conn2 != null)
                {
                    conn2.Close();
                }
                if (conn3 != null)
                {
                    conn3.Close();
                }
            }
            return boolReturn;
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

        static int MakeChoice(Dictionary<int,string> input, string callLine)
        {
            bool choiceMade = false;
            string inputLine;
            int intReturn = 0;

            while (!choiceMade)
            {
                Console.Clear();
                Console.WriteLine($"{callLine}\n");
                foreach (KeyValuePair<int, string> kvp in input)
                {
                    Console.WriteLine($"{kvp.Key}. {kvp.Value}");
                }
                Console.WriteLine($"{input.Count + 1}. Back\n");
                Console.Write("Selection: ");
                inputLine = Console.ReadLine().ToLower();

                if (inputLine == $"{input.Count + 1}" || inputLine == "back")
                {
                    return -1;
                }
                foreach (KeyValuePair<int, string> kvp in input)
                {
                    if (inputLine == $"{kvp.Key}" || inputLine == kvp.Value.ToLower())
                    {
                        intReturn = kvp.Key;
                        Console.WriteLine($"{kvp.Value} is the chosen value");
                        Utility.KeyToProceed();
                        choiceMade = true;
                        break;
                    }
                }
                if (!choiceMade)
                {
                    Console.WriteLine("That is not a recognized entry.");
                    Utility.KeyToProceed();
                }
            }
            return intReturn;
        }
    }
}
