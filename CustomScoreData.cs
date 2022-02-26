using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;


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

        internal static string file_path = @"C:\Program Files (x86)\Steam\steamapps\common\Beat Saber\UserData\PartyExtensions\LocalScores.json";
        internal static string daily_file_path = @"C:\Program Files (x86)\Steam\steamapps\common\Beat Saber\UserData\PartyExtensions\DailyLocalScores.json";


        internal static void Read()
        {
            //Plugin.Log.Debug("Read");

            if (!File.Exists(file_path))
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

                string json_string = File.ReadAllText(file_path);
                //Plugin.Log.Debug(json_string);

                //test_score = JsonConvert.DeserializeObject<CustomScoreData>(json_string);
                //test_leaderboard = JsonConvert.DeserializeObject<CustomLeaderboard>(json_string);
                //test_dict = JsonConvert.DeserializeObject<Dictionary<string, CustomLeaderboard>>(json_string);

                all_scores = JsonConvert.DeserializeObject<Dictionary<string, CustomLeaderboard>>(json_string);
                //all_scores = JsonConvert.DeserializeObject<Dictionary<string, CustomLeaderboard>>(File.ReadAllText(file_path));


                // Extra bonus: Make a backup at the start of each game but ONLY if the file is not an empty dict
                // Without the condition, the backup would be overwritten when the user launches the game
                // This means if the user accidentally pressed leaderboard delete and restarts the game, they will still have once chance here to grab the backup
                if (json_string != "{}")
                {
                    File.Copy(file_path, file_path + ".bak", true);
                }
            }

            if (!File.Exists(daily_file_path))
            {
                Plugin.Log.Debug("Create daily file");

                daily_scores = new Dictionary<string, CustomLeaderboard>();
                Write_Daily();
            }
            else
            {
                Plugin.Log.Debug("Daily file exists");

                string json_string = File.ReadAllText(daily_file_path);
                daily_scores = JsonConvert.DeserializeObject<Dictionary<string, CustomLeaderboard>>(json_string);
                //daily_scores = JsonConvert.DeserializeObject<Dictionary<string, CustomLeaderboard>>(File.ReadAllText(daily_file_path));

                if (json_string != "{}")
                {
                    File.Copy(daily_file_path, daily_file_path + ".bak", true);
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

            File.WriteAllText(file_path, JsonConvert.SerializeObject(all_scores));
        }

        internal static void Write_Daily()
        {
            //Plugin.Log.Debug("Write Daily");

            File.WriteAllText(daily_file_path, JsonConvert.SerializeObject(daily_scores));
        }
    }


    // Note: Translating the basegame gpms into a bool array because storing the original gpm object doesnt work well:
    // When level is cleared, the serialized results of the basegame gpm is correct.
    // However, by the time they are written in AddScore, they may be correct or sometimes all reset to false.
    // Not sure why, maybe its a reference or there is some race condtion but wtv, not worth it. This works fine and is easier
    internal enum CustomGamePlayModifiersEnum
    {
        // Fail and Lives
        NF = 0,
        L1 = 1, // 1 Life
        L4 = 2, // 4 Lives

        // Bombs and Walls
        NB = 3,
        FH = 4, //Full Height Only, probably wont show up
        NO = 5,

        // Arrows
        NA = 6,
        GN = 7,
        DA = 8,

        // Acc and Block
        SC = 9,
        PM = 10,
        SA = 11,
        ZM = 12,

        // Song peed
        FS = 13,
        SS = 14,
        SFS = 15
    }


    internal class CustomScoreData
    {
        // These are in the order in the constructors (don't change!)
        // They must be public or json serializer will do nothing
        public string rank;

        public int missed;
        public int good_cuts;
        public int bad_cuts;
        public int bombs;

        public int raw_score;
        public int mod_score;
        public bool fc;
        public float acc;
        public float mod_acc;
        public float left_acc;
        public float right_acc;

        //public GameplayModifiers modifiers;
        public bool[] custom_gameplaymodifiers;

        public int longest_combo;

        public long timestamp;
        public string playername;

        public CustomScoreData()
        {
            // This is the order they are displayed in UI
            playername = "";
            raw_score = 0;
            mod_score = 0;
            rank = "";
            fc = false;

            acc = 0f;
            mod_acc = 0f;

            left_acc = 0f;
            right_acc = 0f;

            longest_combo = 0;
            good_cuts = 0;
            bad_cuts = 0;
            missed = 0;
            bombs = 0;

            timestamp = 0;

            //modifiers = null;
            custom_gameplaymodifiers = new bool[16];
            
        }

        // Called on levelcleared to lock gpms into a bool array before they can do their weird thing
        public CustomScoreData(string rank, int missed, int good_cuts, int bad_cuts, int bombs, int raw_score, int mod_score, bool fc, float acc, float mod_acc, float left_acc, float right_acc, GameplayModifiers modifiers, int longest_combo, long timestamp, string playername)
        {
            this.playername = playername;
            this.raw_score = raw_score;
            this.mod_score = mod_score;
            this.fc = fc;
            this.rank = rank;

            this.acc = acc;
            this.mod_acc = mod_acc;
            this.left_acc = left_acc;
            this.right_acc = right_acc;

            this.longest_combo = longest_combo;
            this.good_cuts = good_cuts;
            this.bad_cuts = bad_cuts;
            this.missed = missed;
            this.bombs = bombs;

            //this.modifiers = modifiers;
            this.custom_gameplaymodifiers = Make_Custom_Gameplaymodifiers(modifiers);
            this.timestamp = timestamp;
        }


        [JsonConstructor]
        public CustomScoreData(string rank, int missed, int good_cuts, int bad_cuts, int bombs, int raw_score, int mod_score, bool fc, float acc, float mod_acc, float left_acc, float right_acc, bool[] modifiers, int longest_combo, long timestamp, string playername)
        {
            this.playername = playername;
            this.raw_score = raw_score;
            this.mod_score = mod_score;
            this.fc = fc;
            this.rank = rank;

            this.acc = acc;
            this.mod_acc = mod_acc;
            this.left_acc = left_acc;
            this.right_acc = right_acc;

            this.longest_combo = longest_combo;
            this.good_cuts = good_cuts;
            this.bad_cuts = bad_cuts;
            this.missed = missed;
            this.bombs = bombs;

            //this.modifiers = modifiers;
            this.custom_gameplaymodifiers = modifiers;
            this.timestamp = timestamp;
        }


        private static bool[] Make_Custom_Gameplaymodifiers(GameplayModifiers gameplayModifiers)
        {
            bool[] custom_gameplaymodifiers = new bool[16];

            // Not all gpm attributes are used as they aren't things the user would be concerned with

            // Fail and Life Modifiers
            if (gameplayModifiers.noFailOn0Energy)
            {
                custom_gameplaymodifiers[0] = true;
            }

            if (gameplayModifiers.instaFail)
            {
                custom_gameplaymodifiers[1] = true;
            }

            switch ((int)gameplayModifiers.energyType) // This is more maintainable
            {
                case 0:
                    break;
                case 1:
                    custom_gameplaymodifiers[2] = true;
                    break;
                default:
                    break;
            }

            // Bombs and Walls
            if (gameplayModifiers.noBombs)
            {
                custom_gameplaymodifiers[3] = true;
            }

            switch ((int)gameplayModifiers.enabledObstacleType)
            {
                case 0:
                    break;
                case 1:
                    custom_gameplaymodifiers[4] = true;
                    break;
                case 2:
                    custom_gameplaymodifiers[5] = true;
                    break;
                default:
                    break;
            }

            // Arrow Modifiers
            if (gameplayModifiers.noArrows)
            {
                custom_gameplaymodifiers[6] = true;
            }

            if (gameplayModifiers.ghostNotes)
            {
                custom_gameplaymodifiers[7] = true;
            }

            if (gameplayModifiers.disappearingArrows)
            {
                custom_gameplaymodifiers[8] = true;
            }

            // Acc and Block Modifiers
            if (gameplayModifiers.smallCubes)
            {
                custom_gameplaymodifiers[9] = true;
            }

            if (gameplayModifiers.proMode)
            {
                custom_gameplaymodifiers[10] = true;
            }

            if (gameplayModifiers.strictAngles)
            {
                custom_gameplaymodifiers[11] = true;
            }

            if (gameplayModifiers.zenMode)
            {
                custom_gameplaymodifiers[12] = true;
            }

            // Speed Modifiers
            switch ((int)gameplayModifiers.songSpeed)
            {
                case 0:
                    break;
                case 1:
                    custom_gameplaymodifiers[13] = true;
                    break;
                case 2:
                    custom_gameplaymodifiers[14] = true;
                    break;
                case 3:
                    custom_gameplaymodifiers[15] = true;
                    break;
                default:
                    break;
            }

            return custom_gameplaymodifiers;
        }

        internal static string Read_Custom_Gameplaymodifiers(bool[] data)
        {
            string result = "";

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == true)
                {
                    result += (CustomGamePlayModifiersEnum)i + ", ";
                }
            }

            if (result == "")
            {
                result = "None";
            }

            Plugin.Log.Debug(result);

            return result.Trim(',', ' ');
        }
    }

    internal class CustomLeaderboard
    {
        // They must be public or json serializer breaks
        public string leaderboard_id;
        public List<CustomScoreData> map_scores;

        public CustomLeaderboard()
        {
            leaderboard_id = "";
            map_scores = Make_Placeholders();
        }

        /*public CustomLeaderboard(string id)
        {
            leaderboard_id = id;
            map_scores = Make_Placeholders();
        }*/

        [JsonConstructor]
        public CustomLeaderboard(string id, List<CustomScoreData> map_scores)
        {
            this.leaderboard_id = id;

            if (map_scores == null)
            {
                this.map_scores = Make_Placeholders();
            }
            else
            {
                this.map_scores = map_scores;
            }
        }

        // We need placeholders because there may be scores on basegame leaderboards the user has played before getting PartyExtensions
        // Can't expect them to nuke their scores and start again with the mod
        // This is an easy way to keep PE additional data "lined up" with basegame data
        // If the user temporarily removes PE and plays some maps, then adds back PE, there is a possibility some leaderboards will be out of sync
        // and opening the modal will present another score (if scores had been inserted between PE scores).
        // This is unavoidable and building stuff to account for this is out-of-scope of the project
        private static List<CustomScoreData> Make_Placeholders()
        {
            List<CustomScoreData> map_scores = new List<CustomScoreData>();

            for (int i = 0; i < 10; i++)
            {
                map_scores.Add(new CustomScoreData());
            }

            return map_scores;
        }
    }
}

