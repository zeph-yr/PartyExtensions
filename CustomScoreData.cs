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

        [JsonConstructor]
        public CustomScoreData()
        {

        }

        public CustomScoreData(string rank, int missed, int good_cuts, int bad_cuts, float left_acc, float right_acc, GameplayModifiers modifiers, int longest_combo, long timestamp, string playername)
        {
            this.rank = rank;
            this.missed = missed;
            this.good_cuts = good_cuts;
            this.bad_cuts = bad_cuts;
            this.left_acc = left_acc;
            this.right_acc = right_acc;
            this.modifiers = modifiers;
            this.longest_combo = longest_combo;
            this.timestamp = timestamp;
            this.playername = playername;
        }
    }

    class CustomPartyLeaderboard
    {
        public List<CustomScoreData> custom_score_list;

        [JsonConstructor]
        public CustomPartyLeaderboard()
        {
            this.custom_score_list = new List<CustomScoreData>();
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