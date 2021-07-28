using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
namespace AVR.Core
{
    public class AVR_Component : AVR_Behaviour
    {
        
        /// <summary> False if this component should be destroyed on a remote server. </summary>
        [HideInInspector]
        public bool KeepOnRemote = false;

        /// <summary> True if this component should have a different layer on a remote server. </summary>
        [HideInInspector]
        public bool ChangeLayerOnRemote = false;

        /// <summary> True if children should be included on a remote layer change. </summary>
        [HideInInspector]
        public bool ChangeLayerOnRemote_IncludeChildren = false;

        /// <summary> Layer an object should change to if on a remote server. (See ChangeLayerOnRemote) </summary>
        [HideInInspector]
        public int RemoteLayer = 0;

        /// <summary> Called when this component starts/spawns on a remote host. </summary>
        public void OnRemote()
        {
            if (ChangeLayerOnRemote)
            {
                gameObject.layer = RemoteLayer;
                if (ChangeLayerOnRemote_IncludeChildren)
                {
                    foreach (Transform child in transform)
                    {
                        child.gameObject.layer = RemoteLayer;
                    }
                }
            }
            if (!KeepOnRemote)
            {
                GameObject.Destroy(this);
            }
        }

        protected virtual void Awake()
        {
            Debug.Log("hu");
        }
        

        partial void OnRemote() {
            Debug.Log("hi");
        }
    }
}
*/
