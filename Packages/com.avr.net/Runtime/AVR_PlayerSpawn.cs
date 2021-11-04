using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using AVR.Core;
using System.Linq;

namespace AVR.Net {
    /// <summary>
    /// Used for convenient spawning of players with respective network prefabs.
    /// </summary>
    public class AVR_PlayerSpawn : AVR_NetworkBehaviour
    {
        public int max_players = 4;

        public List<Transform> spawnLocations = new List<Transform>();

        public bool disconnectClientIfFailed = true;

        public string prefabHashGenerator {
            get { return _prefabHashGenerator; }
            set { 
                AVR_Settings.set("/net/playerPrefabHashGenerator", value);
                _prefabHashGenerator = value;
            }
        }

        [SerializeField]
        private string _prefabHashGenerator = "NetworkedPlayerRig";

        public override void NetworkStart()
        {
            base.NetworkStart();
            AVR.Core.AVR_DevConsole.print("Requesting playerSpawn from server... (HashGenerator="+prefabHashGenerator+")");
            spawnServerRpc(NetworkManager.Singleton.LocalClientId, prefabHashGenerator);
        }

        void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            foreach(var loc in spawnLocations) {
                if(loc==null) continue;
                Gizmos.DrawSphere(loc.position, 0.1f);
                Gizmos.DrawLine(loc.position, loc.position + loc.forward);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        protected virtual void spawnServerRpc(ulong clientId, string playerPrefabHashGenerator)
        {
            AVR_DevConsole.cprint("Client #"+clientId+" requested spawn with playerPrefabHashGenerator "+playerPrefabHashGenerator, this);

            if(NetworkManager.ConnectedClientsList.Count + 1 > max_players) {
                AVR_DevConsole.cerror("Connected client would exceed the maximum number of allowed players ("+max_players+"). Disconnecting client.", this);
                NetworkManager.DisconnectClient(clientId);
                return;
            }

            Transform spawnLocation = transform;
            if(spawnLocations.Count>0) {
                spawnLocation = spawnLocations[(int)clientId % spawnLocations.Count];
            }

            try {
                // If the client already has a playerobject, destroy it before creating a new one.
                if (MLAPI.Spawning.NetworkSpawnManager.GetPlayerNetworkObject(clientId) != null)
                {
                    AVR_DevConsole.cprint("Client #"+clientId+" already has a playerNetworkObject! Despawning.", this);
                    MLAPI.Spawning.NetworkSpawnManager.GetPlayerNetworkObject(clientId).Despawn(destroy: true);
                }

                // Retrieve the prefab with the respective prefabHashGenerator
                ulong hash = MLAPI.Spawning.NetworkSpawnManager.GetPrefabHashFromGenerator(playerPrefabHashGenerator);
                int index = MLAPI.Spawning.NetworkSpawnManager.GetNetworkPrefabIndexOfHash(hash);
                if(index < 0) {
                    AVR_DevConsole.cerror("PlayerSpawn failed: There is no playerPrefab with hashGenerator " + playerPrefabHashGenerator + "!", this);
                    throw new System.IndexOutOfRangeException();
                }
                GameObject prefab = NetworkManager.Singleton.NetworkConfig.NetworkPrefabs[index].Prefab;

                // Instantiate and spawn the object
                AVR_DevConsole.cprint("Instantiating playerPrefab as playerObject for client #"+clientId, this);
                GameObject obj = Instantiate(prefab, spawnLocation.position, spawnLocation.rotation);
                obj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            }
            // Deal with errors
            catch(System.Exception e) {
                AVR_DevConsole.cerror("PlayerSpawn failed with following exception: "+e.Message, this);
                if(disconnectClientIfFailed) {
                    AVR_DevConsole.cerror("Disconnecting client due to failed spawn.", this);
                    NetworkManager.DisconnectClient(clientId);
                }
            }
        }
    }
}
