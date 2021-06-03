using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.Core;

namespace AVR.Motion {
    public class AVR_LocomotionProvider : AVR_ControllerComponent
    {
        //public AVR_ControllerInputManager.BoolEvent moveEvent;
        public AVR_ControllerInputManager.Vec2Event moveEvent;

        // This needs to be in LateUpdate because AVR_CharacterController.Update() NEEDS to run first.
        void LateUpdate()
        {
            Vector2 move = controller.inputManager.getEventStatus(moveEvent);
            Vector3 dir = new Vector3(move.x, 0, move.y);
            
            Vector3 headRotation = new Vector3(0, AVR_PlayerRig.Instance.MainCamera.transform.eulerAngles.y, 0);

            dir = Quaternion.Euler(headRotation) * dir;

            float speed = 2.0f;
            Vector3 movement = dir * speed;
            
            CharacterController ch = GetComponent<CharacterController>();
            ch.Move(movement * Time.deltaTime);

            Vector3 gravity = Physics.gravity * 1.0f * Time.deltaTime;
            ch.Move(gravity);

            //AVR_PlayerRig.Instance.transform.position = transform.position;
            //transform.position = AVR_PlayerRig.Instance.transform.position;
        }
    }
}
