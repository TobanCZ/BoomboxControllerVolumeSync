﻿using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BoomboxControllerVolumeSync
{
    [BepInPlugin(modGUID,modName,modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "TobanCZ.BoomboxControllerVolumeSync";
        private const string modName = "Boombox Controller Volume Sync";
        private const string modVersion = "0.1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static Plugin Instance;

        public static  ManualLogSource mls;
        public static GameObject volumeSync;

        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            volumeSync = new GameObject();
            volumeSync.AddComponent<VolumeSync>();

            mls.LogInfo("The Boombox Controller Volume Sync has been loaded.");

            harmony.PatchAll(typeof(Plugin));
            harmony.PatchAll(typeof(BoomboxPatch));
            harmony.PatchAll(typeof(GameNetworkManagerPatch));
        }
    }
}