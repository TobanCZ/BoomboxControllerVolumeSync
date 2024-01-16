using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace BoomboxControllerVolumeSync
{
    [BepInPlugin(modGUID,modName,modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "TobanCZ.BoomboxControllerVolumeSync";
        private const string modName = "Boombox Controller Volume Sync";
        private const string modVersion = "1.0.0";

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

            NetcodeWeaver();

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("The Boombox Controller Volume Sync has been loaded.");

            harmony.PatchAll(typeof(Plugin));
            harmony.PatchAll(typeof(BoomboxPatch));
            harmony.PatchAll(typeof(GameNetworkManagerPatch));
        }

        private static void NetcodeWeaver()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }
    }
}
