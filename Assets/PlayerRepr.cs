using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.Core;

public class PlayerRepr : AVR_Component
{
    void Update()
    {
        this.transform.position = new Vector3(playerRig.CameraInWorldSpace.x, 0f, playerRig.CameraInWorldSpace.z);
        this.transform.forward = playerRig.XZPlaneFacingDirection;
    }
}
