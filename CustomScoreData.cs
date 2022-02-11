using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace PartyExtensions
{
    class CustomScoreData
    {
        // points - base
        // rank 
        // highest combo
        // accuary left/right
        // blockcount hit
        // missed blocks
        // mods (speed+, ghost-blocks etc)
        // date/time when reached

        public string rank;

        public int missed;
        public int good_cuts;
        public int bad_cuts;

        public float left_acc;
        public float right_acc;

        public GameplayModifiers modifiers;

        public int longest_combo;

        public long timestamp;
        public string playername;

        public CustomScoreData()
        {
            this.playername = "";

            this.rank = "";

            this.left_acc = 0f;
            this.right_acc = 0f;

            this.good_cuts = 0;
            this.bad_cuts = 0;
            this.missed = 0;
            this.longest_combo = 0;

            this.modifiers = null;
            this.timestamp = 0;
        }

        [JsonConstructor]
        public CustomScoreData(string rank, int missed, int good_cuts, int bad_cuts, float left_acc, float right_acc, GameplayModifiers modifiers, int longest_combo, long timestamp, string playername)
        {
            this.playername = playername;

            this.rank = rank;
            
            this.left_acc = left_acc;
            this.right_acc = right_acc;

            this.good_cuts = good_cuts;
            this.bad_cuts = bad_cuts;
            this.missed = missed;
            this.longest_combo = longest_combo;

            this.modifiers = modifiers;
            this.timestamp = timestamp;
        }
    }

    class CustomLeaderboard
    {
        public string leaderboard_id;
        public List<CustomScoreData> map_scores;

        public CustomLeaderboard()
        {
            this.leaderboard_id = "";
            this.map_scores = new List<CustomScoreData>();
        }

        [JsonConstructor]
        public CustomLeaderboard(string id, List<CustomScoreData> map_scores)
        {
            this.leaderboard_id = id;
            this.map_scores = map_scores;
        }
    }

    class CustomLeaderboardCollection
    {
        public Dictionary<string, CustomLeaderboard> map_leaderboards;

        public CustomLeaderboardCollection()
        {
            this.map_leaderboards = new Dictionary<string, CustomLeaderboard>();
        }

        [JsonConstructor]
        public CustomLeaderboardCollection(Dictionary<string, CustomLeaderboard> map_leaderboards)
        {
            this.map_leaderboards = map_leaderboards;
        }
    }
}

/*DEBUG @ 23:59:55 | PartyExtensions] level cleared
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