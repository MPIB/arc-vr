using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Netcode;

using AVR.Core;

namespace AVR.Net
{
    public class MLAPINetworkAPI : AVR_Component.ComponentNetworkAPI {
        protected NetworkObject GetNetworkObject(AVR_Component comp){
            if (comp.networkObject == null) comp.networkObject = comp.GetComponentInParent<NetworkObject>();
            return ((NetworkObject)comp.networkObject);
        }

        public override int instanceId(AVR_Component comp) {
            return GetNetworkObject(comp).GetInstanceID();
        }
        public override ulong networkId(AVR_Component comp) {
            return GetNetworkObject(comp).NetworkObjectId;
        }
        public override ulong ownerId(AVR_Component comp) {
            return GetNetworkObject(comp).OwnerClientId;
        }
        public override bool isSpawned(AVR_Component comp) {
            return GetNetworkObject(comp).IsSpawned;
        }
        public override bool isLocalPlayer(AVR_Component comp) {
            return GetNetworkObject(comp).IsLocalPlayer;
        }
        public override bool isOwner(AVR_Component comp) {
            return GetNetworkObject(comp).IsOwner;
        }
        public override bool isOwnedByServer(AVR_Component comp) {
            return GetNetworkObject(comp).IsOwnedByServer;
        }
        public override bool isPlayerObject(AVR_Component comp) {
            return GetNetworkObject(comp).IsPlayerObject;
        }
        public override bool? isSceneObject(AVR_Component comp) {
            return GetNetworkObject(comp).IsSceneObject;
        }
        public override bool isOnline(){
            return NetworkManager.Singleton? NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer : false;
        }

        public override void setOwner(AVR_Component comp, ulong newOwnerId) {
            setOwnership(GetNetworkObject(comp), newOwnerId);
        }

        public override void removeOwner(AVR_Component comp) {
            removeOwnership(GetNetworkObject(comp));
        }

        [ServerRpc]
        protected void setOwnership(NetworkObject networkObject, ulong newOwnerId) {
            networkObject.ChangeOwnership(newOwnerId);
        }

        [ServerRpc]
        protected void removeOwnership(NetworkObject networkObject) {
            networkObject.RemoveOwnership();
        }
    }

    [ExecuteInEditMode]
    public class AVR_AutoReplaceAPI
    {
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod]
        static void InitCommands()
        {
            AVR_Component.networkAPI = new MLAPINetworkAPI();
        }
    }
}
