using Newtonsoft.Json;
using UnityEngine;

namespace PartyExtensions
{
    public class PartyExtensionsController : MonoBehaviour, ISaberSwingRatingCounterDidFinishReceiver
    {
        public static PartyExtensionsController Instance { get; private set; }

        internal static float left_acc = 0f;
        internal static float right_acc = 0f;

        internal static int left_hits = 0;
        internal static int right_hits = 0;

        internal static int bomb_hits = 0;


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

                right_acc = 0;
                right_hits = 0;

                bomb_hits = 0;

                //PartyData.is_written = false;

                BS_Utils.Utilities.BSEvents.noteWasCut += Instance.BSEvents_noteWasCut;
                BS_Utils.Utilities.BSEvents.levelCleared += BSEvents_levelCleared;
            }
            else
            {
                //Plugin.Log.Debug("Not Party");

                BS_Utils.Utilities.BSEvents.noteWasCut -= Instance.BSEvents_noteWasCut;
                BS_Utils.Utilities.BSEvents.levelCleared -= BSEvents_levelCleared;
            }
        }

        private static void BSEvents_levelCleared(StandardLevelScenesTransitionSetupDataSO arg1, LevelCompletionResults arg2)
        {
            Plugin.Log.Debug("Level cleared");

            // This might not be what the user wants
            //float total_acc = (left_acc + right_acc) / (left_hits + right_hits);

            int total_cubes = arg2.goodCutsCount + arg2.missedCount + arg2.badCutsCount;
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
            }

            //Plugin.Log.Debug("Max score: " + max_score);
            float total_acc = arg2.rawScore / max_score * 100;
            float mod_acc = arg2.modifiedScore / max_score * 100;
            
            float final_left_acc = left_acc / left_hits;
            float final_right_acc = right_acc / right_hits;

            //Plugin.Log.Debug($"Final: {total_acc} {final_left_acc} {final_right_acc}");

            /*
            Plugin.Log.Debug("Read Config:");
            Plugin.Log.Debug("map_score. left acc:" + PluginConfig.Instance.map_score.left_acc);
            Plugin.Log.Debug("map_leaderboard. id:" + PluginConfig.Instance.map_leaderboard.leaderboard_id);


            CustomScoreData map_score = new CustomScoreData(arg2.rank.ToString(), arg2.missedCount, arg2.goodCutsCount, arg2.badCutsCount, final_left_acc, final_right_acc, arg2.gameplayModifiers, arg2.maxCombo, DateTime.Now.Ticks, "zeph");
            Plugin.Log.Debug(JsonConvert.SerializeObject(map_score, Formatting.Indented));

            Plugin.Log.Debug("Write map score");
            PluginConfig.Instance.map_score = map_score;

            //CustomLeaderboard map_leaderboard = new CustomLeaderboard("this map");
            //map_leaderboard.map_scores.Add(map_score);
            //Plugin.Log.Debug(JsonConvert.SerializeObject(map_leaderboard, Formatting.Indented));

            Plugin.Log.Debug("Write map leaderboard");
            //PluginConfig.Instance.map_leaderboard = map_leaderboard;


            PluginConfig.Instance.map_leaderboard = new CustomLeaderboard("this map");
            PluginConfig.Instance.map_leaderboard.map_scores.Add(map_score);

            Plugin.Log.Debug("map_leaderboard: " + PluginConfig.Instance.map_leaderboard.map_scores[0].left_acc); //This data is stored but not being serialized properly
            */

            PartyData.current_score = new CustomScoreData(arg2.rank.ToString(), arg2.missedCount, arg2.goodCutsCount, arg2.badCutsCount, bomb_hits, arg2.rawScore, arg2.modifiedScore, false, total_acc, mod_acc, final_left_acc, final_right_acc, arg2.gameplayModifiers, arg2.maxCombo, 0 /*DateTime.Now.Ticks*/, "Zeph"); //hehe
            
            //Plugin.Log.Debug(JsonConvert.SerializeObject(PartyData.current_score));
            //PartyData.Write();
        }

        private static int preswing;
        private static int postswing;
        private static int center;
        private static float dist;
        private static string color;

        private void BSEvents_noteWasCut(NoteData arg1, NoteCutInfo arg2, int arg3) // Must be non-static
        {
            // Only want the good cuts
            // Need the arg2 check for wrong direction, wrong color, miss, or it will error a lot in the console
            if (arg1 != null && arg2.allIsOK)
            {
                if (arg1.colorType == ColorType.ColorA)
                {
                    color = "A";
                }

                else if (arg1.colorType == ColorType.ColorB)
                {
                    color = "B";
                }

                dist = arg2.cutDistanceToCenter;
                arg2.swingRatingCounter.RegisterDidFinishReceiver(this);
            }

            else if (arg1 != null && arg1.colorType == ColorType.None) // Can even add bombs cut here as a bonus
            {
                bomb_hits++;
            }
        }

        public void HandleSaberSwingRatingCounterDidFinish(ISaberSwingRatingCounter saberSwingRatingCounter) // Must be public
        {
            ScoreModel.RawScoreWithoutMultiplier(saberSwingRatingCounter, dist, out preswing, out postswing, out center);

            if (color == "A")
            {
                left_acc += (preswing + postswing + center);
                left_hits++;
            }

            else if (color == "B")
            {
                right_acc += (preswing + postswing + center);
                right_hits++;
            }

            //Plugin.Log.Debug($"-----------------------------------------------------------------------");
            //Plugin.Log.Debug($"left: {left_acc} | {preswing} {postswing} {center} | {left_hits}");
            //Plugin.Log.Debug($"right: {right_acc} | {preswing} {postswing} {center} | {right_hits}");
        }


        private void OnDisable()
        {
            BS_Utils.Utilities.BSEvents.lateMenuSceneLoadedFresh -= BSEvents_lateMenuSceneLoadedFresh;
            BS_Utils.Utilities.BSEvents.gameSceneLoaded -= BSEvents_gameSceneLoaded;

            BS_Utils.Utilities.BSEvents.noteWasCut -= BSEvents_noteWasCut;
            BS_Utils.Utilities.BSEvents.levelCleared -= BSEvents_levelCleared;
        }


        private void OnDestroy()
        {
            Plugin.Log?.Debug($"{name}: OnDestroy()");
            if (Instance == this)
                Instance = null; // This MonoBehaviour is being destroyed, so set the static instance property to null.
        }
    }
}