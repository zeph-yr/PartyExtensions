﻿using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using System;

namespace PartyExtensions
{
    [HotReload(@"ButtonViewController.bsml")]
    public partial class ButtonViewController : BSMLAutomaticViewController
    {
        //***********************************
        // Leaderboard Buttons
        //***********************************

        [UIComponent("button_list")]
        private CustomListTableData Button_List;


        [UIAction("button_clicked")]
        private void Button_Clicked(TableView tableView, int row)
        {
            Plugin.Log.Debug("button clicked: " + row);

            // Order really matters as to when the hover hint is drawn
            // Without this hover hints sometimes appear behind modal with contents are out of sync
            parserParams.EmitEvent("close-modal");

            Set_Modal_Content(row);
            //Fill_Modal_Test();

            parserParams.EmitEvent("open-modal"); // This is absolutely critical!! Not sure why but "button_clicked" doesn't work as show-event

            Button_List.tableView.ClearSelection(); // Otherwise user has to click away and back if they want to open the same score again
        }


        private void Fill_Button_List()
        {
            Button_List.data.Clear();

            for (int i = 0; i < 10; i++)
            {
                Button_List.data.Add(new CustomListTableData.CustomCellInfo("<#cc99ff>💟"));
            }

            Button_List.tableView.ReloadData();
            Button_List.tableView.ClearSelection();
        }


        [UIAction("#post-parse")]
        private void PostParse()
        {
            Fill_Button_List();
        }


        //***********************************
        // Modal Panel
        //***********************************

        [UIParams]
        private BSMLParserParams parserParams;

        [UIComponent("modal")]
        private ModalView Modal;


        /*[UIComponent("modal_list")]
        private CustomListTableData Modal_List;

        private void Set_Modal_Content(int row)
        {
            Modal_List.data.Clear();

            CustomLeaderboard temp;

            // Possible scenario where scores exist on one leaderboard but not the other
            // E.g. No score on daily (because new day) but exists in alltime
            if (ButtonController.leaderboardType == LocalLeaderboardsModel.LeaderboardType.Daily)
            {
                if (PartyData.daily_scores.TryGetValue(ButtonController.current_leaderboard, out temp) == false)
                {
                    Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"No data for this score"));

                    Modal_List.tableView.ReloadData();
                    Modal_List.tableView.ClearSelection();

                    Set_Empty_Fields();
                    return;
                }
            }

            else
            {
                if (PartyData.all_scores.TryGetValue(ButtonController.current_leaderboard, out temp) == false)
                {
                    Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"No data for this score"));

                    Modal_List.tableView.ReloadData();
                    Modal_List.tableView.ClearSelection();

                    Set_Empty_Fields();
                    return;
                }
            }

            // For the dummy placeholder scores in data files
            if (temp.map_scores[row].playername == "")
            {
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"No data for this score"));
                Set_Empty_Fields();
            }

            else
            {
                temp_playername = temp.map_scores[row].playername;
                Playername = "changed";

                temp_raw_score =temp.map_scores[row].raw_score.ToString("N0");
                Raw_Score = "changed";

                if (temp.map_scores[row].fc)
                {
                    temp_rank = temp.map_scores[row].rank + " - FC";
                }
                else
                {
                    temp_rank = temp.map_scores[row].rank;
                }
                Rank = "changed";

                temp_acc = String.Format("{0:0.00}", temp.map_scores[row].acc) + "%";
                Acc = "changed";

                // Display only if GPM used
                if (CustomScoreData.Read_Custom_Gameplaymodifiers(temp.map_scores[row].custom_gameplaymodifiers) == "None")
                {
                    temp_mod_score = "<#b3b3cc>Mod: <#ffffff>---";
                    temp_mod_acc = "<#b3b3cc>Mod: <#ffffff>---";
                }
                else
                {
                    temp_mod_score = "<#b3b3cc>Mod: <#ffffff>" + temp.map_scores[row].mod_score.ToString("N0");
                    temp_mod_acc = "<#b3b3cc>Mod: <#ffffff>" + String.Format("{0:0.00}", temp.map_scores[row].mod_acc) + "%";
                }
                Mod_Score = "changed";
                Mod_Acc = "changed";


                //Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"<#ffff00ff>{temp.map_scores[row].playername}"));
                //Modal_List.data.Add(new CustomListTableData.CustomCellInfo($""));
                //Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"Rank: {temp.map_scores[row].rank}"));

                // Avg Acc
                //Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"Accuracy: " + String.Format("{0:0.00}",  temp.map_scores[row].acc) + " - " + String.Format("{0:0.00}", temp.map_scores[row].acc / 115 * 100) + "%"));
                //Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"Accuracy: " + String.Format("{0:0.00}", temp.map_scores[row].acc) + "%"));

                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"<#b3b3cc>Date Set: <#ffffff>{Convert_Timestamp(temp.map_scores[row].timestamp)}"));
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"<#b3b3cc>Accuracy: <#ff0000>" + String.Format("{0:0.00}", temp.map_scores[row].left_acc) + " <#ffffff>- <#1a53ff>" + String.Format("{0:0.00}", temp.map_scores[row].right_acc)));
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"<#b3b3cc>Max Combo: <#ffffff>{temp.map_scores[row].longest_combo}"));
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"<#b3b3cc>Good Cuts: <#ffffff>{temp.map_scores[row].longest_combo}"));
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"<#b3b3cc>Bad Cuts: <#ffffff>{temp.map_scores[row].bad_cuts}"));
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"<#b3b3cc>Missed: <#ffffff>{temp.map_scores[row].missed}"));
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"<#b3b3cc>Bomb Hits: <#ffffff>{temp.map_scores[row].bombs}"));
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"<#b3b3cc>Modifiers: <#ffffff>{CustomScoreData.Read_Custom_Gameplaymodifiers(temp.map_scores[row].custom_gameplaymodifiers)}"));
            }

            Modal_List.tableView.ReloadData();
            Modal_List.tableView.ClearSelection();
        }*/


