using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PartyExtensions
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    public class PartyExtensionsController : MonoBehaviour
    {
        public static PartyExtensionsController Instance { get; private set; }

        internal static float left_acc = 0;
        internal static float right_acc;

        internal static int left_hits;
        internal static int right_hits;


        internal static float final_left_acc;
        internal static float final_right_acc;

        internal CustomScoreData customScore;


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
        /// Called every frame if the script is enabled.
        /// </summary>
        private void Update()
        {

        }

        /// <summary>
        /// Called every frame after every other enabled script's Update().
        /// </summary>
        private void LateUpdate()
        {

        }

        /// <summary>
        /// Called when the script becomes enabled and active
        /// </summary>
        private void OnEnable()
        {
            BS_Utils.Utilities.BSEvents.gameSceneLoaded += BSEvents_gameSceneLoaded;

            BS_Utils.Utilities.BSEvents.noteWasCut += this.BSEvents_noteWasCut;
            BS_Utils.Utilities.BSEvents.levelCleared += BSEvents_levelCleared;

        }

        private void BSEvents_levelCleared(StandardLevelScenesTransitionSetupDataSO arg1, LevelCompletionResults arg2)
        {
            //final_left_acc = left_acc / left_hits;
            //final_right_acc = right_acc / right_hits;

            //Plugin.Log.Debug($"final: {final_left_acc} {final_right_acc}");
            Plugin.Log.Debug("level finished");


            customScore = new CustomScoreData(arg2.rank.ToString(), arg2.missedCount, arg2.goodCutsCount, arg2.badCutsCount, /*final_left_acc, final_right_acc,*/ arg2.gameplayModifiers, arg2.maxCombo, DateTime.Now.Ticks, "");

            Plugin.Log.Debug("level finished");
            Plugin.Log.Debug(JsonConvert.SerializeObject(customScore));
        }

        private void BSEvents_gameSceneLoaded()
        {
            final_left_acc = 0;
            final_right_acc = 0;

            left_acc = 0;
            left_hits = 0;

            right_acc = 0;
            right_hits = 0;

            

        }

        private void BSEvents_noteWasCut(NoteData arg1, NoteCutInfo arg2, int arg3)
        {
            int preswing;
            int postswing;
            int center;

            ScoreModel.RawScoreWithoutMultiplier(arg2.swingRatingCounter, arg2.cutDistanceToCenter, out preswing, out postswing, out center);

            if (arg1 != null && arg1.colorType == ColorType.ColorA)
            {
                left_acc += preswing + postswing + center;
                left_hits++;

                Plugin.Log.Debug($"left: {preswing} {postswing} {center} {left_hits}");
            }

            else if (arg1 != null && arg1.colorType == ColorType.ColorB)
            {
                right_acc += preswing + postswing + center;
                right_hits++;

                Plugin.Log.Debug($"right: {preswing} {postswing} {center} {right_hits}");
            }


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
