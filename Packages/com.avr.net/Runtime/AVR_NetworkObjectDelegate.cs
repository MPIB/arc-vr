using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

using AVR.Core;

namespace AVR.Net {
    public class AVR_NetworkObjectDelegate : NetworkBehaviour
    {
        public List<Component> removeOnRemote = new List<Component>(); 
        public bool removeCamerasOnRemote = true;
        public bool removeXRControllersOnRemote = true;
        public bool removeTrackedPoseDriversOnRemote = true;
        public bool removeCharacterControllersOnRemote = true;
        public bool removeAudoListenersOnRemote = true;

        public override void NetworkStart() {
            if(!this.IsLocalPlayer) {
                foreach(AVR_Component c in GetComponentsInChildren<AVR_Component>())
                {
                    c.OnRemote();
                }
                foreach (Component c in removeOnRemote)
                {
                    GameObject.Destroy(c);
                }
                removeOnRemote.Clear();
                if(removeCamerasOnRemote) {
                    foreach (Camera c in GetComponentsInChildren<Camera>())
                    {
                        //GameObject.Destroy(c);
                        c.enabled = false; //TODO: We're not destroying it cause it breaks AVR_PlayerRig, which is in turn used by PoseAssumer. FIX
                    }
                }
                if (removeCharacterControllersOnRemote)
                {
                    foreach (CharacterController c in GetComponentsInChildren<CharacterController>())
                    {
                        GameObject.Destroy(c);
                    }
                }
                if (removeAudoListenersOnRemote)
                {
                    foreach (AudioListener c in GetComponentsInChildren<AudioListener>())
                    {
                        GameObject.Destroy(c);
                    }
                }
            }
        }
    }
}
