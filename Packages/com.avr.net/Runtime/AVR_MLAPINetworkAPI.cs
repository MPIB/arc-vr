using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MLAPI;

using AVR.Core;

namespace AVR.Net
{
    public class MLAPINetworkAPI : AVR_Component.ComponentNetworkAPI {
        public override int instanceId(AVR_Component comp) {
            if(comp.networkObject==null) comp.networkObject = comp.GetComponentInParent<NetworkObject>();
            return ((NetworkObject)comp.networkObject).GetInstanceID();
        }
        public override ulong networkId(AVR_Component comp) {
            if(comp.networkObject==null) comp.networkObject = comp.GetComponentInParent<NetworkObject>();
            return ((NetworkObject)comp.networkObject).NetworkInstanceId;
        }
        public override ulong ownerId(AVR_Component comp) {
            if(comp.networkObject==null) comp.networkObject = comp.GetComponentInParent<NetworkObject>();
            return ((NetworkObject)comp.networkObject).OwnerClientId;
        }
        public override bool isSpawned(AVR_Component comp) {
            if(comp.networkObject==null) comp.networkObject = comp.GetComponentInParent<NetworkObject>();
            return ((NetworkObject)comp.networkObject).IsSpawned;
        }
        public override bool isLocalPlayer(AVR_Component comp) {
            if(comp.networkObject==null) comp.networkObject = comp.GetComponentInParent<NetworkObject>();
            return ((NetworkObject)comp.networkObject).IsLocalPlayer;
        }
        public override bool isOwner(AVR_Component comp) {
            if(comp.networkObject==null) comp.networkObject = comp.GetComponentInParent<NetworkObject>();
            return ((NetworkObject)comp.networkObject).IsOwner;
        }
        public override bool isOwnedByServer(AVR_Component comp) {
            if(comp.networkObject==null) comp.networkObject = comp.GetComponentInParent<NetworkObject>();
            return ((NetworkObject)comp.networkObject).IsOwnedByServer;
        }
        public override bool isPlayerObject(AVR_Component comp) {
            if(comp.networkObject==null) comp.networkObject = comp.GetComponentInParent<NetworkObject>();
            return ((NetworkObject)comp.networkObject).IsPlayerObject;
        }
        public override bool? isSceneObject(AVR_Component comp) {
            if(comp.networkObject==null) comp.networkObject = comp.GetComponentInParent<NetworkObject>();
            return ((NetworkObject)comp.networkObject).IsSceneObject;
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
