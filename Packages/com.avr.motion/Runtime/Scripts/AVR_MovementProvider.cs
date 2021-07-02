using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR;
using AVR.Core;
using AVR.Core.Attributes;
using ct = AVR.Core.Attributes.ConditionalHideInInspector.compareType;

namespace AVR.Motion {
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_motion_1_1_a_v_r___movement_provider.html")]
    /// <summary>
    /// Provides common teleportation/movement functionality to a controller
    /// </summary>
    public class AVR_MovementProvider : AVR_ControllerComponent
    {
        [Header("Input")]
        /// <summary>
        /// Event which enables/shows the teleportation-ray.
        /// </summary>
        public AVR_ControllerInputManager.BoolEvent Enable_event;

        /// <summary>
        /// Event which (if possible) executes the movement to target location. Can only ocurr if the ray is visible, eg if enable_event is also true.
        /// </summary>
        public AVR_ControllerInputManager.BoolEvent Execute_event;

        /// <summary>
        /// Modes of teleportation/movement.
        /// INSTANT: Instantly moves to the target position
        /// FADE_COLOR: Fades the screen to a given color, teleports to target location, then fades the color back out.
        /// DASH: Moves to target location in a linear fash with a given speed.
        /// FADE_DASH: Combination of FADE_COLOR and DASH. Fades the camera to a given color, then (while the curtains are closed) linearly translates the rig to target location. Then fades the colro back out.
        /// FADE_DASH or DASH are the modes of choice when using an avatar, as they provide a speed value.
        /// </summary>
        public enum TeleportModes { INSTANT, FADE_COLOR, DASH, FADE_DASH }
        // TODO: Transition where we overlay two cameras smoothly

        [Header("Movement type")]
        /// <summary>
        /// Modes of teleportation/movement.
        /// INSTANT: Instantly moves to the target position
        /// FADE_COLOR: Fades the screen to a given color, teleports to target location, then fades the color back out.
        /// DASH: Moves to target location in a linear fash with a given speed.
        /// FADE_DASH: Combination of FADE_COLOR and DASH. Fades the camera to a given color, then (while the curtains are closed) linearly translates the rig to target location. Then fades the colro back out.
        /// FADE_DASH or DASH are the modes of choice when using an avatar, as they provide a speed value.
        /// </summary>
        public TeleportModes teleportMode;

        /// <summary>
        /// Determines which surface is a valid teleport-location and which is not.
        /// </summary>
        public AVR_MovementRestrictor movementRestrictor;

        /// <summary>
        /// Ray which is displayed to target a location.
        /// </summary>
        public AVR_MovementRay ray;

        [Header("Mode-specifics")]
        /// <summary>
        /// CameraFadeEffect used when teleportmode is FADE_COLOR or FADE_DASH.
        /// </summary>
        public AVR_Effect CameraFadeEffect;

        /// <summary>
        /// When using FADE_COLOR or FADE_DASH, this determines the amount of time between fade-in and fade-out transitions.
        /// </summary>
        public float fade_pause = 0.1f;

        /// <summary>
        /// When using DASH or FADE_DASH, this determines the speed (in units/s) we travel towards the target location.
        /// When using DASH its recommended to keep this reasonably high to avoid motion sickness.
        /// </summary>
        public float dash_speed = 30.0f;

        // Private
        private bool tp_in_progress = false;
        private Vector3 destination;
        private Vector3 dash_origin;
        private float dash_stime;

        protected override void Start() {
            base.Start();
            if (!CameraFadeEffect) CameraFadeEffect = GetComponent<AVR_Effect>();
            if (!ray) ray = GetComponentInChildren<AVR_MovementRay>();
            ray.filter = movementRestrictor.isValidTpLocation;
            ray.hide();
        }

        void Update() {
            // Show/hide ray depending on given enable event
            if(controller.inputManager.getEventStatus(Enable_event)) {
                ray.show();
            }
            else {
                ray.hide();
            }

            // Begin teleport if execute event
            if(ray.isVisible && ray.objectHit && controller.inputManager.getEventStatus(Execute_event) && ray.isValid) {
                CommenceTeleport(ray.hitPosition.point);
            }

            // A dash-movement should be continously updated. We do that here.
            if(tp_in_progress && teleportMode==TeleportModes.DASH) {
                float t = Time.time - dash_stime;
                float inter =(t*dash_speed) / (Vector3.Distance(dash_origin, destination));
                MoveRigToFeetLocation(Vector3.Lerp(dash_origin, destination, inter));
                if(inter >= 1.0f) {
                    tp_in_progress = false;
                }
            }
        }

        /// <summary>
        /// This coroutine is exectued when a blink-movement (INSTANT) is performed.
        /// </summary>
        IEnumerator BlinkTeleport() {
            tp_in_progress = true;                                      // Set active flag
            CameraFadeEffect.StartEffect();                             // Shut curtains
            yield return new WaitWhile(()=>CameraFadeEffect.isBusy());  // 
            yield return new WaitForSeconds(fade_pause);                // Pause
            MoveRigToFeetLocation(destination);                         // Move rig to destination
            CameraFadeEffect.EndEffect();                               // Reopen curtains
            yield return new WaitWhile(() => CameraFadeEffect.isBusy());//
            tp_in_progress = false;                                     // Reset flag
        }

        /// <summary>
        /// This coroutine is exectued when a fade-dash-movement is performed.
        /// </summary>
        IEnumerator FadeDash()
        {
            dash_origin = playerRig.FeetInWorldSpace;
            dash_stime = Time.time;
            tp_in_progress = true;                                      // Set active flag

            CameraFadeEffect.StartEffect();                             // Shut curtains
            yield return new WaitWhile(() => CameraFadeEffect.isBusy()); 

            float inter = 0.0f;
            while(inter < 1.0f) {
                yield return new WaitForEndOfFrame();
                inter = ((Time.time - dash_stime) * dash_speed) / (Vector3.Distance(dash_origin, destination));
                MoveRigToFeetLocation(Vector3.Lerp(dash_origin, destination, inter));
            }

            CameraFadeEffect.EndEffect();                               // Reopen curtains
            yield return new WaitWhile(() => CameraFadeEffect.isBusy());
            tp_in_progress = false;                                     // Reset flag
        }

        /// <summary>
        /// Begins a movement towards target location
        /// </summary>
        /// <param name="targetLocation">Feet location we teleport/move to</param>
        void CommenceTeleport(Vector3 targetLocation) {
            if(tp_in_progress) return;

            destination = targetLocation;

            switch(teleportMode) {
                case TeleportModes.INSTANT : {
                    MoveRigToFeetLocation(destination);
                    break;
                }
                case TeleportModes.FADE_COLOR : {
                    StartCoroutine(BlinkTeleport());
                    break;
                }
                case TeleportModes.DASH : {
                    dash_origin = playerRig.FeetInWorldSpace;
                    tp_in_progress = true;
                    dash_stime = Time.time;
                    break;
                }
                case TeleportModes.FADE_DASH : {
                    StartCoroutine(FadeDash());
                    break;
                }
                default : {
                    AVR_DevConsole.cwarn("The given teleportMode is not valid!", this);
                    break;
                }
            }
        }

        void MoveRigToFeetLocation(Vector3 feet) {
            playerRig.MoveRigToFeetPosition(feet);
        }
    }
}
