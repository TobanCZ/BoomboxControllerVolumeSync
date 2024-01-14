using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

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
