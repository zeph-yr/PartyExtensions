using UnityEngine;

namespace PartyExtensions
{
    public class PartyExtensionsController : MonoBehaviour
    {
        public static PartyExtensionsController Instance { get; private set; }

        internal static float left_acc = 0f;
        internal static float right_acc = 0f;

        internal static int left_hits = 0;
        internal static int right_hits = 0;

        internal static int bomb_hits = 0;

        internal static float left_slider_acc = 0f;
        internal static float right_slider_acc = 0f;

        internal static float left_slider_hits = 0f;
        internal static float right_slider_hits = 0f;


        private void Awake()
        {
            // For this particular MonoBehaviour, we only want one instance to exist at any time, so store a reference to it in a static property
            //   and destroy any that are created while one already exists.
            if (Instance != null)
            {
                Plugin.Log?.Warn($"Instance of {GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }
            GameObject.DontDestroyOnLoad(this); // Don't destroy this object on scene changes
            Instance = this;
            Plugin.Log?.Debug($"{name}: Awake()");
        }


        private void OnEnable()
        {
            BS_Utils.Utilities.BSEvents.lateMenuSceneLoadedFresh += BSEvents_lateMenuSceneLoadedFresh;
            BS_Utils.Utilities.BSEvents.gameSceneLoaded += BSEvents_gameSceneLoaded;
        }

        // This is critical
        private void BSEvents_lateMenuSceneLoadedFresh(ScenesTransitionSetupDataSO obj)
        {
            ButtonController.Instance.Create_Buttons();
        }

        private static void BSEvents_gameSceneLoaded()
        {
            if (BS_Utils.Gameplay.Gamemode.IsPartyActive)
            {
                Plugin.Log.Debug("In Party Mode");

                left_acc = 0;
                left_hits = 0;
                left_slider_acc = 0;
                left_slider_hits = 0;

                right_acc = 0;
                right_hits = 0;
                right_slider_acc = 0;
                right_slider_hits = 0;

                bomb_hits = 0;

                BS_Utils.Utilities.BSEvents.levelCleared += BSEvents_levelClearedAsync;
            }
            else
            {
                //Plugin.Log.Debug("Not Party");

                BS_Utils.Utilities.BSEvents.levelCleared -= BSEvents_levelClearedAsync;
            }
        }


        private static async void BSEvents_levelClearedAsync(StandardLevelScenesTransitionSetupDataSO arg1, LevelCompletionResults arg2)
        {
            Plugin.Log.Debug("Level cleared");

            // This might not be what the user wants
            //float total_acc = (left_acc + right_acc) / (left_hits + right_hits);

            /*int total_cubes = arg2.goodCutsCount + arg2.missedCount + arg2.badCutsCount;
            //Plugin.Log.Debug("Total cubes: " + total_cubes);

            // Thanks Dennis!
            float max_score;
            if (total_cubes > 14)
            {
                //115 + 4 * 230 + 8 * 460 + (total_cubes - 13) * 920;
                max_score = total_cubes * 920 - 7245;
            }
            else if (total_cubes > 5) // 13 cubes 
            {
                //115 + 4 * 230 + (total_cubes - 5) * 460;
                max_score = total_cubes * 460 - 1265;
            }
            else if (total_cubes > 0) // 5 cubes
            {
                //115 + (total_cubes - 1) * 230;
                max_score = total_cubes * 230 - 115;
            }
            else // 0 cube
            {
                max_score = 0.001f;
            }*/


            int max_score = ScoreModel.ComputeMaxMultipliedScoreForBeatmap(await(arg1.difficultyBeatmap.GetBeatmapDataAsync(arg1.environmentInfo)));

            float total_acc = (float)arg2.multipliedScore / max_score * 100;
            float mod_acc = (float)arg2.modifiedScore / max_score * 100;
            
            float final_left_acc = left_acc / left_hits;
            float final_right_acc = right_acc / right_hits;

            // BS 1.20.0
            float final_left_mixed_acc = (left_acc + left_slider_acc) / (left_hits + left_slider_hits);
            float final_right_mixed_acc = (right_acc + right_slider_acc) / (right_hits + right_slider_hits);

            //Plugin.Log.Debug("Max score: " + max_score);
            //Plugin.Log.Debug("Multiplied score: " + arg2.multipliedScore);
            //Plugin.Log.Debug("Modified score: " + arg2.modifiedScore);
            //Plugin.Log.Debug($"Final: {total_acc} {final_left_acc} {final_right_acc}");

            PartyData.current_score = new CustomScoreData(arg2.rank.ToString(), arg2.missedCount, arg2.goodCutsCount, arg2.badCutsCount, bomb_hits, arg2.multipliedScore, arg2.modifiedScore, false, total_acc, mod_acc, final_left_acc, final_right_acc, final_left_mixed_acc, final_right_mixed_acc, arg2.gameplayModifiers, arg2.maxCombo, 0 /*DateTime.Now.Ticks*/, "Zeph"); //hehe
            //Plugin.Log.Debug(JsonConvert.SerializeObject(PartyData.current_score));
        }


        private void OnDisable()
        {
            BS_Utils.Utilities.BSEvents.lateMenuSceneLoadedFresh -= BSEvents_lateMenuSceneLoadedFresh;
            BS_Utils.Utilities.BSEvents.gameSceneLoaded -= BSEvents_gameSceneLoaded;
            BS_Utils.Utilities.BSEvents.levelCleared -= BSEvents_levelClearedAsync;
        }


        private void OnDestroy()
        {
            Plugin.Log?.Debug($"{name}: OnDestroy()");
            if (Instance == this)
                Instance = null; // This MonoBehaviour is being destroyed, so set the static instance property to null.
        }
    }
}