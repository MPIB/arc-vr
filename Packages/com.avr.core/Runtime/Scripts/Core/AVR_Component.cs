using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace AVR.Core {
    /// <summary>
    /// Base class for all arc-vr components. A component is typically a Monobehaviour that represents a virtual device, feature-provider or module.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___component.html")]
    public class AVR_Component : AVR_Behaviour
    {
        /// <summary> Events called when this component awakes. </summary>
        [HideInInspector] public UnityEngine.Events.UnityEvent onAwake;

        /// <summary> Events called when this component starts. </summary>
        [HideInInspector] public UnityEngine.Events.UnityEvent onStart;

        /// <summary> Events called when this component is enabled. </summary>
        [HideInInspector] public UnityEngine.Events.UnityEvent onEnable;

        /// <summary> Events called when this component is disabled. </summary>
        [HideInInspector] public UnityEngine.Events.UnityEvent onDisable;

        protected virtual void Awake() {
            onAwake.Invoke();
        }

        protected virtual void Start() {
            onStart.Invoke();
        }

        protected virtual void OnEnable() {
            onEnable.Invoke();
        }

        protected virtual void OnDisable() {
            onDisable.Invoke();
        }

#if AVR_NET
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if(!IsLocalPlayer)
            {
                onRemoteStart.Invoke();

                if (changeLayerOnRemote)
                {
                    gameObject.layer = remoteLayer;
                    if (changeLayerOnRemote_IncludeChildren)
                    {
                        foreach (Transform child in GetComponentsInChildren<Transform>())
                        {
                            child.gameObject.layer = remoteLayer;
                        }
                    }
                }

                if (destroyOnRemote)
                {
                    GameObject.Destroy(this);
                    // Under normal conditions Update() runs for 1 frame before Destroy takes place. DestroyImmediate is dangerous to use here.
                    // This is a hacky way of preventing Update from running: We set gameobject.active = false (component.enabled doesn't do the trick)
                    // And then re-enable the gameobject at the end of the frame through AVR_Root.
                    gameObject.SetActive(false);
                    AVR_Root.Instance.ReEnableAtEndOfFrame(gameObject);
                }
            }
        }

        [HideInInspector]
        public bool destroyOnRemote = true;
        [HideInInspector]
        public bool changeLayerOnRemote = false;
        [HideInInspector]
        public bool changeLayerOnRemote_IncludeChildren = false;
        [HideInInspector]
        public int remoteLayer = 0;

        [HideInInspector] public UnityEngine.Events.UnityEvent onRemoteStart;

        protected interface IInternalState<T> : Unity.Netcode.INetworkSerializable where T : AVR_Component
        {
            public void From(T reference);
            public void Apply(T reference);
        }
#endif
    }
}
