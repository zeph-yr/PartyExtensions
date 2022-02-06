using HarmonyLib;
using System;
using System.Collections.Generic;


namespace PartyExtensions
{
    //[HarmonyPatch(typeof(LocalLeaderboardsModel), "AddScore")]
    [HarmonyPatch(typeof(LocalLeaderboardsModel), "AddScore", new Type[] { typeof(string), typeof(LocalLeaderboardsModel.LeaderboardType), typeof(string), typeof(int), typeof(bool) })]
    public class AddScorePatch
    {
        static void Prefix(string leaderboardId, LocalLeaderboardsModel.LeaderboardType leaderboardType, string playerName, int score, bool fullCombo, ref LocalLeaderboardsModel __instance, out int __state)
        {
            Plugin.Log.Debug("Prefix Start");
            Plugin.Log.Debug("Data: " + leaderboardId + playerName + score + fullCombo);

            LocalLeaderboardsModel.LeaderboardData leaderboardData = __instance.GetLeaderboardData(leaderboardId, leaderboardType);
            int i = 0;
            if (leaderboardData != null)
            {
                List<LocalLeaderboardsModel.ScoreData> scores = leaderboardData._scores;
                for (i = 0; i < scores.Count; i++)
                {
                    Plugin.Log.Debug("LeaderboardData: " + scores[i]._score);

                    if (scores[i]._score < score)
                    {
                        break;
                    }
                }
            }

            __state = i;

            Plugin.Log.Debug("Prefix __state: " + __state);


            /*else
            {
                leaderboardData = new LocalLeaderboardsModel.LeaderboardData();
                leaderboardData._leaderboardId = leaderboardId;
                leaderboardData._scores = new List<LocalLeaderboardsModel.ScoreData>(10); //__instance._maxNumberOfScoresInLeaderboard);
                __instance.GetLeaderboardsData(leaderboardType).Add(leaderboardData);
            }*/
        }

        static void Postfix(string leaderboardId, LocalLeaderboardsModel.LeaderboardType leaderboardType, string playerName, int score, bool fullCombo, ref LocalLeaderboardsModel __instance, int __state)
        {
            Plugin.Log.Debug("PostFix __state: " + __state);

            if (__state < 10) //__instance._maxNumberOfScoresInLeaderboard)
            {
                // Original
                /*LocalLeaderboardsModel.ScoreData scoreData = new LocalLeaderboardsModel.ScoreData();
                scoreData._score = score;
                scoreData._playerName = playerName;
                scoreData._fullCombo = fullCombo;
                scoreData._timestamp = __instance.GetCurrentTimestamp();
                List<LocalLeaderboardsModel.ScoreData> scores2 = leaderboardData._scores;
 
                scores2.Insert(i, scoreData);
                if (scores2.Count > 10) //this._maxNumberOfScoresInLeaderboard)
                {
                    scores2.RemoveAt(scores2.Count - 1);
                }*/

                //LocalLeaderboardsModel.ScoreData scoreData = Plugin.test_score;

                int a = 1000000000;

                a += 2000000000;

                ExtendedScoreData scoreData = new ExtendedScoreData(a, "Zephyr", true, __instance.GetCurrentTimestamp(), 500);

                //2,147,483,648

                LocalLeaderboardsModel.LeaderboardData leaderboardData = __instance.GetLeaderboardData(leaderboardId, leaderboardType);
                List<LocalLeaderboardsModel.ScoreData> scores2 = leaderboardData._scores;

                scores2.RemoveAt(__state);
                scores2.Insert(__state, scoreData);

                //Plugin.Log.Debug("PostFix __playername: " + Plugin.test_score._playerName);

                Plugin.Log.Debug("PostFix __state: " + __state);
            }

            /*this._lastScorePositions[leaderboardType] = i;
            this._lastScoreLeaderboardId = leaderboardId;
            if (this.newScoreWasAddedToLeaderboardEvent != null)
            {
                this.newScoreWasAddedToLeaderboardEvent(leaderboardId, leaderboardType);
            }*/

        }


        class ExtendedScoreData : LocalLeaderboardsModel.ScoreData
        {
            public int a;

            public ExtendedScoreData()
            {

            }

            public ExtendedScoreData(int score, string playername, bool fc, long time, int additional_info)
            {
                this._score = score;
                this._playerName = playername;
                this._fullCombo = fc;
                this._timestamp = time;
                this.a = additional_info;
            }
        }
    }

}