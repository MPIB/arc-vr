﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.Core;

namespace AVR.UI {
    /// <summary>
    /// Represents a UI pointer-ray.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_u_i_1_1_a_v_r___u_i_ray.html")]
    public class AVR_UIRay : AVR_Ray
    {
        [Header("UI Camera")]
        /// <summary>
        /// Camera that is the eventCamera for the underlying inputsystem. If left blank, a Camera with preset value will be added.
        /// </summary>
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
                AVR_DevConsole.cwarn("AVR_UIRay was initialized with a mode other than RayMode.STRAIGHT! UIRays can only be straight.", this);
                mode = RayMode.STRAIGHT;
            }
            base.Awake();
        }
    }
}
