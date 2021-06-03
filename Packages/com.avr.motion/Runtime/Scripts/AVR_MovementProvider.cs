using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR;
using AVR.Core;

namespace AVR.Motion {
    public class AVR_MovementProvider : AVR_ControllerComponent
    {
        public enum TeleportModes { INSTANT, FADE_COLOR, DASH, FADE_DASH }
        // NOTE: How about a transition where we overlay two cameras smoothly

        public TeleportModes teleportMode;

        public AVR_MovementRestrictor movementRestrictor;


        //// TELEPORT

        public AVR_MovementRay ray;
        public AVR_ControllerInputManager.BoolEvent Enable_event;
        public AVR_ControllerInputManager.BoolEvent Execute_event;

        public AVR_Effect CameraFadeEffect;
        public float fade_pause = 0.1f;

        public float dash_speed = 30.0f;

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

        void FixedUpdate() {
            
        }

        void Update() {
            if(controller.inputManager.getEventStatus(Enable_event)) {
                ray.show();
            }
            else {
                ray.hide();
            }

            if(ray.isVisible && ray.objectHit && controller.inputManager.getEventStatus(Execute_event) && ray.isValid) {
                CommenceTeleport(ray.hitPosition.point);
            }

            if(tp_in_progress && teleportMode==TeleportModes.DASH) {
                float t = Time.time - dash_stime;
                float inter =(t*dash_speed) / (Vector3.Distance(dash_origin, destination));
                MoveRigToFeetLocation(Vector3.Lerp(dash_origin, destination, inter));
                if(inter >= 1.0f) {
                    tp_in_progress = false;
                }
            }
        }

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

        IEnumerator FadeDash()
        {
            dash_origin = AVR_PlayerRig.Instance.FeetInWorldSpace;
            dash_stime = Time.time;
            tp_in_progress = true;                                      // Set active flag

            CameraFadeEffect.StartEffect();                             // Shut curtains
            yield return new WaitWhile(() => CameraFadeEffect.isBusy());  // 

            float inter = 0.0f;
            while(inter < 1.0f) {
                yield return new WaitForEndOfFrame();
                inter = ((Time.time - dash_stime) * dash_speed) / (Vector3.Distance(dash_origin, destination));
                MoveRigToFeetLocation(Vector3.Lerp(dash_origin, destination, inter));
            }

            CameraFadeEffect.EndEffect();                               // Reopen curtains
            yield return new WaitWhile(() => CameraFadeEffect.isBusy());//
            tp_in_progress = false;                                     // Reset flag
        }

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
                    dash_origin = AVR_PlayerRig.Instance.FeetInWorldSpace;
                    tp_in_progress = true;
                    dash_stime = Time.time;
                    break;
                }
                case TeleportModes.FADE_DASH : {
                    StartCoroutine(FadeDash());
                    break;
                }
                default : {
                    Debug.Log("Not implemented TeleportMode");
                    break;
                }
            }
        }

        void MoveRigToFeetLocation(Vector3 feet) {
            AVR_PlayerRig.Instance.MoveRigToFeetPosition(feet);
        }
    }
}