        private void Set_Modal_Content(int row)
        {
            Set_Empty_Fields();

            CustomLeaderboard temp;

            // Possible scenario where scores exist on one leaderboard but not the other
            // E.g. No score on daily (because new day) but exists in alltime
            if (ButtonController.leaderboardType == LocalLeaderboardsModel.LeaderboardType.Daily)
            {
                if (PartyData.daily_scores.TryGetValue(ButtonController.current_leaderboard, out temp) == false)
                {
                    return;
                }
            }

            else
            {
                if (PartyData.all_scores.TryGetValue(ButtonController.current_leaderboard, out temp) == false)
                {
                    return;
                }
            }

            // A real score and not a dummy placeholder scores in data files
            if (temp.map_scores[row].playername == "")
            {
                return;
            }

            temp_playername = temp.map_scores[row].playername;
            Playername = "changed";


            temp_raw_score = temp.map_scores[row].raw_score.ToString("N0");
            Raw_Score = "changed";

            if (temp.map_scores[row].fc)
            {
                temp_rank = temp.map_scores[row].rank + " - FC";
            }
            else
            {
                temp_rank = temp.map_scores[row].rank;
            }
            Rank = "changed";

            temp_acc = String.Format("{0:0.00}", temp.map_scores[row].acc) + "%";
            Acc = "changed";

            // Display only if GPM used
            if (CustomScoreData.Read_Custom_Gameplaymodifiers(temp.map_scores[row].custom_gameplaymodifiers) == "None")
            {
                temp_mod_score = "<#b3b3cc>Mod: <#ffffff>---";
                temp_mod_acc = "<#b3b3cc>Mod: <#ffffff>---";
            }
            else
            {
                temp_mod_score = "<#b3b3cc>Mod: <#ffffff>" + temp.map_scores[row].mod_score.ToString("N0");
                temp_mod_acc = "<#b3b3cc>Mod: <#ffffff>" + String.Format("{0:0.00}", temp.map_scores[row].mod_acc) + "%";
            }
            Mod_Score = "changed";
            Mod_Acc = "changed";


            temp_stats = "<#b3b3cc>Date Set: <#ffffff>" + Convert_Timestamp(temp.map_scores[row].timestamp) + "\n" + 
                        "<#b3b3cc>Accuracy: <#ff0000>" + String.Format("{0:0.00}", temp.map_scores[row].left_acc) + " <#ffffff>- <#1a53ff>" + String.Format("{0:0.00}", temp.map_scores[row].right_acc) + "<#ffffff>, " + "<#ff0000>" + String.Format("{0:0.00}", temp.map_scores[row].left_mixed_acc) + " <#ffffff>- <#1a53ff>" + String.Format("{0:0.00}", temp.map_scores[row].right_mixed_acc) + "\n" +
                        "<#b3b3cc>Max Combo: <#ffffff>" + temp.map_scores[row].longest_combo + "\n" +
                        "<#b3b3cc>Good Cuts: <#ffffff>" + temp.map_scores[row].good_cuts + "\n" +
                        "<#b3b3cc>Bad Cuts: <#ffffff>" + temp.map_scores[row].bad_cuts + "\n" +
                        "<#b3b3cc>Missed: <#ffffff>" + temp.map_scores[row].missed + "\n" +
                        "<#b3b3cc>Bomb Hits: <#ffffff>" + temp.map_scores[row].bombs + "\n" +
                        "<#b3b3cc>Modifiers: <#ffffff>" + CustomScoreData.Read_Custom_Gameplaymodifiers(temp.map_scores[row].custom_gameplaymodifiers);
            Stats = "changed";
            //Plugin.Log.Debug(temp_stats);

            temp_mod_hints = temp.map_scores[row].modifier_hints;
            Mod_Hints = temp_mod_hints;
        }


