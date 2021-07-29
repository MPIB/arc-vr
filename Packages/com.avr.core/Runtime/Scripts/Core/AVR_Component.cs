using System.Collections;
using System.Collections.Generic;
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
            #if AVR_NET
            onNetworkStart();
            #endif
            onStart.Invoke();
        }

        protected virtual void OnEnable() {
            onEnable.Invoke();
        }

        protected virtual void OnDisable() {
            onDisable.Invoke();
        }

        #if AVR_NET
        public virtual void onNetworkStart()
        {
            if (networkAPI.isOnline() && !networkAPI.isLocalPlayer(this))
            {
                onRemoteStart.Invoke();

                if (destroyOnRemote)
                {
                    GameObject.Destroy(this);
                    // Under normal conditions Update() runs for 1 frame before Destroy takes place. DestroyImmediate is dangerous to use here.
                    // This is a hacky way of preventing Update from running: We set gameobject.active = false (component.enabled doesn't do the trick)
                    // And then re-enable the gameobject at the end of the frame through AVR_Root.
                    gameObject.SetActive(false);
                    AVR_Root.Instance.ReEnableAtEndOfFrame(gameObject);
                }

                if (changeLayerOnRemote)
                {
                    gameObject.layer = remoteLayer;
                    if (changeLayerOnRemote_IncludeChildren)
                    {
                        foreach (Transform child in transform)
                        {
                            child.gameObject.layer = remoteLayer;
                        }
                    }
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

        public abstract class ComponentNetworkAPI
        {
            public abstract int instanceId(AVR_Component comp);
            public abstract ulong networkId(AVR_Component comp);
            public abstract ulong ownerId(AVR_Component comp);
            public abstract bool isSpawned(AVR_Component comp);
            public abstract bool isLocalPlayer(AVR_Component comp);
            public abstract bool isOwner(AVR_Component comp);
            public abstract bool isOwnedByServer(AVR_Component comp);
            public abstract bool isPlayerObject(AVR_Component comp);
            public abstract bool? isSceneObject(AVR_Component comp);
            public abstract bool isOnline();
        }

        public class DefaultNetworkAPI : ComponentNetworkAPI
        {
            public override int instanceId(AVR_Component comp) => 0;
            public override ulong networkId(AVR_Component comp) => 0;
            public override ulong ownerId(AVR_Component comp) => 0;
            public override bool isSpawned(AVR_Component comp) => false;
            public override bool isLocalPlayer(AVR_Component comp) => false;
            public override bool isOwner(AVR_Component comp) => false;
            public override bool isOwnedByServer(AVR_Component comp) => false;
            public override bool isPlayerObject(AVR_Component comp) => false;
            public override bool? isSceneObject(AVR_Component comp) => false;
            public override bool isOnline() => false;
        }

        [HideInInspector]
        public Object networkObject;

        public static ComponentNetworkAPI networkAPI
        {
            get
            {
                if (_napi == null) _napi = new DefaultNetworkAPI();
                return _napi;
            }
            set
            {
                _napi = value;
            }
        }
        private static ComponentNetworkAPI _napi;
        #endif
    }
}
