using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVR.Core {
    /// <summary>
    /// AVR_Component specifically attatched to an AVR_PlayerRig. Needs to have an AVR_PlayerRig on this gameObject or one of its parents.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___rig_component.html")]
    public class AVR_RigComponent : AVR_Component
    {
        public AVR_PlayerRig rig { private set; get; }

        protected override void Awake()
        {
            rig = GetComponentInParent<AVR_PlayerRig>();
            if(!rig) {
                AVR_DevConsole.cerror("AVR_RigComponent does not have an AVR_PlayerRig in on itself or one of its parents!", this);
            }
            base.Awake();
        }
    }
}
