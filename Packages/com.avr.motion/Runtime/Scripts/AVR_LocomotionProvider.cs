using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.Core;

namespace AVR.Motion {
    /// <summary>
    /// Provides locomotion-movement to a controller.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_motion_1_1_a_v_r___locomotion_provider.html")]
    public class AVR_LocomotionProvider : AVR_ControllerComponent
    {
        /// <summary>
        /// What should determine the direction of movement? Example: "RELATIVE_TO_CONTROLLER" will move the Rig in the direction the controller is facing.
        /// </summary>
        public enum DirectionType { RELATIVE_TO_CONTROLLER, RELATIVE_TO_HEADSET, RELATIVE_TO_RIG, ABSOLUTE }

        /// <summary>
        /// The input is given by a 2d vector from the controller (moveDirection), this value determines how this 2d vector maps into 3d movement.
        /// It gives the directions (with local space given by DirectionType) that represent the x and y inputs.
        /// Example: "XZ" mean input.x maps to movement along the x-axis and input.y maps to movement along the z-axis
        /// </summary>
        public enum DirectionAxes { XY, XZ, YX, YZ, ZX, ZY }

        [Header("Input")]
        /// <summary>
        /// Movement is only performed when this event is true.
        /// </summary>
        public AVR_ControllerInputManager.BoolEvent moveEvent;

        /// <summary>
        /// The event we retrieve our movement vector from.
        /// </summary>
        public AVR_ControllerInputManager.Vec2Event moveDirection;

        [Header("Direction Controls")]
        /// <summary>
        /// What should determine the direction of movement? Example: "RELATIVE_TO_CONTROLLER" will move the Rig in the direction the controller is facing.
        /// </summary>
        public DirectionType dirType = DirectionType.RELATIVE_TO_CONTROLLER;

        /// <summary>
        /// The input is given by a 2d vector from the controller (moveDirection), this value determines how this 2d vector maps into 3d movement.
        /// It gives the directions (with local space given by DirectionType) that represent the x and y inputs.
        /// Example: "XZ" mean input.x maps to movement along the x-axis and input.y maps to movement along the z-axis
        /// </summary>
        public DirectionAxes dirAxes = DirectionAxes.XZ;
        
        [Header("Settings")]
        /// <summary>
        /// Determines the base movement speed of the charactercontroller.
        /// </summary>
        public float speed = 2.0f;

        /// <summary>
        /// Determines whether if gravity is applied to the charactercontroller.
        /// </summary>
        public bool use_gravity = true;

        /// <summary>
        /// Disables movement along the world-space y-axis. This exludes gravity, which will still be applied if use_gravity is set.
        /// </summary>
        public bool disable_y_axis = false;

        /// <summary>
        /// If set to false, speed will be set instantly. If set to true, movement will accelerate over time.
        /// </summary>
        public bool use_acceleration = false;

        [AVR.Core.Attributes.ConditionalHideInInspector("use_acceleration", true)]
        [Range(0, 1)]
        /// <summary>
        /// Determines the amount acceleration. 0 means no movement ever, 1 means instant acceleration.
        /// </summary>
        public float acceleration_constant = 0.1f;

        // Private vars:
        private System.Func<Vector2, Vector3> tF;
        private CharacterController ch => playerRig.characterController;
        private Vector3 movement = Vector3.zero;

        protected override void Awake()
        {
            base.Awake();

            switch(dirAxes) {
                case DirectionAxes.XY: { tF = (v) => new Vector3(v.x, v.y, 0); break; }
                case DirectionAxes.XZ: { tF = (v) => new Vector3(v.x, 0, v.y); break; }
                case DirectionAxes.YX: { tF = (v) => new Vector3(v.y, v.x, 0); break; }
                case DirectionAxes.YZ: { tF = (v) => new Vector3(0, v.x, v.y); break; }
                case DirectionAxes.ZX: { tF = (v) => new Vector3(v.y, 0, v.x); break; }
                case DirectionAxes.ZY: { tF = (v) => new Vector3(0, v.y, v.x); break; }
                default: { AVR_DevConsole.cwarn("The given DirectionAxes are invalid.", this); tF = (v) => new Vector3(v.x, v.y, 0); break; }
            }

            if(!use_acceleration) acceleration_constant = 1.0f;
        }

        // This needs to be in LateUpdate because AVR_CharacterController.Update() NEEDS to run first.
        void LateUpdate()
        {
            if(controller.inputManager.getEventStatus(moveEvent)) {
                Vector2 md = controller.inputManager.getEventStatus(moveDirection);
                Vector3 dir = tF(md);

                switch(dirType) {
                    case DirectionType.RELATIVE_TO_CONTROLLER : {
                        dir = controller.transform.TransformDirection(dir);
                        break;
                    }
                    case DirectionType.RELATIVE_TO_RIG : {
                        dir = playerRig.transform.TransformDirection(dir);
                        break;
                    }
                    case DirectionType.RELATIVE_TO_HEADSET : {
                        dir = playerRig.MainCamera.transform.TransformDirection(dir);
                        break;
                    }
                }

                movement = Vector3.Lerp(movement, dir * speed, acceleration_constant);

                if(disable_y_axis) movement.y = 0;
                
                if(ch) {
                    ch.Move(movement * Time.deltaTime);

                    if(use_gravity) {
                        Vector3 gravity = Physics.gravity * Time.deltaTime;
                        ch.Move(gravity);
                    }
                }
                else {
                    AVR_DevConsole.cerror("The PlayerRig does not have a charactercontroller. LocomotionProvider requires a charactercontroller.", this);
                }
            }
        }
    }
}