        private string temp_playername = "";
        private string temp_raw_score = "";
        private string temp_mod_score = "";
        private string temp_rank = "";
        private string temp_acc = "";
        private string temp_mod_acc = "";
        private static string temp_stats = "";
        private static string temp_mod_hints = "";


        [UIValue("playername")]
        private string Playername
        {
            get => temp_playername;
            set
            {
                NotifyPropertyChanged();
            }
        }

        [UIValue("raw_score")]
        private string Raw_Score
        {
            get => temp_raw_score;
            set
            {
                NotifyPropertyChanged();
            }
        }

        [UIValue("mod_score")]
        private string Mod_Score
        {
            get => temp_mod_score;
            set
            {
                NotifyPropertyChanged();
            }
        }

        [UIValue("rank")]
        private string Rank
        {
            get => temp_rank;
            set
            {
                NotifyPropertyChanged();
            }
        }

        [UIValue("acc")]
        private string Acc
        {
            get => temp_acc;
            set
            {
                NotifyPropertyChanged();
            }
        }

        [UIValue("mod_acc")]
        private string Mod_Acc
        {
            get => temp_mod_acc;
            set
            {
                NotifyPropertyChanged();
            }
        }

        [UIValue("stats")]
        private string Stats
        {
            get => temp_stats;
            set
            {
                NotifyPropertyChanged();
            }
        }

        [UIValue("mod_hints")]
        private string Mod_Hints
        {
            get => temp_mod_hints;
            set
            {
                temp_mod_hints = value;
                NotifyPropertyChanged(nameof(Mod_Hints));
            }
        }

        private void Set_Empty_Fields()
        {
            temp_playername = "";
            Playername = "changed";

            temp_raw_score = "";
            temp_mod_score = "";
            Raw_Score = "changed";
            Mod_Score = "changed";

            temp_rank = "";
            Rank = "changed";

            temp_acc = "";
            Acc = "changed";

            temp_mod_acc = "";
            Mod_Acc = "changed";

            temp_stats = "\nNo data for this score.\nPlay this map and set one now!\n\n\n\n<size=3><#ffff00>Special request for Aroc. Thanks!\n\n<size=2.5><#ff0080>http://github.com/zeph-yr/PartyExtensions \nZephyr#9125";
            Stats = "changed";

            temp_mod_hints = "This hasn't been played yet."; //Should never see this, but just in case
            Mod_Hints = temp_mod_hints;
        }


        private static string Convert_Timestamp(long unix_timestamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddSeconds(unix_timestamp).ToLocalTime().ToString();
        }


        // Keep this note:
        // Using the basegame gpm works right after playing a map, but the data file will randomly change to all false.
        // For example, the LocalScores will have the correct set of gpm but DailyLocalScores will be all false, or vice versa.
        // Maybe this is a reference and there is a race condition? Not sure so went the enum hardcode route to save time. Not worth figuring out.
        
        // Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"Modifiers: {Convert_GPM(temp.map_scores[row].modifiers)}"));
        // Update: Convert_GPM used to be here. Moved to CustomScoreData for reuse
        

        /*private void Fill_Modal_Test()
        {
            Plugin.Log.Debug("Fill Modal");

            Modal_List.data.Clear();

            for (int i = 0; i < 10; i++)
            {
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo("-------"));
                Plugin.Log.Debug("+++++++++++++++++");
            }

            Modal_List.tableView.ReloadData();
            Modal_List.tableView.ClearSelection();
        }*/
    }
}