/*
// Sample of serialized CustomScoreData:
DEBUG @ 23:59:55 | PartyExtensions] level cleared
[DEBUG @ 23:59:55 | PartyExtensions] final: 107.4318 103.6882
[DEBUG @ 23:59:55 | PartyExtensions] {
[DEBUG @ 23:59:55 | PartyExtensions]   "rank": "C",
[DEBUG @ 23:59:55 | PartyExtensions]   "missed": 38,
[DEBUG @ 23:59:55 | PartyExtensions]   "good_cuts": 545,
[DEBUG @ 23:59:55 | PartyExtensions]   "bad_cuts": 1,
[DEBUG @ 23:59:55 | PartyExtensions]   "left_acc": 107.431755,
[DEBUG @ 23:59:55 | PartyExtensions]   "right_acc": 103.688171,
[DEBUG @ 23:59:55 | PartyExtensions]   "modifiers": {
[DEBUG @ 23:59:55 | PartyExtensions]     "energyType": 0,
[DEBUG @ 23:59:55 | PartyExtensions]     "noFailOn0Energy": true,
[DEBUG @ 23:59:55 | PartyExtensions]     "demoNoFail": false,
[DEBUG @ 23:59:55 | PartyExtensions]     "instaFail": false,
[DEBUG @ 23:59:55 | PartyExtensions]     "failOnSaberClash": false,
[DEBUG @ 23:59:55 | PartyExtensions]     "enabledObstacleType": 0,
[DEBUG @ 23:59:55 | PartyExtensions]     "demoNoObstacles": false,
[DEBUG @ 23:59:55 | PartyExtensions]     "fastNotes": false,
[DEBUG @ 23:59:55 | PartyExtensions]     "strictAngles": false,
[DEBUG @ 23:59:55 | PartyExtensions]     "disappearingArrows": false,
[DEBUG @ 23:59:55 | PartyExtensions]     "ghostNotes": false,
[DEBUG @ 23:59:55 | PartyExtensions]     "noBombs": false,
[DEBUG @ 23:59:55 | PartyExtensions]     "songSpeed": 0,
[DEBUG @ 23:59:55 | PartyExtensions]     "noArrows": false,
[DEBUG @ 23:59:55 | PartyExtensions]     "proMode": false,
[DEBUG @ 23:59:55 | PartyExtensions]     "zenMode": false,
[DEBUG @ 23:59:55 | PartyExtensions]     "smallCubes": false,
[DEBUG @ 23:59:55 | PartyExtensions]     "songSpeedMul": 1.0,
[DEBUG @ 23:59:55 | PartyExtensions]     "cutAngleTolerance": 60.0,
[DEBUG @ 23:59:55 | PartyExtensions]     "notesUniformScale": 1.0
[DEBUG @ 23:59:55 | PartyExtensions]   },
[DEBUG @ 23:59:55 | PartyExtensions]   "longest_combo": 195,
[DEBUG @ 23:59:55 | PartyExtensions]   "timestamp": 637797887951893649,
[DEBUG @ 23:59:55 | PartyExtensions]   "playername": ""
[DEBUG @ 23:59:55 | PartyExtensions] }
*/