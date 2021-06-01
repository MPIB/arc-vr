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
        /// <summar> Events called when this component awakes. </summary>
        [HideInInspector] public UnityEngine.Events.UnityEvent onAwake;

        /// <summar> Events called when this component starts. </summary>
        [HideInInspector] public UnityEngine.Events.UnityEvent onStart;

        /// <summar> Events called when this component is enabled. </summary>
        [HideInInspector] public UnityEngine.Events.UnityEvent onEnable;

        /// <summar> Events called when this component is disabled. </summary>
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

        /// <summar> False if this component should be destroyed on a remote server. </summary>
        [HideInInspector]
        public bool KeepOnRemote = false;

        /// <summar> True if this component should have a different layer on a remote server. </summary>
        [HideInInspector]
        public bool ChangeLayerOnRemote = false;

        /// <summar> True if children should be included on a remote layer change. </summary>
        [HideInInspector]
        public bool ChangeLayerOnRemote_IncludeChildren = false;

        /// <summar> Layer an object should change to if on a remote server. (See ChangeLayerOnRemote) </summary>
        [HideInInspector]
        public int RemoteLayer = 0;

        /// <summar> Called when this component starts/spawns on a remote host. </summary>
        public void OnRemote() {
            if (ChangeLayerOnRemote) {
                gameObject.layer = RemoteLayer;
                if(ChangeLayerOnRemote_IncludeChildren) {
                    foreach(Transform child in transform) {
                        child.gameObject.layer = RemoteLayer;
                    }
                }
            }
            if(!KeepOnRemote) {
                GameObject.Destroy(this);
            }
        }

#endif
    }
}
