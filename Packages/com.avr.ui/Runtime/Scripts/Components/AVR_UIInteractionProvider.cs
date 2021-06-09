using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.Core;

namespace AVR.UI {
    /// <summary>
    /// This class provides UI Interaction through a VR controller. NOTE: Often Enabling/Disabling this component leads to poor performance! -> TODO: implement a custom function that "enables/disables" functionality without the OnEnable/Disable hassle
    /// Requires a AVR_UIRay to be set.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_u_i_1_1_a_v_r___u_i_interaction_provider.html")]
    public class AVR_UIInteractionProvider : AVR_ControllerComponent
    {
        /// <summary>
        /// The currently active interactionprovider. NOTE: only one interactionprovider will be active at a time. 
        /// </summary>
        public static AVR_UIInteractionProvider currentActive;

        [Header("Input")]
        /// <summary>
        /// Event that would normally be LMB.
        /// </summary>
        public AVR_ControllerInputManager.BoolEvent clickButton;

        /// <summary>
        /// Event that represents LMB buttondown
        /// </summary>
        public AVR_ControllerInputManager.BoolEvent clickButton_down;

        /// <summary>
        /// Event that represents LMB buttonup
        /// </summary>
        public AVR_ControllerInputManager.BoolEvent clickButton_up;

        [Header("Settings")]
        /// <summary>
        /// Only show UIRay when hovering over a canvas or always.
        /// </summary>
        public bool show_ray_only_on_hover = false;

        [Header("Pointer")]
        /// <summary>
        /// Pointer ray of this interactionprovider. Is required for the interactionprovider to work.
        /// </summary>
        public AVR_UIRay UIRay;

        // We use this variable as a timer in set().
        private float stime = 0.0f;

        protected override void OnEnable() {
            base.OnEnable();
            stime = Time.realtimeSinceStartup;
            set();
        }

        protected override void OnDisable() {
            base.OnDisable();
            unset();
        }

        protected override void Awake()
        {
            base.Awake();
            UIRay = GetComponentInChildren<AVR_UIRay>();

            if(!UIRay) {
                AVR_DevConsole.cerror("UIInteractionProvider requires an AVR_UIRay to function!", this);
                Destroy(this);
            }
        }

        // Sets this UIPorvider as the currently active one.
        void set() {
            // TODO: This is a shitty workaround. AVR_Controller.Awake() calls *after* this, so inputManager is not set on the first frame.
            // We are retrying for 1 second.
            if(Time.realtimeSinceStartup - stime < 1.0 && !controller.inputManager) {
                Invoke("set", 0.0f);
                return;
            }

            if(currentActive) {
                AVR_DevConsole.warn("Attempted to set UIInteractionProvider \"" + gameObject.name + "\" but another UIInteractionprovider is already active! Only one can be active at a time.");
                currentActive.enabled = false;
            }
            else if(!controller.inputManager) {
                AVR_DevConsole.error("AVR_UIInteractionProvider references a controller that has no inputmanager!");
                return;
            }
            else if(!VRInput.Instance) {
                AVR_DevConsole.error("AVR_UIInteractionProvider can only be used with a VRInput component!");
                return;
            }
            VRInput.Instance.clickButton = clickButton;
            VRInput.Instance.clickButton_down = clickButton_down;
            VRInput.Instance.clickButton_up = clickButton_up;
            VRInput.Instance.inputManager = controller.inputManager;
            VRInput.Instance.setEventCamera(UIRay.UICamera);
            currentActive = this;
        }

        // Un-Sets this UIPorvider as the currently active one.
        void unset() {
            VRInput.Instance.inputManager = null;
            VRInput.Instance.setEventCamera(null);
            currentActive = null;
        }

        void Update() {
            // Here we show/hide the UIRay if needed and also set the length of the ray to the distance to the canvas.
            if(AVR_Canvas.active_canvases.Count>0) {
                float min_dist = float.PositiveInfinity;
                foreach(AVR_Canvas canv in AVR_Canvas.active_canvases) {
                    if(canv.GetPlane().Raycast(new Ray(UIRay.UICamera.transform.position, UIRay.UICamera.transform.forward), out float dist)) {
                        min_dist = Mathf.Min(dist, min_dist);
                    }
                }
                UIRay.max_length = min_dist;
                UIRay.show();
            }
            else {
                UIRay.max_length = 30;
                if (show_ray_only_on_hover) UIRay.hide();
            }
        }
    }
}
