using HarmonyLib;
using System;
using System.Collections.Generic;


namespace PartyExtensions
{
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
                    //Plugin.Log.Debug("LeaderboardData: " + scores[i]._score);

                    if (scores[i]._score < score)
                    {
                        break;
                    }
                }
            }

            // Original
            /*else
            {
                //leaderboardData = new LocalLeaderboardsModel.LeaderboardData();
                //leaderboardData._leaderboardId = leaderboardId;
                //leaderboardData._scores = new List<LocalLeaderboardsModel.ScoreData>(10); //__instance._maxNumberOfScoresInLeaderboard);
                //__instance.GetLeaderboardsData(leaderboardType).Add(leaderboardData);
            }*/

            __state = i;
            Plugin.Log.Debug("Prefix __state: " + __state);
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


                // How to best player in the world
                /*
                // Overflow fun KEKW
                //int a = 1000000000;
                //a += 2000000000;

                ExtendedScoreData scoreData = new ExtendedScoreData(a, "Zephyr", true, __instance.GetCurrentTimestamp(), 500);

                LocalLeaderboardsModel.LeaderboardData leaderboardData = __instance.GetLeaderboardData(leaderboardId, leaderboardType);
                List<LocalLeaderboardsModel.ScoreData> scores2 = leaderboardData._scores;

                scores2.RemoveAt(__state);
                scores2.Insert(__state, scoreData);
                */

                if (leaderboardType == LocalLeaderboardsModel.LeaderboardType.AllTime)
                {
                    PartyData.current_score.playername = playerName;
                    PartyData.current_score.timestamp = __instance.GetCurrentTimestamp(); // Yes this might be microseconds later but good enough

                    if (PartyData.all_scores.ContainsKey(leaderboardId))
                    {
                        Plugin.Log.Debug("List insert: " + leaderboardId);

                        PartyData.all_scores[leaderboardId].map_scores.Insert(__state, PartyData.current_score);
                        PartyData.all_scores[leaderboardId].map_scores.RemoveAt(9);
                    }

                    else
                    {
                        Plugin.Log.Debug("Dict add: " + leaderboardId);

                        CustomLeaderboard temp = new CustomLeaderboard();
                        temp.leaderboard_id = leaderboardId;
                        temp.map_scores.Insert(__state, PartyData.current_score);
                        temp.map_scores.RemoveAt(9);

                        PartyData.all_scores.Add(leaderboardId, temp);
                    }

                    PartyData.Write_All();
                }

                else
                {
                    PartyData.current_score.playername = playerName;
                    PartyData.current_score.timestamp = __instance.GetCurrentTimestamp(); // Yes this might be microseconds later but good enough

                    if (PartyData.daily_scores.ContainsKey(leaderboardId))
                    {
                        Plugin.Log.Debug("List insert: " + leaderboardId);

                        PartyData.daily_scores[leaderboardId].map_scores.Insert(__state, PartyData.current_score);
                        PartyData.daily_scores[leaderboardId].map_scores.RemoveAt(9);
                    }

                    else
                    {
                        Plugin.Log.Debug("Dict add: " + leaderboardId);

                        CustomLeaderboard temp = new CustomLeaderboard();
                        temp.leaderboard_id = leaderboardId;
                        temp.map_scores.Insert(__state, PartyData.current_score);
                        temp.map_scores.RemoveAt(9);

                        PartyData.daily_scores.Add(leaderboardId, temp);
                    }

                    PartyData.Write_Daily();
                }

                
                /*if (! PartyData.is_written)
                {
                    PartyData.current_score.playername = playerName;
                    PartyData.current_score.timestamp = __instance.GetCurrentTimestamp(); // Yes this might be microseconds later but good enough

                    if (PartyData.all_scores.ContainsKey(leaderboardId))
                    {
                        Plugin.Log.Debug("List insert: " + leaderboardId);

                        PartyData.all_scores[leaderboardId].map_scores.Insert(__state, PartyData.current_score);
                        PartyData.all_scores[leaderboardId].map_scores.RemoveAt(9);
                    }

                    else
                    {
                        Plugin.Log.Debug("Dict add: " + leaderboardId);

                        CustomLeaderboard temp = new CustomLeaderboard();
                        temp.leaderboard_id = leaderboardId;
                        temp.map_scores.Insert(__state, PartyData.current_score);
                        temp.map_scores.RemoveAt(9);

                        PartyData.all_scores.Add(leaderboardId, temp);
                        PartyData.daily_scores.Add(leaderboardId, temp);
                    }

                    PartyData.Write();
                    PartyData.is_written = true;
                }*/

            }

            /*this._lastScorePositions[leaderboardType] = i;
            this._lastScoreLeaderboardId = leaderboardId;
            if (this.newScoreWasAddedToLeaderboardEvent != null)
            {
                this.newScoreWasAddedToLeaderboardEvent(leaderboardId, leaderboardType);
            }*/
        }


        // How to best player in the world continued
        /*class ExtendedScoreData : LocalLeaderboardsModel.ScoreData
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
        }*/
    }


    [HarmonyPatch(typeof(LocalLeaderboardsModel), "UpdateDailyLeaderboard")]
    public class UpdateDailyPatch
    {
        static void Postfix(string leaderboardId, LocalLeaderboardsModel __instance)
        {
            Plugin.Log.Debug("Update daily learderboard");

            long num = __instance.GetCurrentTimestamp() - 86400L;

            if (PartyData.daily_scores.ContainsKey(leaderboardId))
            {
                for (int i = PartyData.daily_scores[leaderboardId].map_scores.Count - 1; i >= 0; i--)
                {
                    if (PartyData.daily_scores[leaderboardId].map_scores[i].timestamp < num && PartyData.daily_scores[leaderboardId].map_scores[i].timestamp >= 1)
                    {
                        PartyData.daily_scores[leaderboardId].map_scores.RemoveAt(i);
                    }
                }
            }

            PartyData.Write_Daily();
        }
    }

    /*[HarmonyPatch(typeof(LocalLeaderboardsModel), "ClearLeaderboard")]
    public class ClearLeaderboardPatch
    {
        static void Postfix(string leaderboardId)
        {
            Plugin.Log.Debug("Clear leaderboard: " + leaderboardId);

            if (PartyData.all_scores.ContainsKey(leaderboardId))
            {
                Plugin.Log.Debug("Clear all scores");

                PartyData.all_scores.Remove(leaderboardId);
                PartyData.Write_All();
            }

            if (PartyData.daily_scores.ContainsKey(leaderboardId))
            {
                Plugin.Log.Debug("Clear daily scores");

                PartyData.daily_scores.Remove(leaderboardId);
                PartyData.Write_Daily();
            }
        }
    }*/


    [HarmonyPatch(typeof(LocalLeaderboardsModel), "ClearAllLeaderboards")]
    public class ClearAllPatch
    {
        static void Postfix()
        {
            Plugin.Log.Debug("Nuke all party data");

            PartyData.all_scores = new Dictionary<string, CustomLeaderboard>();
            PartyData.Write_All();

            PartyData.daily_scores = new Dictionary<string, CustomLeaderboard>();
            PartyData.Write_Daily();
        }
    }


    [HarmonyPatch(typeof(LocalLeaderboardViewController), "SetContent")]
    public class SetContentPatch
    {
        static void Postfix(string leaderboardID, LocalLeaderboardsModel.LeaderboardType leaderboardType)
        {
            ButtonController.current_leaderboard = leaderboardID;
            ButtonController.leaderboardType = leaderboardType;

            if (leaderboardType == LocalLeaderboardsModel.LeaderboardType.Daily)
            {
                if (PartyData.daily_scores.ContainsKey(leaderboardID))
                {
                    Plugin.Log.Debug("Found daily leaderboard: " + PartyData.daily_scores[leaderboardID].map_scores[0].playername);
                }
            }
            else
            {
                if (PartyData.all_scores.ContainsKey(leaderboardID))
                {
                    Plugin.Log.Debug("Found all time leaderboard: " + PartyData.all_scores[leaderboardID].map_scores[0].playername);
                }
            }
        }
    }
}


// Testing single map's leaderboard
/*if (! PartyData.is_written)
{
    PartyData.test_leaderboard.leaderboard_id = leaderboardData._leaderboardId;
    PartyData.test_leaderboard.map_scores.Insert(__state, PartyData.test_score);
    PartyData.test_leaderboard.map_scores.RemoveAt(9);

    PartyData.Write();

    PartyData.is_written = true;
}*/

// Testing adding to dictionary
/*if (! PartyData.is_written)
{
    CustomLeaderboard temp = new CustomLeaderboard();

    temp.leaderboard_id = leaderboardId;
    temp.map_scores.Insert(__state, PartyData.test_score);
    temp.map_scores.RemoveAt(9);

    PartyData.test_dict.Add(leaderboardId, temp);

    PartyData.Write();

    PartyData.is_written = true;
}*/