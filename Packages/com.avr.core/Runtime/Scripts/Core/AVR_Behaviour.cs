using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVR.Core {
    /// <summary>
    /// Monobehaviour but with an added URL button to a documentation page.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___behaviour.html")]
    public abstract class AVR_Behaviour : MonoBehaviour
    {
        /// <summary>
        /// The current AVR_PlayerRig instance in the scene
        /// </summary>
        protected AVR_PlayerRig playerRig => AVR_PlayerRig.Instance;
    }
}
