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


        public void Show_Buttons()
        {
            if (floatingScreen == null)
            {
                floatingScreen = CreateFloatingScreen();

                buttonViewController = BeatSaberUI.CreateViewController<ButtonViewController>();
                buttonViewController.button_controller = this;
                floatingScreen.SetRootViewController(buttonViewController, HMUI.ViewController.AnimationType.None);

                floatingScreen.gameObject.SetActive(false);

                LocalLeaderboardViewController localLeaderboardViewController = Resources.FindObjectsOfTypeAll<LocalLeaderboardViewController>().FirstOrDefault();
                localLeaderboardViewController.didActivateEvent += LocalLeaderboardViewController_didActivateEvent;
                localLeaderboardViewController.didDeactivateEvent += LocalLeaderboardViewController_didDeactivateEvent;
            }
        }


        private FloatingScreen CreateFloatingScreen()
        {
            Quaternion rotation = new Quaternion(0f, 0f, 0f, 0f);
            //rotation *= Quaternion.Euler(Vector3.up * 90);

            //80, 100
            FloatingScreen screen = FloatingScreen.CreateFloatingScreen(new Vector2(80, 100), false, new Vector3(-0.08f, 1.05f, 1.95f), rotation);

            GameObject.DontDestroyOnLoad(screen.gameObject);
            return screen;
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
