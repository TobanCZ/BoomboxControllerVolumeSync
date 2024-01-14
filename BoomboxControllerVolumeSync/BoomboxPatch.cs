using HarmonyLib;

namespace BoomboxControllerVolumeSync
{
    internal class BoomboxPatch
    {
       
        [HarmonyPatch(typeof(BoomboxController.BoomboxController), nameof(BoomboxController.BoomboxController.SaveCache))]
        [HarmonyPostfix]
        private static void SaveCachePostFix(float vol, string up, string down)
        {
            VolumeSync.Instance.ActivateVolumeSyncServerRPC(vol);
        }
    }
}
