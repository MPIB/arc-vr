﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if AVR_NET
using Unity.Netcode;
#endif

namespace AVR.Core {
    /// <summary>
    /// Represents the players VR setup. Only one instance at a time is allowed.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___player_rig.html")]
    public class AVR_PlayerRig : AVR_SingletonComponent<AVR_PlayerRig>
    {
        private Vector3 _RigPosInWorldSpace =>
#if AVR_NET
            !IsOwner? m_ReplicatedState.Value.rigPos :
#endif
            this.transform.position;

        private Quaternion _RigRotInWorldSpace =>
#if AVR_NET
            !IsOwner ? m_ReplicatedState.Value.rigRot :
#endif
            this.transform.rotation;

        private Vector3 _CameraPosInWorldSpace =>
#if AVR_NET
            !IsOwner ? m_ReplicatedState.Value.camPos :
#endif
            this.MainCamera.transform.position;

        private Quaternion _CameraRotInWorldSpace =>
#if AVR_NET
            !IsOwner ? m_ReplicatedState.Value.camRot :
#endif
            this.MainCamera.transform.rotation;

        /// <summary>
        /// Average motion of the players feet over the last 0.5s.
        /// </summary>
        public Vector3 AvgMotion {
            get { return _motion; }
        }
        private Vector3 _motion = Vector3.zero;
        private Vector3 _lastFPos;

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
            get { return _RigPosInWorldSpace; }
        }

        /// <summary>
        /// Player rig rotation in world space
        /// </summary>
        public Quaternion RigRotationInWorldSpace
        {
            get { return _RigRotInWorldSpace; }
        }

        /// <summary>
        /// Player camera in world space
        /// </summary>
        public Vector3 CameraInWorldSpace {
            get { return _CameraPosInWorldSpace; }
        }

        /// <summary>
        /// Camera rotation in world space
        /// </summary>
        public Quaternion CameraRotationInWorldSpace
        {
            get { return _CameraRotInWorldSpace; }
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
        /// Returns a vector in the XZ plane that corresponds to the direction the player is facing with his body.
        /// This is NOT equivalent with the facing direction of the camera. (For instance when the player bows or leans backward.)
        /// </summary>
        public Vector3 XZPlaneFacingDirection {
            get { Vector3 tf = Vector3.Cross(MainCamera.transform.right, Vector3.up); tf.y = 0; return tf; }
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
            get { return CameraInWorldSpace.y - RigInWorldSpace.y; }
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
        public void MoveRigToFeetPosition(Vector3 pos, bool affectMotion=true) {
            transform.position = (pos + (RigInWorldSpace - FeetInWorldSpace));
            if(!affectMotion) _lastFPos = pos;
        }

        /// <summary>
        /// Instantly moves the PlayerRig to a given location
        /// </summary>
        public void MoveRigToPosition(Vector3 pos, bool affectMotion=true) {
            transform.position = pos;
            if(!affectMotion) _lastFPos = FeetInWorldSpace;
        }

        /// <summary>
        /// Instantly moves the PlayerRig so that the camera ends up at world coordinates 'pos'
        /// </summary>
        public void MoveRigToCameraPosition(Vector3 pos, bool affectMotion=true) {
            transform.position += (pos - CameraInWorldSpace);
            if(!affectMotion) _lastFPos = FeetInWorldSpace;
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

        protected void LateUpdate() {
            Vector3.SmoothDamp(_motion, (FeetInWorldSpace - _lastFPos) / Time.deltaTime, ref _motion, 0.5f);
            _lastFPos = FeetInWorldSpace;
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

#if AVR_NET
        [HideInInspector]
        [AVR.Core.Attributes.ShowInNetPrompt]
        public bool synchronizeRigPositions = false;

        [ServerRpc(RequireOwnership = false)]
        private void syncServerRpc(InternalState state)
        {
            m_ReplicatedState.Value = state;
        }

        private void sync()
        {
            if (IsOwner)
            {
                InternalState state = new InternalState();
                state.FromReference(this);
            }
            else
            {
                m_ReplicatedState.Value.ApplyState(this);
            }
        }

        private readonly NetworkVariable<InternalState> m_ReplicatedState = new NetworkVariable<InternalState>(NetworkVariableReadPermission.Everyone, new InternalState());

        private struct InternalState : IInternalState<AVR_PlayerRig>
        {
            public Vector3 rigPos;
            public Quaternion rigRot;
            public Vector3 camPos;
            public Quaternion camRot;

            public void FromReference(AVR_PlayerRig reference)
            {
                rigPos = reference.RigInWorldSpace;
                rigRot = reference.RigRotationInWorldSpace;
                camPos = reference.CameraInWorldSpace;
                camRot = reference.CameraRotationInWorldSpace;
            }

            public void ApplyState(AVR_PlayerRig reference)
            {
                
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref rigPos);
                serializer.SerializeValue(ref rigRot);
                serializer.SerializeValue(ref camPos);
                serializer.SerializeValue(ref camRot);
            }
        }
#endif
    }
}
