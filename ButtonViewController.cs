using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using System;
using TMPro;

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

            Set_Modal_Content(row);
            //Fill_Modal_Test();

            // This is absolutely critical!! Not sure why but "button_clicked" doesn't work as show-event
            parserParams.EmitEvent("open-modal"); // First click results in blank modal

            Button_List.tableView.ClearSelection(); // Otherwise user has to click away and back if they want to open the same score again
        }


        private void Fill_Button_List()
        {
            Button_List.data.Clear();

            for (int i = 0; i < 10; i++)
            {
                Button_List.data.Add(new CustomListTableData.CustomCellInfo("i"));
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


        [UIComponent("modal_list")]
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
              
                    return;
                }
            }

            // For the dummy placeholder scores in data files
            if (temp.map_scores[row].playername == "")
            {
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"No data for this score"));
            }

            else
            {
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"Player: {temp.map_scores[row].playername}"));
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"Rank: {temp.map_scores[row].rank}"));
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"Accuracy: {temp.map_scores[row].acc} - " + String.Format("{0:0.00}", temp.map_scores[row].acc / 115 * 100) + "%"));
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"<#ffffff>Left-Right: <#ff0000>" + String.Format("{0:0.00}", temp.map_scores[row].left_acc) + " - <#0000ff>" + String.Format("{0:0.00}", temp.map_scores[row].right_acc)));
                //Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"Right: {temp.map_scores[row].right_acc}"));
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"Longest Combo: {temp.map_scores[row].longest_combo}"));
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"Bad Cuts: {temp.map_scores[row].bad_cuts}"));
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"Missed: {temp.map_scores[row].missed}"));
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"Time: {Convert_Timestamp(temp.map_scores[row].timestamp)}"));
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"Modifiers: {CustomScoreData.Read_Custom_Gameplaymodifiers(temp.map_scores[row].custom_gameplaymodifiers)}"));
            }

            Modal_List.tableView.ReloadData();
            Modal_List.tableView.ClearSelection();
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

        /*private static string Convert_GPM(GameplayModifiers gameplayModifiers)
        {
            string result = "";

            // Fail and Life Modifiers
            if (gameplayModifiers.noFailOn0Energy)
            {
                result += "NF, ";
            }

            if (gameplayModifiers.instaFail)
            {
                result += "1-L, ";
            }

            switch ((int)gameplayModifiers.energyType)
            {
                case 0:
                    break;
                case 1:
                    result += "4-L, ";
                    break;
                default:
                    break;
            }


            // Bombs and Walls
            if (gameplayModifiers.noBombs)
            {
                result += "NB, ";
            }

            switch ((int)gameplayModifiers.enabledObstacleType)
            {
                case 0:
                    break;
                case 1:
                    result += "FH, "; // Probably wont show up
                    break;
                case 2:
                    result += "NO, ";
                    break;
                default:
                    break;
            }

            // Arrow Modifiers
            if (gameplayModifiers.noArrows)
            {
                result += "NA, ";
            }

            if (gameplayModifiers.ghostNotes)
            {
                result += "GN, ";
            }

            if (gameplayModifiers.disappearingArrows)
            {
                result += "DA, ";
            }
         
            
            // Acc and Block Modifiers
            if (gameplayModifiers.smallCubes)
            {
                result += "SC, ";
            }

            if (gameplayModifiers.proMode)
            {
                result += "PM, ";
            }
   
            if (gameplayModifiers.strictAngles)
            {
                result += "SA, ";
            }

            if (gameplayModifiers.zenMode)
            {
                result += "ZM, ";
            }


            // Speed Modifiers
            switch ((int)gameplayModifiers.songSpeed)
            {
                case 0:
                    break;
                case 1:
                    result += "FS, ";
                    break;
                case 2:
                    result += "SS, ";
                    break;
                case 3:
                    result += "SFS, ";
                    break;
                default:
                    break;
            }

            Plugin.Log.Debug(result);

            return result.Trim(',', ' ');
        }*/


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


        /*[UIComponent("cancelbutton")]
        private TextMeshProUGUI cancelbutton_text;

        [UIAction("disablescore")]
        protected void ClickButtonAction()
        {
            Plugin.Log.Debug("disable_clicked");
        }*/

    }
}

/*		<horizontal bg='round-rect-panel' bg-color='#ffffffdd'>
			<button text='akjsdlfj' on-click='disablescore' event-clicked='button_clicked' id='cancelbutton' min-width='30' min-height='10' rich-text='true'></button>
			<text text='PartyExtensions WOOHOO' align='Left'></text>
		</horizontal>*/