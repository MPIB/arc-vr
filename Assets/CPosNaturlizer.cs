using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.Core;

public class CPosNaturlizer : AVR_ControllerComponent
{

    public AVR_ControllerInputManager.BoolEvent record_event;

    public AVR_Logger logger;

    public Vector3 logPos;

    // Update is called once per frame
    void Update()
    {
        if(controller.inputManager.getEventStatus(record_event)) {
            Vector3 wpos = controller.transform.position;

            Vector3 cpos = AVR_PlayerRig.Instance.MainCamera.transform.InverseTransformPoint(wpos);

            logPos = cpos;

            logger.logObjects();
        }
    }
}
