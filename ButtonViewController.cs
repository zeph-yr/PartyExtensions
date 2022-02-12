using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using TMPro;

namespace PartyExtensions
{
    [HotReload(@"ButtonViewController.bsml")]
    public partial class ButtonViewController : BSMLAutomaticViewController
    {
        internal ButtonController ParentCoordinator;

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

        /*internal class ButtonViewController : BSMLResourceViewController
    {
        // For this method of setting the ResourceName, this class must be the first class in the file.
        //public override string ResourceName => string.Join(".", GetType().Namespace, GetType().Name);

    }*/
}
