using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using System.Linq;
using UnityEngine;

namespace PartyExtensions
{
    internal class ButtonController
    {
        internal static string current_leaderboard;
        internal static LocalLeaderboardsModel.LeaderboardType leaderboardType;

        private FloatingScreen floatingScreen;
        private ButtonViewController buttonViewController;


        public static ButtonController _instance { get; private set; }

        public static ButtonController Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ButtonController();
                return _instance;
            }
        }


        public void Create_Buttons()
        {
            if (floatingScreen == null)
            {
                // Make FloatingScreen:
                Quaternion rotation = new Quaternion(0f, 0f, 0f, 0f);
                //rotation *= Quaternion.Euler(Vector3.up * 90);

                floatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(80, 100), false, new Vector3(-0.08f, 1.05f, 1.95f), rotation); //Size: 80, 100
                GameObject.DontDestroyOnLoad(floatingScreen.gameObject);

                // Make ViewControlller and give it to the FloatingScreen:
                buttonViewController = BeatSaberUI.CreateViewController<ButtonViewController>();
                floatingScreen.SetRootViewController(buttonViewController, HMUI.ViewController.AnimationType.None);

                floatingScreen.gameObject.SetActive(false);

                // Make FloatingScreen appear/disappear with party leaderboard
                LocalLeaderboardViewController localLeaderboardViewController = Resources.FindObjectsOfTypeAll<LocalLeaderboardViewController>().FirstOrDefault();
                localLeaderboardViewController.didActivateEvent += LocalLeaderboardViewController_didActivateEvent;
                localLeaderboardViewController.didDeactivateEvent += LocalLeaderboardViewController_didDeactivateEvent;
            }
        }


        private void LocalLeaderboardViewController_didDeactivateEvent(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            floatingScreen.gameObject.SetActive(false);
        }


        private void LocalLeaderboardViewController_didActivateEvent(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            floatingScreen.gameObject.SetActive(true);
        }
    }
}
