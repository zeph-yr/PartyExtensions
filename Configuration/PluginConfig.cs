﻿/*
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace PartyExtensions.Configuration
{
    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }

        //[UseConverter(typeof(CustomLeaderboardCollection))]
        //[NonNullable]
        //public virtual CustomLeaderboardCollection party_data { get; set; } = new CustomLeaderboardCollection();

        public virtual CustomScoreData map_score { get; set; } = new CustomScoreData();

        public virtual CustomLeaderboard map_leaderboard { get; set; } = new CustomLeaderboard();

        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        public virtual void OnReload()
        {
            // Do stuff after config is read from disk.
        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
            // Do stuff when the config is changed.
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            // This instance's members populated from other
        }
    }
}*/