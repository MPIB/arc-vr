using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Namespace of the arc-vr-core package
/// </summary>
namespace AVR.Core {
    /// <summary>
    /// Monobehaviour but with an added URL button to a documentation page.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___behaviour.html")]
    public abstract class AVR_Behaviour :
#if AVR_NET
        Unity.Netcode.NetworkBehaviour
#else
        MonoBehaviour
#endif
    {
        /// <summary>
        /// The current AVR_PlayerRig instance in the scene
        /// </summary>
        protected AVR_PlayerRig playerRig => AVR_PlayerRig.Instance;

        /// <summary>
        /// The current AVR_Root instance in the scene
        /// </summary>
        protected AVR_Root root => AVR_Root.Instance;

        /// <summary>
        /// True if the application is running as a VR/XR application
        /// </summary>
        protected bool vrEnabled => UnityEngine.XR.XRSettings.enabled;

#if AVR_NET
        /// <summary>
        /// This method works exaclty as regular MLAPI NetworkBehaviour.HasNetworkObject, except that it *only checks once* for NetworkObjects
        /// </summary>
        new public bool HasNetworkObject {
            get {
                if(!networkobj_checked) networkobj_connected = base.HasNetworkObject;
                networkobj_checked = true;
                return networkobj_connected;
            }
        }
        private bool networkobj_checked = false;
        private bool networkobj_connected = false;

        /// <summary>
        /// True if we are hosting or connected to a network
        /// </summary>
        protected bool IsOnline => HasNetworkObject && NetworkManager && (IsServer || IsClient);

        /// <summary>
        /// Returns true if we are the network owner of this script or of we are not online.
        /// </summary>
        new public bool IsOwner => !this.IsOnline || base.IsOwner;
#else
        // We define OnDestroy here to keep consistency with Networkbehaviour
        public virtual void OnDestroy()
        {

        }
#endif
    }
}
