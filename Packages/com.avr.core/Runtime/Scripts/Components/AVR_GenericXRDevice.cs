using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace AVR.Core
{
    /// <summary>
    /// Represents some spatially tracked XR device. Provides functionality for spacial tracking.
    /// Can be used to represent hardware trackers, game controllers or other references.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___generic_x_r_device.html")]
    public class AVR_GenericXRDevice : AVR_Component
    {
        [Header("XR Node")]

        /// <summary>
        /// Which XRNode this object represents.
        /// </summary> 
        public XRNode controllerNode;

        /// <summary>
        /// XR.Inputdevice that this controller represents.
        /// </summary> 
        public InputDevice inputDevice => _inputDevice.isValid ? _inputDevice : _inputDevice = InputDevices.GetDeviceAtXRNode(controllerNode);
        private InputDevice _inputDevice;


        [Header("Tracking")]

        /// <summary>
        /// Defines if spatial tracking should be performed.
        /// </summary>
        public bool tracking = true;

        /// <summary>
        /// Defines when positional tracking should take place. Recommended setting is OnBeforeRenderAndUpdate
        /// </summary>
        public enum TrackingUpdateType
        {
            OnUpdate,
            OnBeforeRender,
            OnBeforeRenderAndUpdate,
        }

        /// <summary>
        /// Defines when positional tracking should take place. Recommended setting is OnBeforeRenderAndUpdate
        /// </summary>
        [AVR.Core.Attributes.ConditionalHideInInspector("tracking", true)]
        public TrackingUpdateType updateType;


        [Header("Smoothing")]

        /// <summary>
        /// Defines if positional tracking is smoothed across several frames. Makes controllers less shaky.
        /// </summary>
        public bool smoothing = false;

        /// <summary>
        /// Defines the amount of smoothing.
        /// </summary>
        [AVR.Core.Attributes.ConditionalHideInInspector("smoothing", true)]
        public float smoothingFidelity = 0.3f;

        // Variables for smoothing
        private Vector3 prevPos = Vector3.zero;
        private Quaternion prevRot = Quaternion.identity;



        /// <summary>
        /// Updates the controller position and rotation.
        /// </summary>
        protected void UpdateTracking()
        {
            if (inputDevice.TryGetFeatureValue(CommonUsages.trackingState, out var trackingState))
            {
                if ((trackingState & InputTrackingState.Position) != 0 && inputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out var devicePosition))
                {
                    if (smoothing)
                    {
                        transform.localPosition = Vector3.Lerp(transform.localPosition, devicePosition, Time.deltaTime * smoothingFidelity * 90.0f);
                    }
                    else
                    {
                        transform.localPosition = devicePosition;
                    }
                }

                if ((trackingState & InputTrackingState.Rotation) != 0 && inputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out var deviceRotation))
                {
                    if (smoothing)
                    {
                        transform.localRotation = Quaternion.Lerp(transform.localRotation, deviceRotation, Time.deltaTime * smoothingFidelity * 90.0f);
                    }
                    else
                    {
                        transform.localRotation = deviceRotation;
                    }
                }
            }
        }

        protected void OnBeforeRender()
        {
            if (tracking && (updateType == TrackingUpdateType.OnBeforeRender || updateType == TrackingUpdateType.OnBeforeRenderAndUpdate))
            {
                UpdateTracking();
            }
        }

        protected void Update()
        {
            if (tracking && (updateType == TrackingUpdateType.OnUpdate || updateType == TrackingUpdateType.OnBeforeRenderAndUpdate))
            {
                UpdateTracking();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Application.onBeforeRender += OnBeforeRender;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Application.onBeforeRender -= OnBeforeRender;
        }
    }
}
