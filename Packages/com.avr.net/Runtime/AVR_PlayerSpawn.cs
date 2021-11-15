using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using AVR.Core;
using System.Linq;

namespace AVR.Net {
    /// <summary>
    /// Used for convenient spawning of players with respective network prefabs.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_net_1_1_a_v_r___player_spawn.html")]
    public class AVR_PlayerSpawn : AVR_Behaviour
    {
        public int max_players = 4;

        public List<Transform> spawnLocations = new List<Transform>();

        public bool disconnectClientIfFailed = true;

        public NetworkObject prefab;

        protected virtual NetworkObject getPrefab() => prefab;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            spawnServerRpc(NetworkManager.Singleton.LocalClientId);
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
        protected virtual void spawnServerRpc(ulong clientId)
        {
            AVR_DevConsole.cprint("Client #"+clientId+" requested spawn", this);

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
                if (NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId) != null)
                {
                    AVR_DevConsole.cprint("Client #"+clientId+" already has a playerNetworkObject! Despawning.", this);
                    NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId).Despawn(destroy: true);
                }

                // Instantiate and spawn the object
                AVR_DevConsole.cprint("Instantiating playerPrefab as playerObject for client #"+clientId, this);
                GameObject obj = Instantiate(getPrefab().gameObject, spawnLocation.position, spawnLocation.rotation);
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
