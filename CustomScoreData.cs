using Newtonsoft.Json;


namespace PartyExtensions
{
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

        // BS 1.20.0
        public float left_mixed_acc;
        public float right_mixed_acc;

        //public GameplayModifiers modifiers;
        public bool[] custom_gameplaymodifiers;

        public int longest_combo;

        public long timestamp;
        public string playername;
        public string modifier_hints;


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
            
            // BS 1.20.0
            left_mixed_acc = 0f;
            right_mixed_acc = 0f;

            longest_combo = 0;
            good_cuts = 0;
            bad_cuts = 0;
            missed = 0;
            bombs = 0;

            //modifiers = null;
            custom_gameplaymodifiers = new bool[16];
            modifier_hints = "";

            timestamp = 0;
        }


        // Called on levelcleared to lock gpms into a bool array before they can do their weird thing
        public CustomScoreData(string rank, int missed, int good_cuts, int bad_cuts, int bombs, int raw_score, int mod_score, bool fc, float acc, float mod_acc, float left_acc, float right_acc, float left_mixed_acc, float right_mixed_acc, GameplayModifiers modifiers, int longest_combo, long timestamp, string playername)
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

            //BS 1.20.0
            this.left_mixed_acc = left_mixed_acc;
            this.right_mixed_acc = right_mixed_acc;

            this.longest_combo = longest_combo;
            this.good_cuts = good_cuts;
            this.bad_cuts = bad_cuts;
            this.missed = missed;
            this.bombs = bombs;

            //this.modifiers = modifiers;
            this.custom_gameplaymodifiers = Make_Custom_Gameplaymodifiers(modifiers);
            this.modifier_hints = Convert_GPM(modifiers);
            //Plugin.Log.Debug("mod hints: " + this.modifier_hints);

            this.timestamp = timestamp;
        }


        [JsonConstructor]
        public CustomScoreData(string rank, int missed, int good_cuts, int bad_cuts, int bombs, int raw_score, int mod_score, bool fc, float acc, float mod_acc, float left_acc, float right_acc, float left_mixed_acc, float right_mixed_acc, bool[] modifiers, int longest_combo, long timestamp, string playername, string modifier_hints)
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

            //BS 1.20.0
            this.left_mixed_acc = left_mixed_acc;
            this.right_mixed_acc = right_mixed_acc;

            this.longest_combo = longest_combo;
            this.good_cuts = good_cuts;
            this.bad_cuts = bad_cuts;
            this.missed = missed;
            this.bombs = bombs;

            //this.modifiers = modifiers;
            this.custom_gameplaymodifiers = modifiers;

            // Bandaid for old data before the hints were implemented
            // Without check, previous map's modifiers will be displayed on the next map clicked if it was played before hints implemented
            // End user will users should never see this
            if (modifier_hints == null || modifier_hints == "" || modifier_hints == " ")
            {
                this.modifier_hints = "No modifiers used for this play"; // hoverhint strings cannot be empty, will display the last non-empty one
            }
            else
            {
                this.modifier_hints = modifier_hints;
            }
            //Plugin.Log.Debug("json mod hints: " + modifier_hints);

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


        // Repurposed from ButtonViewController
        internal static string Convert_GPM(GameplayModifiers gameplayModifiers)
        {
            string result = "";

            // Fail and Life Modifiers
            if (gameplayModifiers.noFailOn0Energy)
            {
                result += "NoFail, ";
            }

            if (gameplayModifiers.instaFail)
            {
                result += "1-Life, ";
            }

            switch ((int)gameplayModifiers.energyType)
            {
                case 0:
                    break;
                case 1:
                    result += "4-Lives, ";
                    break;
                default:
                    break;
            }


            // Bombs and Walls
            if (gameplayModifiers.noBombs)
            {
                result += "NoBombs, ";
            }

            switch ((int)gameplayModifiers.enabledObstacleType)
            {
                case 0:
                    break;
                case 1:
                    result += "FullHeightOnly, "; // Probably wont show up
                    break;
                case 2:
                    result += "NoObstacles, ";
                    break;
                default:
                    break;
            }

            // Arrow Modifiers
            if (gameplayModifiers.noArrows)
            {
                result += "NoArrows, ";
            }

            if (gameplayModifiers.ghostNotes)
            {
                result += "GhostNotes, ";
            }

            if (gameplayModifiers.disappearingArrows)
            {
                result += "DisappearingArrows, ";
            }


            // Acc and Block Modifiers
            if (gameplayModifiers.smallCubes)
            {
                result += "SmallCubes, ";
            }

            if (gameplayModifiers.proMode)
            {
                result += "ProMode, ";
            }

            if (gameplayModifiers.strictAngles)
            {
                result += "StrictAngles, ";
            }

            if (gameplayModifiers.zenMode)
            {
                result += "ZenMode, ";
            }


            // Speed Modifiers
            switch ((int)gameplayModifiers.songSpeed)
            {
                case 0:
                    break;
                case 1:
                    result += "FasterSong, ";
                    break;
                case 2:
                    result += "SlowerSong, ";
                    break;
                case 3:
                    result += "SuperFastSong, ";
                    break;
                default:
                    break;
            }

            //Plugin.Log.Debug(result);
            result = result.Trim(',', ' ');

            if (result != " ")
            {
                return result;
            }

            return "No modifiers used for this play";  // Actually saving the empty hover hint
           // return result.Trim(',', ' ');
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