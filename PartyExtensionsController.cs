using Newtonsoft.Json;
//using PartyExtensions.Configuration;
using System;
using UnityEngine;


namespace PartyExtensions
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    public class PartyExtensionsController : MonoBehaviour, ISaberSwingRatingCounterDidFinishReceiver
    {
        public static PartyExtensionsController Instance { get; private set; }

        internal static float left_acc = 0f;
        internal static float right_acc = 0f;

        internal static int left_hits = 0;
        internal static int right_hits = 0;

        internal static float final_left_acc = 0f;
        internal static float final_right_acc = 0f;


        // These methods are automatically called by Unity, you should remove any you aren't using.
        #region Monobehaviour Messages
        /// <summary>
        /// Only ever called once, mainly used to initialize variables.
        /// </summary>
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
        /// <summary>
        /// Only ever called once on the first frame the script is Enabled. Start is called after any other script's Awake() and before Update().
        /// </summary>
        private void Start()
        {

        }


        /// <summary>
        /// Called when the script becomes enabled and active
        /// </summary>
        private void OnEnable()
        {
            BS_Utils.Utilities.BSEvents.gameSceneLoaded += BSEvents_gameSceneLoaded;
            BS_Utils.Utilities.BSEvents.noteWasCut += BSEvents_noteWasCut;

            BS_Utils.Utilities.BSEvents.levelCleared += BSEvents_levelCleared;

        }


        private static void BSEvents_levelCleared(StandardLevelScenesTransitionSetupDataSO arg1, LevelCompletionResults arg2)
        {
            Plugin.Log.Debug("Level cleared");

            final_left_acc = left_acc / left_hits;
            final_right_acc = right_acc / right_hits;

            Plugin.Log.Debug($"Final: {final_left_acc} {final_right_acc}");

            /*Plugin.Log.Debug("Read Config:");
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

            PartyData.current_score = new CustomScoreData(arg2.rank.ToString(), arg2.missedCount, arg2.goodCutsCount, arg2.badCutsCount, final_left_acc, final_right_acc, arg2.gameplayModifiers, arg2.maxCombo, 0 /*DateTime.Now.Ticks*/, "Zeph");
            //PartyData.Write();
        }

        private static void BSEvents_gameSceneLoaded()
        {
            Plugin.Log.Debug("Game scene loaded");

            final_left_acc = 0;
            final_right_acc = 0;

            left_acc = 0;
            left_hits = 0;

            right_acc = 0;
            right_hits = 0;

            PartyData.is_written = false;
        }

        int preswing;
        int postswing;
        int center;
        float dist;

        string color;

        private void BSEvents_noteWasCut(NoteData arg1, NoteCutInfo arg2, int arg3)
        {
            if (arg1 != null)
            {
                if (arg1.colorType == ColorType.ColorA)
                {
                    color = "A";
                }

                else if (arg1.colorType == ColorType.ColorB)
                {
                    color = "B";
                }

                else
                {
                    return;
                }

                dist = arg2.cutDistanceToCenter;
                arg2.swingRatingCounter.RegisterDidFinishReceiver(this);
            }
        }

        public void HandleSaberSwingRatingCounterDidFinish(ISaberSwingRatingCounter saberSwingRatingCounter)
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




        /*
        [DEBUG @ 23:07:55 | PartyExtensions] left: 70 6 15 18
        [DEBUG @ 23:07:55 | PartyExtensions] left: 70 30 15 19
        [DEBUG @ 23:07:56 | PartyExtensions] left: 70 0 15 20
        [DEBUG @ 23:07:56 | PartyExtensions] left: 70 30 15 21
        [DEBUG @ 23:07:56 | PartyExtensions] left: 70 0 15 22
        [DEBUG @ 23:07:57 | PartyExtensions] left: 70 30 15 23
        */


        /*
        [DEBUG @ 23:56:52 | PartyExtensions] left: 70 0 15 198
        [DEBUG @ 23:56:52 | PartyExtensions] right: 70 0 15 184
        [DEBUG @ 23:56:52 | PartyExtensions] left: 70 0 15 199
        [DEBUG @ 23:56:52 | PartyExtensions] right: 70 0 15 185
        [DEBUG @ 23:56:52 | PartyExtensions] left: 70 0 15 200
        [DEBUG @ 23:56:52 | PartyExtensions] right: 70 0 15 186
        [DEBUG @ 23:56:52 | PartyExtensions] left: 70 0 15 201
        [DEBUG @ 23:56:52 | PartyExtensions] right: 70 0 15 187
        [DEBUG @ 23:56:52 | PartyExtensions] left: 70 25 1 202
        [DEBUG @ 23:56:52 | PartyExtensions] left: 70 30 8 203
        */



        /// <summary>
        /// Called when the script becomes disabled or when it is being destroyed.
        /// </summary>
        private void OnDisable()
        {

        }

        /// <summary>
        /// Called when the script is being destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Plugin.Log?.Debug($"{name}: OnDestroy()");
            if (Instance == this)
                Instance = null; // This MonoBehaviour is being destroyed, so set the static instance property to null.

        }
        #endregion
    }
}

