using Newtonsoft.Json;
using System.Collections.Generic;

namespace PartyExtensions
{
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
