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
    }
}
