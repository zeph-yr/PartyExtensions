using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using TMPro;

namespace PartyExtensions
{
    [HotReload(@"ButtonViewController.bsml")]
    public partial class ButtonViewController : BSMLAutomaticViewController
    {
        internal ButtonController ParentCoordinator;



        [UIComponent("button_list")]
        public CustomListTableData Button_List;


        [UIAction("button_clicked")]
        private void Button_Clicked(TableView tableView, int row)
        {
            Plugin.Log.Debug("button clicked: " + row);


            //Set_Modal_Content(row);
            Fill_Modal_Test();

            parserParams.EmitEvent("open-modal");
        }






        [UIParams]
        private BSMLParserParams parserParams;

        [UIComponent("modal")]
        public ModalView Modal;



        [UIComponent("modal_list")]
        public CustomListTableData Modal_List;


        /*private void Set_Modal_Content(int row)
        {
            Modal_List.data.Clear();

            if (ButtonController.leaderboardType == LocalLeaderboardsModel.LeaderboardType.Daily)
            {
                CustomLeaderboard temp;
                if (PartyData.daily_scores.TryGetValue(ButtonController.current_leaderboard, out temp) == false)
                {
                    //
                    return;
                }

                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"Player: {temp.map_scores[row].playername}"));
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"Rank: {temp.map_scores[row].rank}"));
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"Left Acc: {temp.map_scores[row].left_acc}"));
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"Right Acc: {temp.map_scores[row].right_acc}"));
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"Longest Combo: {temp.map_scores[row].longest_combo}"));
                Modal_List.data.Add(new CustomListTableData.CustomCellInfo($"Missed: {temp.map_scores[row].missed}"));

            }

            else
            {

            }

            Modal_List.tableView.ReloadData();
            Modal_List.tableView.ClearSelection();
        }*/


        /*protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            if (!firstActivation)
            {
                Set_Modal_Content(0);
            }
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
        }*/









        private void Fill_Modal_Test()
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
        }











        private void Fill_List()
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
            Fill_List();
            //Fill_Modal_Test();
        }






        [UIComponent("cancelbutton")]
        private TextMeshProUGUI cancelbutton_text;

        [UIAction("disablescore")]
        protected void ClickButtonAction()
        {
            Plugin.Log.Debug("disable_clicked");
            //Fill_Modal_Test();
        }

    }

}

/*				
 *				<background bg='round-rect-panel' bg-color='#ff0000dd'>	</background><horizontal bg='round-rect-panel' bg-color='#ffffffdd'>
					<button text='~left_acc' on-click='disablescore' id='cancelbutton' min-width='30' min-height='10' rich-text='true'></button>
					<text text='PartyExtensions WOOHOO' align='Left'></text>
				</horizontal>*/