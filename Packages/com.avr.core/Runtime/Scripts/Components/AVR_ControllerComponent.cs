using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVR.Core {
    /// <summary>
    /// AVR_Component specifically attatched to an AVR_Controller. Needs to have an AVR_Controller on this gameObject or one of its parents.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___controller_component.html")]
    public class AVR_ControllerComponent : AVR_Component
    {
        public AVR_Controller controller { private set; get; }

        protected override void Awake()
        {
            controller = GetComponentInParent<AVR_Controller>();
            if(!controller) {
                AVR_DevConsole.cerror("AVR_ControllerComponent does not have an AVR_Controller in on itself or one of its parents!", this);
            }
            base.Awake();
        }
    }
}
