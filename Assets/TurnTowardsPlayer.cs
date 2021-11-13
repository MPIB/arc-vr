using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.Core;

public class TurnTowardsPlayer : AVR_Behaviour
{
    void Update()
    {
        transform.forward = Vector3.Lerp(
            transform.forward,
            -(playerRig.CameraInWorldSpace - transform.position),
            Time.deltaTime
        );
    }
}
