using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace PartyExtensions
{
    internal class PartyData
    {
        //internal static bool is_written = false;

        //internal static CustomScoreData test_score;
        //internal static CustomLeaderboard test_leaderboard;
        //internal static Dictionary<string, CustomLeaderboard> test_dict;


        internal static CustomScoreData current_score;
        internal static Dictionary<string, CustomLeaderboard> all_scores;
        internal static Dictionary<string, CustomLeaderboard> daily_scores;

        //internal static string file_path = @"C:\Program Files (x86)\Steam\steamapps\common\Beat Saber\UserData\PartyExtensions\LocalScores.json";
        //internal static string daily_file_path = @"C:\Program Files (x86)\Steam\steamapps\common\Beat Saber\UserData\PartyExtensions\DailyLocalScores.json";

        internal static string alltime_path;
        internal static string daily_path;


        internal static void Read()
        {
            //Plugin.Log.Debug("Read");

            string appdata_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string base_alltime_path = appdata_path + @"Low\Hyperbolic Magnetism\Beat Saber\LocalLeaderboards.dat";
            string base_daily_path = appdata_path + @"Low\Hyperbolic Magnetism\Beat Saber\LocalDailyLeaderboards.dat";

            string userdata_path = IPA.Utilities.UnityGame.UserDataPath;
            alltime_path = userdata_path + @"\PartyExtensions\LocalScores.json";
            daily_path = userdata_path + @"\PartyExtensions\DailyLocalScores.json";


            //Plugin.Log.Debug("AppData: " + base_alltime_path);
            //Plugin.Log.Debug("AppData: " + base_daily_path);

            //Plugin.Log.Debug("UserData: " + alltime_path);
            //Plugin.Log.Debug("Userata: " + daily_path);


            if (!File.Exists(alltime_path))
            {
                Plugin.Log.Debug("Create alltime file");

                //test_score = new CustomScoreData();
                //test_leaderboard = new CustomLeaderboard();
                //test_dict = new Dictionary<string, CustomLeaderboard>();
                //test_dict.Add("none", new CustomLeaderboard());

                all_scores = new Dictionary<string, CustomLeaderboard>();
                Write_All();
            }
            else
            {
                Plugin.Log.Debug("Alltime file exists");

                string json_string = File.ReadAllText(alltime_path);
                //Plugin.Log.Debug(json_string);

                //test_score = JsonConvert.DeserializeObject<CustomScoreData>(json_string);
                //test_leaderboard = JsonConvert.DeserializeObject<CustomLeaderboard>(json_string);
                //test_dict = JsonConvert.DeserializeObject<Dictionary<string, CustomLeaderboard>>(json_string);

                all_scores = JsonConvert.DeserializeObject<Dictionary<string, CustomLeaderboard>>(json_string);
                //all_scores = JsonConvert.DeserializeObject<Dictionary<string, CustomLeaderboard>>(File.ReadAllText(file_path));


                // Extra bonus: Make a backup at the start of each game but ONLY if the file is not an empty dict
                // Without the condition, the backup would be overwritten when the user launches the game
                // This means if the user accidentally pressed leaderboard delete and restarts the game, they will still have once chance here to grab the backup
                // However this will not backup the base game scores *until* the second game launch after the user has set their first PartyExtension score.
                if (json_string != "{}")
                {
                    File.Copy(alltime_path, alltime_path + ".bak", true);
                    File.Copy(base_alltime_path, userdata_path + @"\PartyExtensions\LocalLeaderboards.json.bak", true);
                }
            }

            if (!File.Exists(daily_path))
            {
                Plugin.Log.Debug("Create daily file");

                daily_scores = new Dictionary<string, CustomLeaderboard>();
                Write_Daily();
            }
            else
            {
                Plugin.Log.Debug("Daily file exists");

                string json_string = File.ReadAllText(daily_path);
                daily_scores = JsonConvert.DeserializeObject<Dictionary<string, CustomLeaderboard>>(json_string);
                //daily_scores = JsonConvert.DeserializeObject<Dictionary<string, CustomLeaderboard>>(File.ReadAllText(daily_file_path));

                if (json_string != "{}")
                {
                    File.Copy(daily_path, daily_path + ".bak", true);
                    File.Copy(base_daily_path, userdata_path + @"\PartyExtensions\LocalDailyLeaderboards.json.bak", true);
                }
            }
        }

        internal static void Write_All()
        {
            //Plugin.Log.Debug("Write All");

            //string json_string = JsonConvert.SerializeObject(test_score);
            //string json_string = JsonConvert.SerializeObject(test_leaderboard);
            //string json_string = JsonConvert.SerializeObject(test_dict);

            //string json_string = JsonConvert.SerializeObject(all_scores);
            //Plugin.Log.Debug(json_string);

            File.WriteAllText(alltime_path, JsonConvert.SerializeObject(all_scores));
        }

        internal static void Write_Daily()
        {
            //Plugin.Log.Debug("Write Daily");

            File.WriteAllText(daily_path, JsonConvert.SerializeObject(daily_scores));
        }
    }
}
