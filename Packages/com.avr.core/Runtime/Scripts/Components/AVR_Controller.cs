using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace AVR.Core {
    /// <summary>
    /// Represents a VR controller. Provides functionality for spacial tracking and haptic feedback.
    /// Can also be used to represent hardware trackers, game controllers or other references.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___controller.html")]
    public class AVR_Controller : AVR_GenericXRDevice
    {
        /// <summary>
        /// the AVR_ControllerInputManager assigned to this controller. Returns null if no inputmanager is assigned
        /// </summary>
        public AVR_ControllerInputManager inputManager {
            get {
                return _inputManager;
            }
        }
        private AVR_ControllerInputManager _inputManager=null;



        protected override void Awake() {
            base.Awake();
            _inputManager = GetComponentInChildren<AVR_ControllerInputManager>();
        }

        /// <summary>
        /// Returns the haptic capabilites of this controller.
        /// </summary>
        public HapticCapabilities GetHapticCapabilities() {
            if(inputDevice.TryGetHapticCapabilities(out var capabilities)) {
                return capabilities;
            }
            else {
                AVR_DevConsole.cwarn("Could not retrieve haptic capavilites from controller.", this);
                return default(HapticCapabilities);
            }
        }

        /// <summary>
        /// Performs a haptic pulse on this controller (if available)
        /// <param name="amplitude"> Amplitude of the pulse. </param>
        /// <param name="duration"> Duration of the pulse. </param>
        /// </summary>
        public void HapticPulse(float amplitude, float duration) {
            if (GetHapticCapabilities().supportsImpulse)
            {
                inputDevice.SendHapticImpulse(0u, amplitude, duration);
            }
        }
    }
}
