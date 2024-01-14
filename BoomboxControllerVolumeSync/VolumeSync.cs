using System;
using System.IO;
using Unity.Netcode;
using Newtonsoft.Json;

namespace BoomboxControllerVolumeSync
{
    internal class VolumeSync : NetworkBehaviour
    {
        private float syncedVolume = 1f;
        internal static VolumeSync Instance { get; private set; }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if ((NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer) && Instance != null)
                Instance.gameObject.GetComponent<NetworkObject>().Despawn();
            Instance = this;

            NetworkManager.Singleton.OnClientConnectedCallback += onPlayerJoin;
        }

        private void onPlayerJoin(ulong obj)
        {
            if(IsServer)
                ActivateVolumeSyncClientRpc(syncedVolume, obj);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ActivateVolumeSyncServerRPC(float newVolume, ServerRpcParams serverRpcParams = default)
        {
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClients.Keys)
            {
                if (clientId != serverRpcParams.Receive.SenderClientId)
                {
                    ActivateVolumeSyncClientRpc(newVolume, clientId);
                }
            }
            Plugin.mls.LogInfo("ActivateVolumeSyncServerRPC: Activating Volume sync. newVolume: " + newVolume);
        }

        [ClientRpc]
        public void ActivateVolumeSyncClientRpc(float newVolume, ulong targetClientId)
        {
            if (IsClient && NetworkManager.Singleton.LocalClientId == targetClientId)
            {
                syncedVolume = newVolume;
                updateCache();
                Plugin.mls.LogInfo("ActivateVolumeSyncClientRpc: Activating Volume sync locally. newVolume: " + newVolume);
            }
        }

        private void updateCache()
        {
            var cache = LoadCache();
            
            if(cache != null)
            {
                SaveCache(syncedVolume, cache.UpButton, cache.DownButton);
            }
            else
            {
                SaveCache(syncedVolume, null, null);
            }
        }

        private void SaveCache(float vol, string up, string down)
        {
            BoomboxController.BoomboxController.Cache cache = new BoomboxController.BoomboxController.Cache();
            cache.Volume = vol;
            cache.UpButton = up;
            cache.DownButton = down;
            string json = JsonConvert.SerializeObject(cache);
            using (StreamWriter sw = new StreamWriter(@"BoomboxController\cache"))
            {
                sw.WriteLine(json);
            }
        }

        private BoomboxController.BoomboxController.Cache LoadCache()
        {
            string json = String.Empty;
            if (File.Exists(@"BoomboxController\cache"))
            {
                using (StreamReader sr = new StreamReader(@"BoomboxController\cache"))
                {
                    json = sr.ReadToEnd();
                }
                return JsonConvert.DeserializeObject<BoomboxController.BoomboxController.Cache>(json);
            }
            return null;
        }
    }
}
