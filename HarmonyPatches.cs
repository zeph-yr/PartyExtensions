﻿using HarmonyLib;
using System;
using System.Collections.Generic;


namespace PartyExtensions
{
    [HarmonyPatch(typeof(LocalLeaderboardsModel), "AddScore", new Type[] { typeof(string), typeof(LocalLeaderboardsModel.LeaderboardType), typeof(string), typeof(int), typeof(bool) })]
    public class AddScorePatch
    {
        static void Prefix(string leaderboardId, LocalLeaderboardsModel.LeaderboardType leaderboardType, string playerName, int score, bool fullCombo, ref LocalLeaderboardsModel __instance, out int __state)
        {
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

            __state = i;
            //Plugin.Log.Debug("Prefix __state: " + __state);
        }


        static void Postfix(string leaderboardId, LocalLeaderboardsModel.LeaderboardType leaderboardType, string playerName, int score, bool fullCombo, ref LocalLeaderboardsModel __instance, int __state)
        {
            //Plugin.Log.Debug("PostFix __state: " + __state);
            //Plugin.Log.Debug("Score: " + score);

            if (__state < 10) //__instance._maxNumberOfScoresInLeaderboard)
            {
                // AddScore is called twice by base game, once for alltime and once for daily leaderboard
                // If we dont do this (or make a is_written flag), there will be duplicates in the files

                PartyData.current_score.playername = playerName;
                PartyData.current_score.fc = fullCombo;
                PartyData.current_score.timestamp = __instance.GetCurrentTimestamp(); // Yes this might be microseconds later but good enough

                if (leaderboardType == LocalLeaderboardsModel.LeaderboardType.AllTime)
                {
                    if (PartyData.all_scores.ContainsKey(leaderboardId)) // PartyExtensions already has scores for this map
                    {
                        Plugin.Log.Debug("Alltime List insert: " + leaderboardId);

                        PartyData.all_scores[leaderboardId].map_scores.Insert(__state, PartyData.current_score);
                        PartyData.all_scores[leaderboardId].map_scores.RemoveAt(10);
                    }
                    else // PartyExtensions doesn't have scores for this map yet
                    {
                        Plugin.Log.Debug("Alltime Dict add: " + leaderboardId);

                        CustomLeaderboard temp = new CustomLeaderboard();
                        temp.leaderboard_id = leaderboardId;
                        temp.map_scores.Insert(__state, PartyData.current_score);
                        temp.map_scores.RemoveAt(10);

                        PartyData.all_scores.Add(leaderboardId, temp);
                    }

                    PartyData.Write_All();
                }

                else // Daily Leaderboard
                {
                    if (PartyData.daily_scores.ContainsKey(leaderboardId))
                    {
                        Plugin.Log.Debug("Daily List insert: " + leaderboardId);

                        PartyData.daily_scores[leaderboardId].map_scores.Insert(__state, PartyData.current_score);
                        PartyData.daily_scores[leaderboardId].map_scores.RemoveAt(10);
                    }
                    else
                    {
                        Plugin.Log.Debug("Daily Dict add: " + leaderboardId);

                        CustomLeaderboard temp = new CustomLeaderboard();
                        temp.leaderboard_id = leaderboardId;
                        temp.map_scores.Insert(__state, PartyData.current_score);
                        temp.map_scores.RemoveAt(10);

                        PartyData.daily_scores.Add(leaderboardId, temp);
                    }

                    PartyData.Write_Daily();
                }
            }
        }
    }


    [HarmonyPatch(typeof(LocalLeaderboardsModel), "UpdateDailyLeaderboard")]
    public class UpdateDailyPatch
    {
        static void Postfix(string leaderboardId, LocalLeaderboardsModel __instance)
        {
            //Plugin.Log.Debug("Update daily learderboard");

            long num = __instance.GetCurrentTimestamp() - 86400L;

            if (PartyData.daily_scores.ContainsKey(leaderboardId))
            {
                for (int i = 0; i < PartyData.daily_scores[leaderboardId].map_scores.Count; i++)
                {
                    if (PartyData.daily_scores[leaderboardId].map_scores[i].timestamp < num && PartyData.daily_scores[leaderboardId].map_scores[i].timestamp >= 1)
                    {
                        PartyData.daily_scores[leaderboardId].map_scores.RemoveAt(i);
                        PartyData.daily_scores[leaderboardId].map_scores.Add(new CustomScoreData());
                    }
                }
            }

            PartyData.Write_Daily();
        }
    }

    
    // Wrote this to delete scores for a specific map but
    // seems like this method is never called in the game so this patch is never called
    // May be handy in future

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


    // Pressing trash button in PartyMode nukes all scores across all maps
    // Todo: Make a backup system
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

            ButtonController.current_leaderboard = null; // Need this or it will fill all the modals with the last score lol
        }
    }


    // Use this to get and store the leaderboardID that is relevant to the user
    [HarmonyPatch(typeof(LocalLeaderboardViewController), "SetContent")]
    public class SetContentPatch
    {
        static void Postfix(string leaderboardID, LocalLeaderboardsModel.LeaderboardType leaderboardType)
        {
            ButtonController.current_leaderboard = leaderboardID;
            ButtonController.leaderboardType = leaderboardType;

            // For debug only
            /*if (leaderboardType == LocalLeaderboardsModel.LeaderboardType.Daily)
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
            }*/
        }
    }


    [HarmonyPatch(typeof(BeatmapObjectExecutionRatingsRecorder), "HandleScoringForNoteDidFinish")]
    internal class HandleScoringForNoteDidFinishPatch
    {
        static void Postfix(ScoringElement scoringElement)
        {
            if (scoringElement != null)
            {
                NoteData noteData = scoringElement.noteData;
                if (noteData.colorType == ColorType.None)
                {
                    PartyExtensionsController.bomb_hits++;
                }

                GoodCutScoringElement goodCutScoringElement;
                if ((goodCutScoringElement = (scoringElement as GoodCutScoringElement)) != null)
                {
                    if (goodCutScoringElement.noteData.colorType == ColorType.ColorA)
                    {
                        if (goodCutScoringElement.maxPossibleCutScore < 115)
                        {
                            PartyExtensionsController.left_slider_acc += goodCutScoringElement.cutScore;
                            PartyExtensionsController.left_slider_hits++;
                        }
                        else
                        {
                            PartyExtensionsController.left_acc += goodCutScoringElement.cutScore;
                            PartyExtensionsController.left_hits++;
                        }
                        //Plugin.Log.Debug("left: " + goodCutScoringElement.cutScore + " " + PartyExtensionsController.left_hits);
                    }
                    if (goodCutScoringElement.noteData.colorType == ColorType.ColorB)
                    {
                        if (goodCutScoringElement.maxPossibleCutScore < 115)
                        {
                            PartyExtensionsController.right_slider_acc += goodCutScoringElement.cutScore;
                            PartyExtensionsController.right_slider_hits++;
                        }
                        else
                        {
                            PartyExtensionsController.right_acc += goodCutScoringElement.cutScore;
                            PartyExtensionsController.right_hits++;
                        }
                        //Plugin.Log.Debug("right: " + goodCutScoringElement.cutScore + " " + PartyExtensionsController.right_hits);
                    }
                }
            }
        }
    }
}