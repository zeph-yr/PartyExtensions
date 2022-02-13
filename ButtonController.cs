using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using System.Linq;
using UnityEngine;

namespace PartyExtensions
{
    class ButtonController
    {
        public FloatingScreen floatingScreen;
        public ButtonViewController buttonViewController;

        public static string current_leaderboard;
        public static LocalLeaderboardsModel.LeaderboardType leaderboardType;



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
                buttonViewController.ParentCoordinator = this;
                floatingScreen.SetRootViewController(buttonViewController, HMUI.ViewController.AnimationType.None);

                floatingScreen.gameObject.SetActive(false);

                LocalLeaderboardViewController localLeaderboardViewController = Resources.FindObjectsOfTypeAll<LocalLeaderboardViewController>().FirstOrDefault();
                localLeaderboardViewController.didActivateEvent += LocalLeaderboardViewController_didActivateEvent;
                localLeaderboardViewController.didDeactivateEvent += LocalLeaderboardViewController_didDeactivateEvent;
            }
        }


        public FloatingScreen CreateFloatingScreen()
        {
            Quaternion rotation = new Quaternion(0f, 0f, 0f, 0f);
            //rotation *= Quaternion.Euler(Vector3.up * 90);

            //80, 100
            FloatingScreen screen = FloatingScreen.CreateFloatingScreen(new Vector2(300, 300), false, new Vector3(-0.08f, 1.05f, 1.95f), rotation);

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
