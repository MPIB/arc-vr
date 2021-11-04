using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.Core;


namespace AVR.Demos.CapsuleMPDemo {
    public class CapsulePlayerRepr : AVR_Component
    {
        void Update()
        {
            if(playerRig) {
                this.transform.position = new Vector3(playerRig.CameraInWorldSpace.x, 0f, playerRig.CameraInWorldSpace.z);
                this.transform.forward = playerRig.XZPlaneFacingDirection;
            }
        }
    }
}
