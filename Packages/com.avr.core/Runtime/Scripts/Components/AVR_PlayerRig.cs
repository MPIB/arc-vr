using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVR.Core {
    /// <summary>
    /// Represents the players VR setup. Only one instance at a time is allowed.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___player_rig.html")]
    public class AVR_PlayerRig : AVR_SingletonComponent<AVR_PlayerRig>
    {
        /// <summary>
        /// Camera that represents the HMD
        /// </summary>
        public Camera MainCamera;

        /// <summary>
        /// CharacterController represeting the player. Null if no charactercontroller is present.
        /// </summary>
        public CharacterController characterController {
            get { return _characterController; }
        }
        private CharacterController _characterController;

        // -------------------------- RIG ----------------------------
        /// <summary>
        /// Player Rig location in world space
        /// </summary>
        public Vector3 RigInWorldSpace {
            get { return transform.position; }
        }

        /// <summary>
        /// Player camera in world space
        /// </summary>
        public Vector3 CameraInWorldSpace {
            get { return MainCamera.transform.position; }
        }

        /// <summary>
        /// View-vector in local coordinates relative to the camera
        /// </summary>
        public Vector3 LocalViewVector {
            get { return MainCamera.transform.InverseTransformDirection(MainCamera.transform.forward); }
        }

        /// <summary>
        /// View-vector in world space
        /// </summary>
        public Vector3 ViewVector {
            get { return MainCamera.transform.forward; }
        }

        /// <summary>
        /// Position of the cameras view-vector tip in world space
        /// </summary>
        public Vector3 ViewPos
        {
            get { return MainCamera.transform.position + MainCamera.transform.forward; }
        }

        /// <summary>
        /// Player camera in local space (relative to the AVR_PlayerRig object)
        /// </summary>
        public Vector3 CameraInRigSpace {
            get { return transform.InverseTransformPoint(MainCamera.transform.position); }
        }

        /// <summary>
        /// Height of the camera in local space. Equivalent to the distance between HMD from the gorund.
        /// </summary>
        public float CameraHeightInRigSpace {
            get { return MainCamera.transform.position.y - RigInWorldSpace.y; }
        }

        /// <summary>
        /// Position on the gorund directly underneath the camera. (World space)
        /// </summary>
        public Vector3 FeetInWorldSpace {
            get { return CameraInWorldSpace - transform.up*CameraHeightInRigSpace; }
        }

        /// <summary>
        /// Position on the gorund directly underneath the camera. (local space, relative to AVR_PlayerRig object)
        /// </summary>
        public Vector3 FeetInRigSpace {
            get { return transform.InverseTransformPoint(FeetInWorldSpace); }
        }

        /// <summary>
        /// Instantly moves the PlayerRig so that the players feet (anchor) end up at world coordinates 'pos'
        /// </summary>
        public void MoveRigToFeetPosition(Vector3 pos) {
            transform.position = (pos - FeetInRigSpace);
        }

        /// <summary>
        /// Instantly moves the PlayerRig to a given location
        /// </summary>
        public void MoveRigToPosition(Vector3 pos) {
            transform.position = pos;
        }

        /// <summary>
        /// Instantly moves the PlayerRig so that the camera ends up at world coordinates 'pos'
        /// </summary>
        public void MoveRigToCameraPosition(Vector3 pos) {
            transform.position += (pos - CameraInWorldSpace);
        }

        /// <summary>
        /// Returns true if the player is crouching / crawling. Uses the "/settings/calibration/player_height" setting to determine this.
        /// </summary>
        public bool isCrouching()
        {
            float val1 = CameraHeightInRigSpace;
            float hf = AVR_Settings.get_float("/settings/calibration/player_height");

            if (leftHandController && rightHandController)
            {
                float val2 = 0.5f * ((leftHandController.transform.position.y - transform.position.y) + (rightHandController.transform.position.y - transform.position.y));
                return val1 < 0.72f * hf || val2 < 0.36f * hf;
            }
            return val1 < hf;
        }

        /// <summary>
        /// Returns true if the player is leaning forwards / bowing.
        /// </summary>
        public bool isLeaningForwards()
        {
            return MainCamera.transform.rotation.eulerAngles.x > 55.0f && MainCamera.transform.rotation.eulerAngles.x < 120.0f;
        }

        /// <summary>
        /// Returns a confidence value of how much the player is leaning forwards / bowing.
        /// </summary>
        public float isLeaningForwardsConfidence()
        {
            return Mathf.Clamp((MainCamera.transform.rotation.eulerAngles.x - 40.0f) / 30.0f, 0.0f, 1.0f);
        }

        /// <summary>
        /// Returns the AVR_Controller that represents the left hand controller (if it exists). This value depends on the controllerNode value and is not updated if this one is changed during runtime.
        /// </summary>
        public AVR_Controller leftHandController => AVR_Controller.leftHandController;

        /// <summary>
        /// Returns the AVR_Controller that represents the right hand controller (if it exists). This value depends on the controllerNode value and is not updated if this one is changed during runtime.
        /// </summary>
        public AVR_Controller rightHandController => AVR_Controller.rightHandController;

        protected override void Start() {
            base.Start();

            _characterController = GetComponent<CharacterController>();
            if (characterController)
            {
                AVR_DevConsole.print(gameObject.name + " got CharacterController.");
            }

            if(!MainCamera) MainCamera = Camera.main;
            if(!MainCamera) AVR_DevConsole.cerror("The MainCamera property is not set and there is no Camera.main component in the scene!", this);
        }
        
        protected void Update() {
            if(characterController) {
                // Set charactercontroller height based on camera height
                characterController.height = Mathf.Clamp(AVR_PlayerRig.Instance.CameraHeightInRigSpace, AVR_Settings.get_float("/settings/core/charactercontroller/min_height"), AVR_Settings.get_float("/settings/core/charactercontroller/max_height"));

                // Set charactercontroller center to camera position in rig space. NOTE: This assumes that this.transform.pos is
                // the same as rig.position. Eg. transform.localPosition = Vector3.zero.
                Vector3 center = CameraInRigSpace;
                center.y = characterController.skinWidth + (characterController.height * 0.5f);
                characterController.center = center;
            }
        }
    }
}