/*[DEBUG @ 23:29:26 | PartyExtensions] left: 1380 | 70 15 11 | 12
[DEBUG @ 23:29:26 | PartyExtensions] right: 3026 | 70 15 11 | 29
[DEBUG @ 23:29:26 | PartyExtensions] -----------------------------------------------------------------------
[DEBUG @ 23:29:26 | PartyExtensions] left: 1380 | 70 30 11 | 12
[DEBUG @ 23:29:26 | PartyExtensions] right: 3137 | 70 30 11 | 30
[DEBUG @ 23:29:27 | PartyExtensions] -----------------------------------------------------------------------
[DEBUG @ 23:29:27 | PartyExtensions] left: 1380 | 70 30 11 | 12
[DEBUG @ 23:29:27 | PartyExtensions] right: 3248 | 70 30 11 | 31
[DEBUG @ 23:29:27 | PartyExtensions] -----------------------------------------------------------------------
[DEBUG @ 23:29:27 | PartyExtensions] left: 1380 | 70 30 11 | 12
[DEBUG @ 23:29:27 | PartyExtensions] right: 3359 | 70 30 11 | 32
[DEBUG @ 23:29:27 | PartyExtensions] -----------------------------------------------------------------------
[DEBUG @ 23:29:27 | PartyExtensions] left: 1380 | 70 30 2 | 12
[DEBUG @ 23:29:27 | PartyExtensions] right: 3461 | 70 30 2 | 33
[DEBUG @ 23:29:27 | PartyExtensions] -----------------------------------------------------------------------
[DEBUG @ 23:29:27 | PartyExtensions] left: 1380 | 70 21 2 | 12
[DEBUG @ 23:29:27 | PartyExtensions] right: 3554 | 70 21 2 | 34
[DEBUG @ 23:29:27 | PartyExtensions] -----------------------------------------------------------------------
[DEBUG @ 23:29:27 | PartyExtensions] left: 1495 | 70 30 15 | 13
[DEBUG @ 23:29:27 | PartyExtensions] right: 3554 | 70 30 15 | 34
[DEBUG @ 23:29:27 | PartyExtensions] -----------------------------------------------------------------------
[DEBUG @ 23:29:27 | PartyExtensions] left: 1610 | 70 30 15 | 14
[DEBUG @ 23:29:27 | PartyExtensions] right: 3554 | 70 30 15 | 34
[DEBUG @ 23:29:28 | BS_Utils] Triggering LevelFinishEvent.
[DEBUG @ 23:29:28 | BS_Utils] Solo/Party mode level finished.
[DEBUG @ 23:29:28 | BS_Utils] Solo/Party mode level finished.
[DEBUG @ 23:29:28 | PartyExtensions] level finished
[INFO @ 23:29:28 | BeatSaviorData] BSD : Song stats saved successfully.
[DEBUG @ 23:29:29 | PartyExtensions] level cleared
[DEBUG @ 23:29:29 | PartyExtensions] final: 115 104.5294

[DEBUG @ 23:50:06 | PartyExtensions] level cleared
[DEBUG @ 23:50:06 | PartyExtensions] final: 115 103.2453
[DEBUG @ 23:50:06 | PartyExtensions] {
[DEBUG @ 23:50:06 | PartyExtensions]   "rank": "SS",
[DEBUG @ 23:50:06 | PartyExtensions]   "missed": 0,
[DEBUG @ 23:50:06 | PartyExtensions]   "good_cuts": 95,
[DEBUG @ 23:50:06 | PartyExtensions]   "bad_cuts": 0,
[DEBUG @ 23:50:06 | PartyExtensions]   "left_acc": 115.0,
[DEBUG @ 23:50:06 | PartyExtensions]   "right_acc": 103.245285,
[DEBUG @ 23:50:06 | PartyExtensions]   "longest_combo": 95,
[DEBUG @ 23:50:06 | PartyExtensions]   "timestamp": 637797882069234481,
[DEBUG @ 23:50:06 | PartyExtensions]   "playername": ""
[DEBUG @ 23:50:06 | PartyExtensions] }
[DEBUG @ 23:50:06 | PartyExtensions] stuff: SS0950 
*/

