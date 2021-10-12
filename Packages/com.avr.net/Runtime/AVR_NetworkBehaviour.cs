using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

using AVR.Core;

/// <summary>
/// Namespace of the arc-vr-net package
/// </summary>
namespace AVR.Net {
    /// <summary>
    /// MLAPI NetworkBehaviour but with an added URL button to a documentation page.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___behaviour.html")]
    public class AVR_NetworkBehaviour : NetworkBehaviour
    {
        /// <summary>
        /// The current AVR_PlayerRig instance in the scene
        /// </summary>
        protected AVR_PlayerRig playerRig => AVR_PlayerRig.Instance;
    }
}
