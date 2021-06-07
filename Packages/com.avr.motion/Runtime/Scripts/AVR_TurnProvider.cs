using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using AVR;
using AVR.Core;
using AVR.Core.Attributes;

namespace AVR.Motion {
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_motion_1_1_a_v_r___turn_provider.html")]
    /// <summary>
    /// Provides turning functionality to a controller.
    /// </summary>
    public class AVR_TurnProvider : AVR_ControllerComponent
    {
        /// <summary>
        /// Mode of turning. Snap will instantly turn by a given amount while smooth is a continous turn over time
        /// </summary>
        public enum turnMode { SNAP, SMOOTH };

        [Header("Mode")]
        /// <summary>
        /// Mode of turning. Snap will instantly turn by a given amount while smooth is a continous turn over time
        /// </summary>
        public turnMode mode;

        [Header("Input")]
        /// <summary>
        /// Event on which a turn is performed.
        /// </summary>
        public AVR_ControllerInputManager.BoolEvent turnEvent;

        /// <summary>
        /// Float value that determines the direction of the turn. 
        /// </summary>
        public AVR_ControllerInputManager.FloatEvent turnDirection;

        [Header("Settings")]
        [ConditionalHideInInspector("mode", ((int)turnMode.SNAP), ConditionalHideInInspector.compareType.EQUAL, true)]
        /// <summary>
        /// If the mode is set to snap, the amount of rotation (in degrees) is determined by this value
        /// </summary>
        public float snp_rotationAmount = 45;

        [ConditionalHideInInspector("mode", ((int)turnMode.SNAP), ConditionalHideInInspector.compareType.EQUAL, true)]
        /// <summary>
        /// If the mode is set to snap, this determines the amount of time (in seconds) that has to pass between turns
        /// </summary>
        public float snp_cooldown = 0.5f;

        [ConditionalHideInInspector("mode", ((int)turnMode.SMOOTH), ConditionalHideInInspector.compareType.EQUAL, true)]
        /// <summary>
        /// If the mode is set to smooth, this determines the speed of rotation (id deg/s)
        /// </summary>
        public float smt_rotationSpeed = 45; //In degrees per second

        private float stime;

        protected override void Awake()
        {
            base.Awake();
            stime = Time.time;
        }

        void Update()
        {
            // Snap turn triggered
            if(mode==turnMode.SNAP && controller.inputManager.getEventStatus(turnEvent) && (stime + snp_cooldown) < Time.time) {
                stime = Time.time; // Restart cooldown timer
                // Turn left
                if (controller.inputManager.getEventStatus(turnDirection) < -0.5)
                    AVR_PlayerRig.Instance.transform.Rotate(Vector3.up, -snp_rotationAmount, Space.Self);
                // Turn right
                else if (controller.inputManager.getEventStatus(turnDirection) > 0.5)
                    AVR_PlayerRig.Instance.transform.Rotate(Vector3.up, snp_rotationAmount, Space.Self);
            }

            else if(mode==turnMode.SMOOTH && controller.inputManager.getEventStatus(turnEvent)) {
                // Turn left
                if (controller.inputManager.getEventStatus(turnDirection) < -0.5)
                    AVR_PlayerRig.Instance.transform.Rotate(Vector3.up, -smt_rotationSpeed * Time.deltaTime, Space.Self);
                // Turn right
                else if (controller.inputManager.getEventStatus(turnDirection) > 0.5)
                    AVR_PlayerRig.Instance.transform.Rotate(Vector3.up, smt_rotationSpeed * Time.deltaTime, Space.Self);
            }
        }
    }
}