/*
[DEBUG @ 01:51:30 | PartyExtensions] {
[DEBUG @ 01:51:30 | PartyExtensions]   "custom_score_list": [
[DEBUG @ 01:51:30 | PartyExtensions]     {
[DEBUG @ 01:51:30 | PartyExtensions]       "rank": "SS",
[DEBUG @ 01:51:30 | PartyExtensions]       "missed": 0,
[DEBUG @ 01:51:30 | PartyExtensions]       "good_cuts": 744,
[DEBUG @ 01:51:30 | PartyExtensions]       "bad_cuts": 0,
[DEBUG @ 01:51:30 | PartyExtensions]       "left_acc": 114.25338,
[DEBUG @ 01:51:30 | PartyExtensions]       "right_acc": 114.629463,
[DEBUG @ 01:51:30 | PartyExtensions]       "modifiers": {
[DEBUG @ 01:51:30 | PartyExtensions]         "energyType": 0,
[DEBUG @ 01:51:30 | PartyExtensions]         "noFailOn0Energy": true,
[DEBUG @ 01:51:30 | PartyExtensions]         "demoNoFail": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "instaFail": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "failOnSaberClash": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "enabledObstacleType": 0,
[DEBUG @ 01:51:30 | PartyExtensions]         "demoNoObstacles": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "fastNotes": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "strictAngles": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "disappearingArrows": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "ghostNotes": true,
[DEBUG @ 01:51:30 | PartyExtensions]         "noBombs": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "songSpeed": 3,
[DEBUG @ 01:51:30 | PartyExtensions]         "noArrows": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "proMode": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "zenMode": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "smallCubes": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "songSpeedMul": 1.5,
[DEBUG @ 01:51:30 | PartyExtensions]         "cutAngleTolerance": 60.0,
[DEBUG @ 01:51:30 | PartyExtensions]         "notesUniformScale": 1.0
[DEBUG @ 01:51:30 | PartyExtensions]       },
[DEBUG @ 01:51:30 | PartyExtensions]       "longest_combo": 744,
[DEBUG @ 01:51:30 | PartyExtensions]       "timestamp": 637797932373460591,
[DEBUG @ 01:51:30 | PartyExtensions]       "playername": ""
[DEBUG @ 01:51:30 | PartyExtensions]     },
[DEBUG @ 01:51:30 | PartyExtensions]     {
[DEBUG @ 01:51:30 | PartyExtensions]       "rank": "B",
[DEBUG @ 01:51:30 | PartyExtensions]       "missed": 40,
[DEBUG @ 01:51:30 | PartyExtensions]       "good_cuts": 2415,
[DEBUG @ 01:51:30 | PartyExtensions]       "bad_cuts": 2,
[DEBUG @ 01:51:30 | PartyExtensions]       "left_acc": 111.831055,
[DEBUG @ 01:51:30 | PartyExtensions]       "right_acc": 111.971245,
[DEBUG @ 01:51:30 | PartyExtensions]       "modifiers": {
[DEBUG @ 01:51:30 | PartyExtensions]         "energyType": 0,
[DEBUG @ 01:51:30 | PartyExtensions]         "noFailOn0Energy": true,
[DEBUG @ 01:51:30 | PartyExtensions]         "demoNoFail": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "instaFail": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "failOnSaberClash": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "enabledObstacleType": 0,
[DEBUG @ 01:51:30 | PartyExtensions]         "demoNoObstacles": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "fastNotes": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "strictAngles": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "disappearingArrows": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "ghostNotes": true,
[DEBUG @ 01:51:30 | PartyExtensions]         "noBombs": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "songSpeed": 3,
[DEBUG @ 01:51:30 | PartyExtensions]         "noArrows": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "proMode": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "zenMode": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "smallCubes": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "songSpeedMul": 1.5,
[DEBUG @ 01:51:30 | PartyExtensions]         "cutAngleTolerance": 60.0,
[DEBUG @ 01:51:30 | PartyExtensions]         "notesUniformScale": 1.0
[DEBUG @ 01:51:30 | PartyExtensions]       },
[DEBUG @ 01:51:30 | PartyExtensions]       "longest_combo": 739,
[DEBUG @ 01:51:30 | PartyExtensions]       "timestamp": 637797934807592494,
[DEBUG @ 01:51:30 | PartyExtensions]       "playername": ""
[DEBUG @ 01:51:30 | PartyExtensions]     },
[DEBUG @ 01:51:30 | PartyExtensions]     {
[DEBUG @ 01:51:30 | PartyExtensions]       "rank": "E",
[DEBUG @ 01:51:30 | PartyExtensions]       "missed": 465,
[DEBUG @ 01:51:30 | PartyExtensions]       "good_cuts": 0,
[DEBUG @ 01:51:30 | PartyExtensions]       "bad_cuts": 0,
[DEBUG @ 01:51:30 | PartyExtensions]       "left_acc": "NaN",
[DEBUG @ 01:51:30 | PartyExtensions]       "right_acc": "NaN",
[DEBUG @ 01:51:30 | PartyExtensions]       "modifiers": {
[DEBUG @ 01:51:30 | PartyExtensions]         "energyType": 0,
[DEBUG @ 01:51:30 | PartyExtensions]         "noFailOn0Energy": true,
[DEBUG @ 01:51:30 | PartyExtensions]         "demoNoFail": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "instaFail": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "failOnSaberClash": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "enabledObstacleType": 0,
[DEBUG @ 01:51:30 | PartyExtensions]         "demoNoObstacles": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "fastNotes": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "strictAngles": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "disappearingArrows": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "ghostNotes": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "noBombs": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "songSpeed": 0,
[DEBUG @ 01:51:30 | PartyExtensions]         "noArrows": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "proMode": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "zenMode": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "smallCubes": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "songSpeedMul": 1.0,
[DEBUG @ 01:51:30 | PartyExtensions]         "cutAngleTolerance": 60.0,
[DEBUG @ 01:51:30 | PartyExtensions]         "notesUniformScale": 1.0
[DEBUG @ 01:51:30 | PartyExtensions]       },
[DEBUG @ 01:51:30 | PartyExtensions]       "longest_combo": 0,
[DEBUG @ 01:51:30 | PartyExtensions]       "timestamp": 637797939780572055,
[DEBUG @ 01:51:30 | PartyExtensions]       "playername": ""
[DEBUG @ 01:51:30 | PartyExtensions]     },
[DEBUG @ 01:51:30 | PartyExtensions]     {
[DEBUG @ 01:51:30 | PartyExtensions]       "rank": "E",
[DEBUG @ 01:51:30 | PartyExtensions]       "missed": 892,
[DEBUG @ 01:51:30 | PartyExtensions]       "good_cuts": 0,
[DEBUG @ 01:51:30 | PartyExtensions]       "bad_cuts": 0,
[DEBUG @ 01:51:30 | PartyExtensions]       "left_acc": "NaN",
[DEBUG @ 01:51:30 | PartyExtensions]       "right_acc": "NaN",
[DEBUG @ 01:51:30 | PartyExtensions]       "modifiers": {
[DEBUG @ 01:51:30 | PartyExtensions]         "energyType": 0,
[DEBUG @ 01:51:30 | PartyExtensions]         "noFailOn0Energy": true,
[DEBUG @ 01:51:30 | PartyExtensions]         "demoNoFail": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "instaFail": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "failOnSaberClash": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "enabledObstacleType": 0,
[DEBUG @ 01:51:30 | PartyExtensions]         "demoNoObstacles": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "fastNotes": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "strictAngles": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "disappearingArrows": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "ghostNotes": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "noBombs": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "songSpeed": 0,
[DEBUG @ 01:51:30 | PartyExtensions]         "noArrows": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "proMode": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "zenMode": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "smallCubes": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "songSpeedMul": 1.0,
[DEBUG @ 01:51:30 | PartyExtensions]         "cutAngleTolerance": 60.0,
[DEBUG @ 01:51:30 | PartyExtensions]         "notesUniformScale": 1.0
[DEBUG @ 01:51:30 | PartyExtensions]       },
[DEBUG @ 01:51:30 | PartyExtensions]       "longest_combo": 0,
[DEBUG @ 01:51:30 | PartyExtensions]       "timestamp": 637797944485704322,
[DEBUG @ 01:51:30 | PartyExtensions]       "playername": ""
[DEBUG @ 01:51:30 | PartyExtensions]     },
[DEBUG @ 01:51:30 | PartyExtensions]     {
[DEBUG @ 01:51:30 | PartyExtensions]       "rank": "SS",
[DEBUG @ 01:51:30 | PartyExtensions]       "missed": 3,
[DEBUG @ 01:51:30 | PartyExtensions]       "good_cuts": 489,
[DEBUG @ 01:51:30 | PartyExtensions]       "bad_cuts": 0,
[DEBUG @ 01:51:30 | PartyExtensions]       "left_acc": 114.74884,
[DEBUG @ 01:51:30 | PartyExtensions]       "right_acc": 114.890511,
[DEBUG @ 01:51:30 | PartyExtensions]       "modifiers": {
[DEBUG @ 01:51:30 | PartyExtensions]         "energyType": 0,
[DEBUG @ 01:51:30 | PartyExtensions]         "noFailOn0Energy": true,
[DEBUG @ 01:51:30 | PartyExtensions]         "demoNoFail": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "instaFail": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "failOnSaberClash": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "enabledObstacleType": 0,
[DEBUG @ 01:51:30 | PartyExtensions]         "demoNoObstacles": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "fastNotes": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "strictAngles": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "disappearingArrows": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "ghostNotes": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "noBombs": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "songSpeed": 0,
[DEBUG @ 01:51:30 | PartyExtensions]         "noArrows": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "proMode": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "zenMode": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "smallCubes": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "songSpeedMul": 1.0,
[DEBUG @ 01:51:30 | PartyExtensions]         "cutAngleTolerance": 60.0,
[DEBUG @ 01:51:30 | PartyExtensions]         "notesUniformScale": 1.0
[DEBUG @ 01:51:30 | PartyExtensions]       },
[DEBUG @ 01:51:30 | PartyExtensions]       "longest_combo": 216,
[DEBUG @ 01:51:30 | PartyExtensions]       "timestamp": 637797947818548569,
[DEBUG @ 01:51:30 | PartyExtensions]       "playername": ""
[DEBUG @ 01:51:30 | PartyExtensions]     },
[DEBUG @ 01:51:30 | PartyExtensions]     {
[DEBUG @ 01:51:30 | PartyExtensions]       "rank": "SSS",
[DEBUG @ 01:51:30 | PartyExtensions]       "missed": 0,
[DEBUG @ 01:51:30 | PartyExtensions]       "good_cuts": 580,
[DEBUG @ 01:51:30 | PartyExtensions]       "bad_cuts": 0,
[DEBUG @ 01:51:30 | PartyExtensions]       "left_acc": 115.0,
[DEBUG @ 01:51:30 | PartyExtensions]       "right_acc": 115.0,
[DEBUG @ 01:51:30 | PartyExtensions]       "modifiers": {
[DEBUG @ 01:51:30 | PartyExtensions]         "energyType": 0,
[DEBUG @ 01:51:30 | PartyExtensions]         "noFailOn0Energy": true,
[DEBUG @ 01:51:30 | PartyExtensions]         "demoNoFail": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "instaFail": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "failOnSaberClash": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "enabledObstacleType": 0,
[DEBUG @ 01:51:30 | PartyExtensions]         "demoNoObstacles": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "fastNotes": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "strictAngles": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "disappearingArrows": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "ghostNotes": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "noBombs": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "songSpeed": 0,
[DEBUG @ 01:51:30 | PartyExtensions]         "noArrows": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "proMode": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "zenMode": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "smallCubes": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "songSpeedMul": 1.0,
[DEBUG @ 01:51:30 | PartyExtensions]         "cutAngleTolerance": 60.0,
[DEBUG @ 01:51:30 | PartyExtensions]         "notesUniformScale": 1.0
[DEBUG @ 01:51:30 | PartyExtensions]       },
[DEBUG @ 01:51:30 | PartyExtensions]       "longest_combo": 580,
[DEBUG @ 01:51:30 | PartyExtensions]       "timestamp": 637797951485156329,
[DEBUG @ 01:51:30 | PartyExtensions]       "playername": ""
[DEBUG @ 01:51:30 | PartyExtensions]     },
[DEBUG @ 01:51:30 | PartyExtensions]     {
[DEBUG @ 01:51:30 | PartyExtensions]       "rank": "E",
[DEBUG @ 01:51:30 | PartyExtensions]       "missed": 1394,
[DEBUG @ 01:51:30 | PartyExtensions]       "good_cuts": 0,
[DEBUG @ 01:51:30 | PartyExtensions]       "bad_cuts": 0,
[DEBUG @ 01:51:30 | PartyExtensions]       "left_acc": "NaN",
[DEBUG @ 01:51:30 | PartyExtensions]       "right_acc": "NaN",
[DEBUG @ 01:51:30 | PartyExtensions]       "modifiers": {
[DEBUG @ 01:51:30 | PartyExtensions]         "energyType": 0,
[DEBUG @ 01:51:30 | PartyExtensions]         "noFailOn0Energy": true,
[DEBUG @ 01:51:30 | PartyExtensions]         "demoNoFail": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "instaFail": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "failOnSaberClash": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "enabledObstacleType": 0,
[DEBUG @ 01:51:30 | PartyExtensions]         "demoNoObstacles": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "fastNotes": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "strictAngles": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "disappearingArrows": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "ghostNotes": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "noBombs": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "songSpeed": 0,
[DEBUG @ 01:51:30 | PartyExtensions]         "noArrows": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "proMode": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "zenMode": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "smallCubes": false,
[DEBUG @ 01:51:30 | PartyExtensions]         "songSpeedMul": 1.0,
[DEBUG @ 01:51:30 | PartyExtensions]         "cutAngleTolerance": 60.0,
[DEBUG @ 01:51:30 | PartyExtensions]         "notesUniformScale": 1.0
[DEBUG @ 01:51:30 | PartyExtensions]       },
[DEBUG @ 01:51:30 | PartyExtensions]       "longest_combo": 0,
[DEBUG @ 01:51:30 | PartyExtensions]       "timestamp": 637797954900511620,
[DEBUG @ 01:51:30 | PartyExtensions]       "playername": ""
[DEBUG @ 01:51:30 | PartyExtensions]     }
[DEBUG @ 01:51:30 | PartyExtensions]   ]
[DEBUG @ 01:51:30 | PartyExtensions] }
*/