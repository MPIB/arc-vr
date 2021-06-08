using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.Core;

namespace AVR.UI {
    public class AVR_UIRay : AVR_Ray
    {
        [Header("UI Camera")]
        public Camera UICamera;

        protected override void Awake() {
            if(!UICamera) {
                UICamera = lr.gameObject.AddComponent<Camera>();
                UICamera.clearFlags = CameraClearFlags.Nothing;
                UICamera.cullingMask = 0;
                UICamera.fieldOfView = 0.00001f;
                UICamera.nearClipPlane = 0.01f;
                UICamera.farClipPlane = 1000f;
                UICamera.depth = 0f;
                UICamera.allowHDR = false;
                UICamera.allowMSAA = false;
                UICamera.targetDisplay = 7;
                UICamera.enabled = false;
            }
            if(mode!=RayMode.STRAIGHT) {
                AVR_DevConsole.error("AVR_UIRay was initialized with a mode other than RayMode.STRAIGHT! UIRays can only be straight.");
            }
            base.Awake();
        }
    }
}
