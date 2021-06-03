using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using AVR;
using AVR.Core;

namespace AVR.Motion {
    public class AVR_TurnProvider : AVR_ControllerComponent
    {
        public enum turnMode { SNAP, SMOOTH };

        public turnMode mode;

        public AVR_ControllerInputManager.BoolEvent turnEvent;
        public AVR_ControllerInputManager.FloatEvent turnDirection;

        public float snp_rotationAmount = 45;
        public float snp_cooldown = 0.5f;

        public float smt_rotationSpeed = 45; //In degrees per second

        private float stime;

        protected override void Awake()
        {
            base.Awake();
            stime = Time.time;
        }

        void Update()
        {
            //TODO: exclusivity with other components
            //if (AVR_Root.Instance.tmp1.enabled || AVR_Root.Instance.tmp2.gameObject.activeSelf) return; //cheaty line, TODO: delete this

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
