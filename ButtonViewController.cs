using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
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
        //private ButtonListElement Element = null;

        [UIAction("button_clicked")]
        private void Button_Clicked(TableView tableView, int row)
        {
            Set_Modal_Content(0);

        }


        internal void Set_Modal_Content(int row)
        {
            Left_Acc = PartyData.all_scores[ButtonController.current_leaderboard].map_scores[row].left_acc;
        }


        [UIValue("left_acc")]
        private float Left_Acc;





















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
        }






        [UIComponent("cancelbutton")]
        private TextMeshProUGUI cancelbutton_text;

        [UIAction("disablescore")]
        protected void ClickButtonAction()
        {

        }

        public void UpdateText()
        {

        }





    }






}
