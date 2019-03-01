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
            MySqlConnection conn4 = null;
            MySqlConnection conn5 = null;
            MySqlConnection conn6 = null;
            bool boolReturn = false;
            try
            {
                //Dictionary is used to ensure that the name corresponds to the activity
                Dictionary<int, string> categories = new Dictionary<int, string>();
                Dictionary<int, string> descriptions = new Dictionary<int, string>();
                Dictionary<int, string> dates = new Dictionary<int, string>();
                Dictionary<int, string> days = new Dictionary<int, string>();
                Dictionary<int, string> times = new Dictionary<int, string>();
                string inTransfer, inputLine;
                
                bool newActivitySelectionMade = false;
                int idValue, categoryChoice, descriptionChoice, dayChoice, dayOfWeekChoice, timeChoice;
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
                descriptionChoice = MakeChoice(descriptions, "Pick a description of the activity:\n");
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

                //Reset values for new database read
                conn4 = new MySqlConnection(cs);
                conn4.Open();
                stm = "SELECT day_id, day_name FROM days_of_week";
                MySqlCommand cmd4 = new MySqlCommand(stm, conn4);
                MySqlDataReader rdr4 = cmd4.ExecuteReader();

                while (rdr4.Read())
                {
                    var inTrans = rdr4["day_id"];
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
                    } while (days.ContainsKey(idValue));
                    days.Add(idValue, rdr4["day_name"] as string);
                }
                dayOfWeekChoice = MakeChoice(days, "What day of the week was the activity on: ");

                //Reset values for new database read
                conn5 = new MySqlConnection(cs);
                conn5.Open();
                stm = "SELECT activity_time_id, time_spent_on_activity FROM activity_times";
                MySqlCommand cmd5 = new MySqlCommand(stm, conn5);
                MySqlDataReader rdr5 = cmd5.ExecuteReader();
                double transDouble;

                while (rdr5.Read())
                {
                    var inTrans = rdr5["activity_time_id"];
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
                    } while (times.ContainsKey(idValue));
                    inTrans = rdr5["time_spent_on_activity"];
                    if (inTrans == null)
                    {
                        inTransfer = "unable to read data";
                    }
                    else
                    {
                        inTransfer = inTrans.ToString();
                    }
                    while (!double.TryParse(inTransfer, out transDouble))
                    {
                        Console.Write($"Value not recognized as a number.\nWhat is the number value of {inTransfer}: ");
                        inTransfer = Console.ReadLine();
                    }
                    times.Add(idValue, transDouble.ToString());
                }
                timeChoice = MakeChoice(times, "How much time did you spend on the activity: ");


                conn6 = new MySqlConnection(cs);
                stm = "INSERT INTO activity_log (user_id, calendar_day, calendar_date, day_name, category_description, activity_description, time_spent_on_activity) " +
                    "VALUES (1, @calendarDay, @calendarDate, @dayName, @categoryDescription, @activityDescription, @timeSpentOnActivity)";
                conn6.Open();

                MySqlCommand cmd6 = new MySqlCommand(stm, conn6);
                cmd6.Parameters.AddWithValue("@calendarDay", dayChoice);
                cmd6.Parameters.AddWithValue("@calendarDate", dayChoice);
                cmd6.Parameters.AddWithValue("@dayName", dayOfWeekChoice);
                cmd6.Parameters.AddWithValue("@categoryDescription", categoryChoice);
                cmd6.Parameters.AddWithValue("@activityDescription", descriptionChoice);
                cmd6.Parameters.AddWithValue("@timeSpentOnActivity", timeChoice);

                MySqlDataReader rdr6 = cmd6.ExecuteReader();
                

                Console.WriteLine($"Activity entered:\n" +
                        $"Activity Category: {categories[categoryChoice]}\n" +
                        $"Activity Description: {descriptions[descriptionChoice]}\n" +
                        $"Date of Activity: {dates[dayChoice]}\n" +
                        $"Day of Activity: {dayChoice}\n" +
                        $"Day of Week: {days[dayOfWeekChoice]}\n" +
                        $"Time Spent: {times[timeChoice]}");
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
                if (conn4 != null)
                {
                    conn4.Close();
                }
                if (conn5 != null)
                {
                    conn5.Close();
                }
                if (conn6 != null)
                {
                    conn6.Close();
                }
            }
            return boolReturn;
        }

        static bool EnterActivity(int calendar_date_id)
        {
            MySqlConnection conn = null;
            MySqlConnection conn2 = null;
            MySqlConnection conn3 = null;
            MySqlConnection conn4 = null;
            MySqlConnection conn5 = null;
            MySqlConnection conn6 = null;
            bool boolReturn = false;
            try
            {
                //Dictionary is used to ensure that the name corresponds to the activity
                Dictionary<int, string> categories = new Dictionary<int, string>();
                Dictionary<int, string> descriptions = new Dictionary<int, string>();
                Dictionary<int, string> dates = new Dictionary<int, string>();
                Dictionary<int, string> days = new Dictionary<int, string>();
                Dictionary<int, string> times = new Dictionary<int, string>();
                string inTransfer, inputLine;

                bool newActivitySelectionMade = false;
                int idValue, categoryChoice, descriptionChoice, dayChoice, dayOfWeekChoice, timeChoice;
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
                descriptionChoice = MakeChoice(descriptions, "Pick a description of the activity:\n");
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
                //Calendar date was passed in
                dayChoice = calendar_date_id;

                //Reset values for new database read
                conn4 = new MySqlConnection(cs);
                conn4.Open();
                stm = "SELECT day_id, day_name FROM days_of_week";
                MySqlCommand cmd4 = new MySqlCommand(stm, conn4);
                MySqlDataReader rdr4 = cmd4.ExecuteReader();

                while (rdr4.Read())
                {
                    var inTrans = rdr4["day_id"];
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
                    } while (days.ContainsKey(idValue));
                    days.Add(idValue, rdr4["day_name"] as string);
                }
                dayOfWeekChoice = MakeChoice(days, "What day of the week was the activity on: ");

                //Reset values for new database read
                conn5 = new MySqlConnection(cs);
                conn5.Open();
                stm = "SELECT activity_time_id, time_spent_on_activity FROM activity_times";
                MySqlCommand cmd5 = new MySqlCommand(stm, conn5);
                MySqlDataReader rdr5 = cmd5.ExecuteReader();
                double transDouble;

                while (rdr5.Read())
                {
                    var inTrans = rdr5["activity_time_id"];
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
                    } while (times.ContainsKey(idValue));
                    inTrans = rdr5["time_spent_on_activity"];
                    if (inTrans == null)
                    {
                        inTransfer = "unable to read data";
                    }
                    else
                    {
                        inTransfer = inTrans.ToString();
                    }
                    while (!double.TryParse(inTransfer, out transDouble))
                    {
                        Console.Write($"Value not recognized as a number.\nWhat is the number value of {inTransfer}: ");
                        inTransfer = Console.ReadLine();
                    }
                    times.Add(idValue, transDouble.ToString());
                }
                timeChoice = MakeChoice(times, "How much time did you spend on the activity: ");

                conn6 = new MySqlConnection(cs);
                stm = "INSERT INTO activity_log (user_id, calendar_day, calendar_date, day_name, category_description, activity_description, time_spent_on_activity) " +
                    "VALUES (1, @calendarDay, @calendarDate, @dayName, @categoryDescription, @activityDescription, @timeSpentOnActivity)";
                conn6.Open();

                MySqlCommand cmd6 = new MySqlCommand(stm, conn6);
                
                cmd6.Parameters.AddWithValue("@calendarDay", dayChoice);
                cmd6.Parameters.AddWithValue("@calendarDate", dayChoice);
                cmd6.Parameters.AddWithValue("@dayName", dayOfWeekChoice);
                cmd6.Parameters.AddWithValue("@categoryDescription", categoryChoice);
                cmd6.Parameters.AddWithValue("@activityDescription", descriptionChoice);
                cmd6.Parameters.AddWithValue("@timeSpentOnActivity", timeChoice);
                
                MySqlDataReader rdr6 = cmd6.ExecuteReader();

                Console.WriteLine($"Activity entered:\n" +
                        $"Activity Category: {categories[categoryChoice]}\n" +
                        $"Activity Description: {descriptions[descriptionChoice]}\n" +
                        $"Date of Activity: {dates[dayChoice]}\n" +
                        $"Day of Activity: {dayChoice}\n" +
                        $"Day of Week: {days[dayOfWeekChoice]}\n" +
                        $"Time Spent: {times[timeChoice]}");
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
                if (conn4 != null)
                {
                    conn4.Close();
                }
                if (conn5 != null)
                {
                    conn5.Close();
                }
                if (conn6 != null)
                {
                    conn6.Close();
                }
            }
            return boolReturn;
        }

        static void ViewTrackedData()
        {
            bool menuRunning = true;
            string menu = "1. Select by Date\n" +
                "2. Select by Category\n" +
                "3. Select by Description\n" +
                "4. Back";
            string inputLine;

            while (menuRunning)
            {
                Console.Clear();
                Console.WriteLine(menu);
                Console.Write("\nSelection: ");
                inputLine = Console.ReadLine().ToLower();

                switch (inputLine)
                {
                    case "1":
                    case "select by date":
                    case "date":
                        Dictionary<int, string> dates = PullCalendarData();
                        bool selectionMade = false;

                        Console.Clear();
                        while (!selectionMade)
                        {
                            Console.WriteLine("Which date would you like to view?:");
                            foreach (KeyValuePair<int, string> kvp in dates)
                            {
                                Console.WriteLine($"{kvp.Key}. {kvp.Value}");
                            }
                            Console.WriteLine($"{dates.Count + 1}. Back");
                            Console.Write("Selection: ");
                            inputLine = Console.ReadLine().ToLower();

                            if (inputLine == $"{dates.Count + 1}" || inputLine == "back")
                            {
                                selectionMade = true;
                                break;
                            }
                            foreach (KeyValuePair<int, string> kvp in dates)
                            {
                                if (inputLine == $"{kvp.Key}" || inputLine == $"{kvp.Value.ToLower()}")
                                {
                                    ViewDateData(kvp.Key);
                                    selectionMade = true;
                                }
                            }
                            if (!selectionMade)
                            {
                                Console.WriteLine("Input not recognized");
                                Utility.KeyToProceed();
                            }
                        }
                        break;
                    case "2":
                    case "select by category":
                    case "category":
                        Dictionary<int, string> categories = PullData("activity_category_id", "category_description", "activity_categories");
                        selectionMade = false;

                        Console.Clear();
                        while (!selectionMade)
                        {
                            Console.WriteLine("Which category would you like to view?:");
                            foreach (KeyValuePair<int, string> kvp in categories)
                            {
                                Console.WriteLine($"{kvp.Key}. {kvp.Value}");
                            }
                            Console.WriteLine($"{categories.Count + 1}. Back");
                            Console.Write("Selection: ");
                            inputLine = Console.ReadLine().ToLower();

                            if (inputLine == $"{categories.Count + 1}" || inputLine == "back")
                            {
                                selectionMade = true;
                                break;
                            }
                            foreach (KeyValuePair<int, string> kvp in categories)
                            {
                                if (inputLine == $"{kvp.Key}" || inputLine == $"{kvp.Value.ToLower()}")
                                {
                                    ViewData("activity_log.category_description", kvp.Key, kvp.Value);
                                    selectionMade = true;
                                }
                            }
                            if (!selectionMade)
                            {
                                Console.WriteLine("Input not recognized");
                                Utility.KeyToProceed();
                            }
                        }
                        break;
                    case "3":
                    case "select by description":
                    case "description":
                        Dictionary<int, string> descriptions = PullData("activity_descriptions_id", "activity_description", "activity_descriptions");
                        selectionMade = false;

                        Console.Clear();
                        while (!selectionMade)
                        {
                            Console.WriteLine("Which description would you like to view?:");
                            foreach (KeyValuePair<int, string> kvp in descriptions)
                            {
                                Console.WriteLine($"{kvp.Key}. {kvp.Value}");
                            }
                            Console.WriteLine($"{descriptions.Count + 1}. Back");
                            Console.Write("Selection: ");
                            inputLine = Console.ReadLine().ToLower();

                            if (inputLine == $"{descriptions.Count + 1}" || inputLine == "back")
                            {
                                selectionMade = true;
                                break;
                            }
                            foreach (KeyValuePair<int, string> kvp in descriptions)
                            {
                                if (inputLine == $"{kvp.Key}" || inputLine == $"{kvp.Value.ToLower()}")
                                {
                                    ViewData("activity_log.activity_description", kvp.Key, kvp.Value);
                                    selectionMade = true;
                                }
                            }
                            if (!selectionMade)
                            {
                                Console.WriteLine("Input not recognized");
                                Utility.KeyToProceed();
                            }
                        }
                        break;
                    case "4":
                    case "back":
                        menuRunning = false;
                        break;
                    default:
                        Console.WriteLine("Input not recognized");
                        Utility.KeyToProceed();
                        break;
                }
            }
        }

        static void RunCalculations()
        {
            Console.Clear();
            Console.WriteLine($"1. The amount of time spent on moving tasks this month was {TimeSpent("WHERE category_description = 8")} hours.\n" +
                $"2. The percentage of time spent sleeping this month was {(TimeSpent("WHERE category_description = 4")/TotalHoursInDatabase()).ToString("P")}\n" +
                $"3. The amount of time spent eating this month was {TimeSpent("WHERE category_description = 7")} hours.\n" +
                $"4. The percentage of time spent this month working on projects was {(TimeSpent("WHERE activity_description = 9")/TotalHoursInDatabase()).ToString("P")}\n" +
                $"5. The amount of time spent on travel and driving was {TimeSpent("WHERE activity_description = 4")+ TimeSpent("WHERE activity_description = 11")} hours.\n" +
                $"6. The percentage of time spent on school not including sleeping and work was {((TimeSpent("WHERE category_description = 2")+ TimeSpent("WHERE category_description = 3"))/((TotalHoursInDatabase()-TimeSpent("WHERE category_description = 4"))- TimeSpent("WHERE category_description = 1"))).ToString("P")}\n" +
                $"7. The amount of time spent on the well being of the wife and kids was {TimeSpent("WHERE category_description = 9")+ TimeSpent("WHERE category_description = 10")} hours.\n" +
                $"8. The percentage of time spent on movies, watching TV, or playing games was {((TimeSpent("WHERE activity_description = 1")+ TimeSpent("WHERE activity_description = 2")+ TimeSpent("WHERE activity_description = 3"))/TotalHoursInDatabase()).ToString("P")}\n" +
                $"9. The amount of time spent on relaxation this month was {TimeSpent("WHERE category_description = 6")} hours.\n" +
                $"10. The percentage of time spent eating breakfast in relation to all eating activities is {(TimeSpent("WHERE activity_description = 5")/TimeSpent("WHERE category_description = 7")).ToString("P")}");
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

        static void ViewDateData(int dateID)
        {
            using (MySqlConnection conn = new MySqlConnection(cs))
            {
                conn.Open();
                string stm = $"SELECT activity_categories.category_description AS Category, activity_descriptions.activity_description AS Description, activity_times.time_spent_on_activity AS TimeSpent " +
                    $"FROM activity_log " +
                    $"LEFT JOIN activity_categories ON activity_log.category_description = activity_categories.activity_category_id " +
                    $"LEFT JOIN activity_descriptions ON activity_log.activity_description = activity_descriptions.activity_descriptions_id " +
                    $"LEFT JOIN activity_times ON activity_log.time_spent_on_activity = activity_times.activity_time_id " +
                    $"WHERE calendar_date = {dateID}";
                MySqlCommand cmd = new MySqlCommand(stm, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                List<string> categories = new List<string>();
                List<string> descriptions = new List<string>();
                List<double> timeSpent = new List<double>();
                double transDouble, timeTracked, timeNotTracked;
                string inTransfer, inputLine;
                bool answerProvided = false;

                while (rdr.Read())
                {
                    categories.Add(rdr["Category"] as string);
                    descriptions.Add(rdr["Description"] as string);
                    var inTrans = rdr["TimeSpent"];
                    if (inTrans == null)
                    {
                        inTransfer = "unable to read data";
                    }
                    else
                    {
                        inTransfer = inTrans.ToString();
                    }
                    while (!double.TryParse(inTransfer, out transDouble))
                    {
                        Console.Write($"Value not recognized as a number.\nWhat is the number value of {inTransfer}: ");
                        inTransfer = Console.ReadLine();
                    }
                    timeSpent.Add(transDouble);
                }
                timeTracked = timeSpent.Sum();
                timeNotTracked = 24 - timeSpent.Sum();
                Console.Clear();
                for (int i = 0; i < categories.Count; i++)
                {
                    Console.WriteLine($"Category: {categories[i]}\t\tDescription: {descriptions[i]}\t\tTime Spent: {timeSpent[i]}");
                }
                Console.WriteLine($"\nTotal Hours Tracked: {timeTracked}\n" +
                    $"Total Hours Not Tracked: {timeNotTracked}\n");
                Utility.KeyToProceed();
                if (timeNotTracked > 0)
                {
                    while (!answerProvided)
                    {
                        Console.WriteLine("Would you like to enter a new activity for this date?");
                        Console.Write("(Y/N): ");
                        inputLine = Console.ReadLine().ToLower();

                        switch (inputLine)
                        {
                            case "y":
                            case "yes":
                                while (EnterActivity(dateID))
                                {
                                    //Runs until user decides to not enter another activity
                                }
                                answerProvided = true;
                                break;
                            case "n":
                            case "no":
                                answerProvided = true;
                                break;
                            default:
                                Console.WriteLine("Input not recognized.");
                                Utility.KeyToProceed();
                                break;
                        }
                    }
                }
            }
        }

        static void ViewData (string whereField, int whereID, string selectedValue)
        {
            using (MySqlConnection conn = new MySqlConnection(cs))
            {
                conn.Open();
                string stm = $"SELECT activity_categories.category_description AS Category, activity_descriptions.activity_description AS Description, tracked_calendar_dates.calendar_date AS ActivityDate, activity_times.time_spent_on_activity AS TimeSpent " +
                    $"FROM activity_log " +
                    $"LEFT JOIN activity_categories ON activity_log.category_description = activity_categories.activity_category_id " +
                    $"LEFT JOIN activity_descriptions ON activity_log.activity_description = activity_descriptions.activity_descriptions_id " +
                    $"LEFT JOIN tracked_calendar_dates ON activity_log.calendar_date = tracked_calendar_dates.calendar_date_id " +
                    $"LEFT JOIN activity_times ON activity_log.time_spent_on_activity = activity_times.activity_time_id " +
                    $"WHERE {whereField} = {whereID}";
                string inTransfer = "";
                double transDouble;
                List<string> categories = new List<string>();
                List<string> descriptions = new List<string>();
                List<double> timeSpent = new List<double>();
                List<string> dates = new List<string>();
                MySqlCommand cmd = new MySqlCommand(stm, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    categories.Add(rdr["Category"] as string);
                    descriptions.Add(rdr["Description"] as string);
                    var inTrans = rdr["TimeSpent"];
                    if (inTrans == null)
                    {
                        inTransfer = "unable to read data";
                    }
                    else
                    {
                        inTransfer = inTrans.ToString();
                    }
                    while (!double.TryParse(inTransfer, out transDouble))
                    {
                        Console.Write($"Value not recognized as a number.\nWhat is the number value of {inTransfer}: ");
                        inTransfer = Console.ReadLine();
                    }
                    timeSpent.Add(transDouble);
                    inTrans = rdr["ActivityDate"];
                    if (inTrans.GetType() == typeof(DateTime))
                    {
                        DateTime temp = (DateTime)inTrans;

                        inTransfer = temp.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        inTransfer = inTrans.ToString();
                    }
                    dates.Add(inTransfer);
                }

                Console.Clear();
                for (int i = 0; i < categories.Count; i++)
                {
                    Console.WriteLine($"Category: {categories[i]}\t\tDate: {dates[i]}\n" +
                        $"Description: {descriptions[i]}\t\tTime Spent: {timeSpent[i]}\n");
                }
                transDouble = timeSpent.Sum();
                Console.WriteLine($"Total Time spent on {selectedValue}: {transDouble}");
                Utility.KeyToProceed();
            }
        }

        static Dictionary<int,string> PullData(string dataID, string dataField, string table)
        {
            Dictionary<int, string> thisData = new Dictionary<int, string>();
            using (MySqlConnection conn = new MySqlConnection(cs))
            {
                conn.Open();
                string stm = $"SELECT {dataID}, {dataField} FROM {table}";
                string inTransfer;
                int idValue;
                MySqlCommand cmd = new MySqlCommand(stm, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var inTrans = rdr[$"{dataID}"];
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
                    } while (thisData.ContainsKey(idValue));
                    thisData.Add(idValue, rdr[$"{dataField}"] as string);
                }
            }

            return thisData;
        }

        static double TotalHoursInDatabase()
        {
            double totalHours = 0;
            using (MySqlConnection conn = new MySqlConnection(cs))
            {
                conn.Open();
                string stm = "SELECT activity_times.time_spent_on_activity AS TimeSpent " +
                    "FROM activity_log " +
                    "LEFT JOIN activity_times ON activity_log.time_spent_on_activity = activity_times.activity_time_id";
                string inTransfer;
                List<double> times = new List<double>();
                double transDouble;
                MySqlCommand cmd = new MySqlCommand(stm, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var inTrans = rdr["TimeSpent"];
                    if (inTrans == null)
                    {
                        inTransfer = "unable to read data";
                    }
                    else
                    {
                        inTransfer = inTrans.ToString();
                    }
                    while (!double.TryParse(inTransfer, out transDouble))
                    {
                        Console.Write($"Value not recognized as a number.\nWhat is the number value of {inTransfer}: ");
                        inTransfer = Console.ReadLine();
                    }
                    times.Add(transDouble);
                }
                totalHours = times.Sum();
            }
            return totalHours;
        }

        static double TimeSpent(string whereClause)
        {
            double totalHours = 0;
            using (MySqlConnection conn = new MySqlConnection(cs))
            {
                conn.Open();
                string stm = $"SELECT activity_times.time_spent_on_activity AS TimeSpent " +
                    $"FROM activity_log " +
                    $"LEFT JOIN activity_times ON activity_log.time_spent_on_activity = activity_times.activity_time_id " +
                    $"{whereClause}";
                string inTransfer;
                List<double> times = new List<double>();
                double transDouble;
                MySqlCommand cmd = new MySqlCommand(stm, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var inTrans = rdr["TimeSpent"];
                    if (inTrans == null)
                    {
                        inTransfer = "unable to read data";
                    }
                    else
                    {
                        inTransfer = inTrans.ToString();
                    }
                    while (!double.TryParse(inTransfer, out transDouble))
                    {
                        Console.Write($"Value not recognized as a number.\nWhat is the number value of {inTransfer}: ");
                        inTransfer = Console.ReadLine();
                    }
                    times.Add(transDouble);
                }
                totalHours = times.Sum();
            }
            return totalHours;
        }

        static Dictionary<int,string> PullCalendarData()
        {
            Dictionary<int, string> thisData = new Dictionary<int, string>();
            using (MySqlConnection conn = new MySqlConnection(cs))
            {
                conn.Open();
                string stm = "SELECT calendar_date_id, calendar_date FROM tracked_calendar_dates";
                MySqlCommand cmd = new MySqlCommand(stm, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                string inTransfer;
                int idValue;
                while (rdr.Read())
                {
                    var inTrans = rdr["calendar_date_id"];
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
                    } while (thisData.ContainsKey(idValue));
                    inTrans = rdr["calendar_date"];
                    if (inTrans.GetType() == typeof(DateTime))
                    {
                        DateTime temp = (DateTime)inTrans;

                        inTransfer = temp.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        inTransfer = inTrans.ToString();
                    }
                    thisData.Add(idValue, inTransfer);
                }
            }
            return thisData;
        }
    }
}
