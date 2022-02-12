using IPA;
using IPA.Config;
using System;
using System.Reflection;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace PartyExtensions
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
         public const string HarmonyId = "com.github.YourGitHub.PartyExtensions";
         internal static readonly HarmonyLib.Harmony harmony = new HarmonyLib.Harmony(HarmonyId);

        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        internal static PartyExtensionsController PluginController { get { return PartyExtensionsController.Instance; } }

        internal static ButtonController buttons;



        [Init]
        public Plugin(IPALogger logger /*Config conf*/)
        {
            Instance = this;
            Plugin.Log = logger;
            Plugin.Log?.Debug("Logger initialized.");

            //Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            //Plugin.Log?.Debug("Config loaded");
        }


        [OnEnable]
        public void OnEnable()
        {
            PartyData.Read();

            new GameObject("PartyExtensionsController").AddComponent<PartyExtensionsController>();
            ApplyHarmonyPatches();


        }


        [OnDisable]
        public void OnDisable()
        {
            if (PluginController != null)
                GameObject.Destroy(PluginController);
            RemoveHarmonyPatches();
        }


        /*
        /// <summary>
        /// Called when the plugin is disabled and on Beat Saber quit.
        /// Return Task for when the plugin needs to do some long-running, asynchronous work to disable.
        /// [OnDisable] methods that return Task are called after all [OnDisable] methods that return void.
        /// </summary>
        [OnDisable]
        public async Task OnDisableAsync()
        {
            await LongRunningUnloadTask().ConfigureAwait(false);
        }
        */
        

        internal static void ApplyHarmonyPatches()
        {
            try
            {
                Plugin.Log?.Debug("Applying Harmony patches.");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error("Error applying Harmony patches: " + ex.Message);
                Plugin.Log?.Debug(ex);
            }
        }


        internal static void RemoveHarmonyPatches()
        {
            try
            {
                harmony.UnpatchSelf();
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error("Error removing Harmony patches: " + ex.Message);
                Plugin.Log?.Debug(ex);
            }
        }
    }
}
