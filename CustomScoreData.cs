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

        internal string rank;

        internal int missed;
        internal int good_cuts;
        internal int bad_cuts;

        internal float left_acc;
        internal float right_acc;

        internal GameplayModifiers modifiers;

        internal int longest_combo;

        internal long timestamp;
        internal string playername;

        [JsonConstructor]
        public CustomScoreData()
        {

        }

        public CustomScoreData(string rank, int missed, int good_cuts, int bad_cuts, /*float left_acc, float right_acc,*/ GameplayModifiers modifiers, int longest_combo, long timestamp, string playername)
        {
            this.rank = rank;
            this.missed = missed;
            this.good_cuts = good_cuts;
            this.bad_cuts = bad_cuts;
            //this.left_acc = left_acc;
            //this.right_acc = right_acc;
            this.modifiers = modifiers;
            this.longest_combo = longest_combo;
            this.timestamp = timestamp;
            this.playername = playername;
        }




    }
}
