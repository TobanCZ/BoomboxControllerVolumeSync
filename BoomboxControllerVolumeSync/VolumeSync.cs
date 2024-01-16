using System;
using System.IO;
using Unity.Netcode;
using Newtonsoft.Json;

namespace BoomboxControllerVolumeSync
{
    internal class VolumeSync : NetworkBehaviour
    {
        private float syncedVolume = 0.5f;
        internal static VolumeSync Instance { get; private set; }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if ((NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer) && Instance != null)
            {
                Instance.gameObject.GetComponent<NetworkObject>().Despawn();
            }

            Instance = this;

            NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerJoin;
        }

        private void OnPlayerJoin(ulong playerId)
        {
            if (IsServer)
            {
                ActivateVolumeSyncOnJoinClientRpc(playerId, syncedVolume);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void ActivateVolumeSyncServerRPC(float newVolume, ServerRpcParams serverRpcParams = default)
        {
            ActivateVolumeSyncClientRpc(newVolume);
        }

        [ClientRpc]
        public void ActivateVolumeSyncClientRpc(float newVolume)
        {
            syncedVolume = newVolume;
            BoomboxController.BoomboxController.boomboxItem.boomboxAudio.volume = syncedVolume;
        }

        [ClientRpc]
        public void ActivateVolumeSyncOnJoinClientRpc(ulong targetPlayerId, float newVolume)
        {
            if (IsClient && NetworkManager.Singleton.LocalClientId == targetPlayerId)
            {
                syncedVolume = newVolume;
                BoomboxController.BoomboxController.boomboxItem.boomboxAudio.volume = syncedVolume;
            }
        }
    }
}
