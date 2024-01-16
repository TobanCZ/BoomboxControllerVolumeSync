using HarmonyLib;

namespace BoomboxControllerVolumeSync
{
    internal class BoomboxPatch
    {
       
        [HarmonyPatch(typeof(BoomboxController.BoomboxController), nameof(BoomboxController.BoomboxController.SaveCache))]
        [HarmonyPostfix]
        private static void SaveCachePostFix(float vol)
        {
            VolumeSync.Instance.ActivateVolumeSyncServerRPC(vol);
        }
    }
}
