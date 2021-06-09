using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using AVR.Core;
using AVR;
using AVR.UI;

namespace AVR.UI.Utils {
    /// <summary>
    /// Window-Handle that allows the attatched canvas to be click and dragged in world space by a UIInteractionProvider.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_u_i_1_1_utils_1_1_window_handle.html")]
    public class WindowHandle : AVR_Behaviour, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// Canvas that is moved by this handle
        /// </summary>
        public AVR_Canvas canvas;

        Transform og_parent = null;
        bool clicked = false;

        void Awake() {
            if(!canvas) {
                canvas = GetComponentInParent<AVR_Canvas>();
                if(!canvas) {
                    AVR_DevConsole.cerror("WindowHandle does not have an AVR_Canvas attatched!", this);
                }
            }
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if(clicked) return;

            clicked = true;

            og_parent = canvas.transform.parent;
            canvas.anchor_to_transform(AVR_UIInteractionProvider.currentActive.transform);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!clicked) return;

            clicked = false;

            canvas.anchor_to_transform(og_parent);
        }
    }
}
