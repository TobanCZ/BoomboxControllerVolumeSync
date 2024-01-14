using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace BoomboxControllerVolumeSync
{
    internal class GameNetworkManagerPatch
    {

        private static GameObject _networkPrefab = null;

        [HarmonyPatch(typeof(GameNetworkManager), "Start")]
        [HarmonyPostfix]
        private static void Init()
        {
            if (_networkPrefab != null)
                return;

            var disabledPrefab = new GameObject("VolumeSyncContainer") { hideFlags = HideFlags.HideAndDontSave };
            disabledPrefab.SetActive(false);

            var prefab = new GameObject("VolumeSyncHandler");
            prefab.transform.SetParent(disabledPrefab.transform);
            prefab.AddComponent<NetworkObject>();
            prefab.AddComponent<VolumeSync>();
            prefab.hideFlags = HideFlags.HideAndDontSave;
            NetworkManager.Singleton.AddNetworkPrefab(prefab);
            _networkPrefab = prefab;
        }

        [HarmonyPatch(typeof(StartOfRound), "Awake")]
        [HarmonyPostfix]
        private static void SpawnNetworkHandler()
        {
            if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer) return;

            var networkHandlerHost = UnityEngine.Object.Instantiate(_networkPrefab, Vector3.zero, Quaternion.identity, StartOfRound.Instance.transform);
            networkHandlerHost.GetComponent<NetworkObject>().Spawn();

            Plugin.mls.LogInfo("VolumeSyncHandler has been created.");
        }
    }
}
