using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Reflection;

namespace BoomboxControllerVolumeSync
{
    internal class GameNetworkManagerPatch
    {

        private static GameObject networkPrefab = null;

        [HarmonyPatch(typeof(GameNetworkManager), "Start")]
        [HarmonyPostfix]
        private static void Init()
        {
            if (networkPrefab != null)
                return;

            AssetBundle data = AssetBundle.LoadFromMemory(Properties.Resources.boomboxcontrollervolumesync);
            networkPrefab = data.LoadAsset<GameObject>("Assets/BoomboxVolumeSyncHandler.prefab");
            networkPrefab.AddComponent<VolumeSync>();

            NetworkManager.Singleton.AddNetworkPrefab(networkPrefab);
        }


        [HarmonyPatch(typeof(StartOfRound), "Awake")]
        [HarmonyPostfix]
        private static void SpawnNetworkHandler()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                var networkHandlerHost = Object.Instantiate(networkPrefab, Vector3.zero, Quaternion.identity);
                networkHandlerHost.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}
