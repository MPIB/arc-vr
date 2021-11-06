using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
#if UNITY_EDITOR
using UnityEditor;
#endif

using AVR.Core;

namespace AVR.Avatar {
    /// <summary>
    /// Estimates the pose of a player from the locations of VR controllers and HMD
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_avatar_1_1_a_v_r___pose_provider.html")]
    public class AVR_PoseProvider : AVR.Core.AVR_Component
    {
        /// <summary>
        /// Endpoint of the players view vector
        /// </summary>
        public Vector3 lookAtPos =>
            #if AVR_NET
            !IsOwner ? m_ReplicatedState.Value.lookAtPos :
            #endif
            eyeTransform.localPosition + transform.InverseTransformDirection(eyeTransform.forward);

        /// <summary>
        /// Left hand rotation
        /// </summary>
        public Quaternion leftHandRot =>
            #if AVR_NET
            !IsOwner ? m_ReplicatedState.Value.leftHandRot :
            #endif
            leftHandTarget.rotation;

        /// <summary>
        /// Right hand rotation
        /// </summary>
        public Quaternion rightHandRot => 
            #if AVR_NET
            !IsOwner ? m_ReplicatedState.Value.rightHandRot :
            #endif
            rightHandTarget.rotation;

        /// <summary>
        /// Left foot position
        /// </summary>
        public Vector3 leftFootPos => 
            #if AVR_NET
            !IsOwner ? m_ReplicatedState.Value.leftFootPos :
            #endif
            leftFootTarget.localPosition;

        /// <summary>
        /// Left foot rotation
        /// </summary>
        public Quaternion leftFootRot => 
            #if AVR_NET
            !IsOwner ? m_ReplicatedState.Value.leftFootRot :
            #endif
            leftFootTarget.rotation;

        /// <summary>
        /// Right foot position
        /// </summary>
        public Vector3 rightFootPos => 
            #if AVR_NET
            !IsOwner ? m_ReplicatedState.Value.rightFootPos :
            #endif
            rightFootTarget.localPosition;

        /// <summary>
        /// Right foot rotation
        /// </summary>
        public Quaternion rightFootRot => 
            #if AVR_NET
            !IsOwner ? m_ReplicatedState.Value.rightFootRot :
            #endif
            rightFootTarget.rotation;

        /// <summary>
        /// Position of the rig's pivot (Point on the ground directly underneath the HMD)
        /// </summary>
        public Vector3 pivotPos => 
            #if AVR_NET
            !IsOwner ? m_ReplicatedState.Value.pivotPos :
            #endif
            pivotTransform.localPosition;

        /// <summary>
        /// Rotation of the pivot
        /// </summary>
        public Quaternion pivotRot => 
            #if AVR_NET
            !IsOwner ? m_ReplicatedState.Value.pivotRot :
            #endif
            pivotTransform.rotation;

        /// <summary>
        /// Position of the body
        /// </summary>
        public Vector3 bodyPos => 
            #if AVR_NET
            !IsOwner ? m_ReplicatedState.Value.bodyPos :
            #endif
            bodyTransform.localPosition;

        /// <summary>
        /// Rotation of the body
        /// </summary>
        public Quaternion bodyRot => 
            #if AVR_NET
            !IsOwner ? m_ReplicatedState.Value.bodyRot :
            #endif
            bodyTransform.rotation;

        /// <summary>
        /// Position of the head
        /// </summary>
        public Vector3 eyePos => 
            #if AVR_NET
            !IsOwner ? m_ReplicatedState.Value.eyePos :
            #endif
            eyeTransform.localPosition;

        /// <summary>
        /// Rotation of the head
        /// </summary>
        public Quaternion eyeRot => 
            #if AVR_NET
            !IsOwner ? m_ReplicatedState.Value.eyeRot :
#endif
            eyeTransform.rotation;

        /// <summary>
        /// Left hand position
        /// </summary>
        public Vector3 leftHandPos {
            get
            {
                #if AVR_NET
                if(!IsOwner)
                {
                    return m_ReplicatedState.Value.leftHandPos;
                }
                #endif

                if (leftHandFilter) return transform.InverseTransformPoint(leftHandFilter.naturalize_point(leftHandTarget.position));
                return transform.InverseTransformPoint(leftHandTarget.position);
            }
        }

        /// <summary>
        /// Right hand position
        /// </summary>
        public Vector3 rightHandPos {
            get
            {
                #if AVR_NET
                if (!IsOwner)
                {
                    return m_ReplicatedState.Value.rightHandPos;
                }
                #endif

                if (rightHandFilter) return transform.InverseTransformPoint(rightHandFilter.naturalize_point(rightHandTarget.position));
                return transform.InverseTransformPoint(rightHandTarget.position);
            }
        }

        /// <summary>
        /// Left hand naturalization filter
        /// </summary>
        [AVR.Core.Attributes.FoldoutGroup("Filters")]
        public AVR_PoseNaturalizationFilter leftHandFilter;

        /// <summary>
        /// Right hand naturalization filter
        /// </summary>
        [AVR.Core.Attributes.FoldoutGroup("Filters")]
        public AVR_PoseNaturalizationFilter rightHandFilter;

        protected Transform leftHandTarget => playerRig.leftHandController ? playerRig.leftHandController.transform : pivotTransform;
        protected Transform rightHandTarget => playerRig.rightHandController ? playerRig.rightHandController.transform : pivotTransform;
        protected Transform leftFootTarget;
        protected Transform rightFootTarget;
        protected Transform pivotTransform;
        protected Transform bodyTransform;
        protected Transform eyeTransform;

        /// <summary>
        /// Inertia of the body. (Increasing this will help with micromovements and stutters of the body)
        /// </summary>
        [Range(0.0001f, 1.0f)]
        public float body_inertia = 0.3f;

        /// <summary>
        /// Blending speed between a leaning and standing position
        /// </summary>
        [AVR.Core.Attributes.FoldoutGroup("Calibration")]
        public float lean_blend_speed = 3.5f;

        /// <summary>
        /// Maximum yaw angle of the head in relation of the body
        /// </summary>
        [AVR.Core.Attributes.FoldoutGroup("Calibration")]
        public float max_head_yaw = 30f;

        /// <summary>
        /// Maximum pitch angle of the head in relation of the body
        /// </summary>
        [AVR.Core.Attributes.FoldoutGroup("Calibration")]
        public float max_head_pitch = 30f;

        /// <summary>
        /// Maximum roll angle of the head in relation of the body
        /// </summary>
        [AVR.Core.Attributes.FoldoutGroup("Calibration")]
        public float max_head_roll = 30f;

        /// <summary>
        /// Default height of the body/torso
        /// </summary>
        [AVR.Core.Attributes.FoldoutGroup("Calibration")]
        public float default_body_height = 0.9f;

        /// <summary>
        /// Maximum height of the body/torso fromt he ground
        /// </summary>
        [AVR.Core.Attributes.FoldoutGroup("Calibration")]
        public float max_body_height = 1.0f;

        /// <summary>
        /// Minimum height of the body/torso fromt he ground
        /// </summary>
        [AVR.Core.Attributes.FoldoutGroup("Calibration")]
        public float min_body_height = 0.5f;

        /// <summary>
        /// Local offset vector from eyes to neck
        /// </summary>
        [AVR.Core.Attributes.FoldoutGroup("Calibration")]
        public Vector3 local_eye_to_neck_offset = new Vector3(0.0f, -0.1f, -0.05f);

        /// <summary>
        /// Distance between neck and body/torso
        /// </summary>
        [AVR.Core.Attributes.FoldoutGroup("Calibration")]
        public float neck_body_distance = 0.4f;

        /// <summary>
        /// Distance between the right foot and the pivot
        /// </summary>
        [AVR.Core.Attributes.FoldoutGroup("Calibration")]
        public Vector3 foot_offset_from_pivot = new Vector3(0.2f, 0.05f, 0.0f);

        /// <summary>
        /// Speed at which feet move to their target position
        /// </summary>
        [AVR.Core.Attributes.FoldoutGroup("Calibration")]
        public float foot_follow_speed = 3.0f;

        /// <summary>
        /// Collision mask of the ground
        /// </summary>
        public LayerMask groundCollisionMask;

        private float lean_factor = 0.0f;
        private float lean_conf = 0.0f;
        private float last_yaw = 0.0f;

        protected override void Awake()
        {
            base.Awake();
            leftFootTarget = AVR.Core.Utils.Misc.CreateEmptyGameObject("leftFootTarget", transform);
            rightFootTarget = AVR.Core.Utils.Misc.CreateEmptyGameObject("rightFootTarget", transform);
            pivotTransform = AVR.Core.Utils.Misc.CreateEmptyGameObject("pivotTarget", transform);
            bodyTransform = AVR.Core.Utils.Misc.CreateEmptyGameObject("bodyTarget", transform);
            eyeTransform = AVR.Core.Utils.Misc.CreateEmptyGameObject("eyeTransform", transform);
        }

        Vector3 ApplyInertia(Vector3 current, Vector3 target) {
            float dist = Vector3.Distance(current, target);
            float move_mult = Mathf.SmoothStep(0f, 1f, dist / body_inertia);
            return Vector3.MoveTowards(current, target, dist*move_mult);
        }

        void SetBody() {
            // Pre-processing
            {
                // Calculation of lean_conf, which indicates our confidence of whether the palyer is leaning forwards or standing upright.
                if (playerRig.isLeaningForwardsConfidence() > 0.5f)
                {
                    lean_factor = Mathf.Lerp(lean_factor, 1.0f, Time.deltaTime * lean_blend_speed);
                }
                else
                {
                    lean_factor = Mathf.Lerp(lean_factor, 0.0f, Time.deltaTime * lean_blend_speed);
                }
                lean_conf = lean_factor * playerRig.isLeaningForwardsConfidence();


                // We apply inertia to the main cameras position, to avoid unnecessary micro-movement. Note: We do not apply this to the lookat-target, so the head rotation will be fully responsive.
                eyeTransform.position = ApplyInertia(eyeTransform.position, playerRig.MainCamera.transform.position);

                // Here we apply clamping bounds on the pitch and roll angles of the head. We deal with yaw in a separate function.
                //NOTE / TODO: This could lead to problems, as we use localRotation of the camera here. If, say, the camera was in a child object of the GenericXRDevice object, the local rotation would be zero.
                Quaternion r = playerRig.MainCamera.transform.localRotation;
                r = AVR.Core.Utils.Geom.ClampQuaternionRotation(r, new Vector3(max_head_pitch, 360, max_head_roll));
                eyeTransform.localRotation = r;
            }



            //No we get to the actual body transform. First we reset its rotation:
            bodyTransform.up = Vector3.up;

            // Get the rotation and position if the player is standing upright:
            GetStandingTransform(out Quaternion stout, out Vector3 stpos);
            // Get the rotation and position if the player is leaning forwards:
            GetLeanTransform(out Quaternion bdout, out Vector3 bdpos);

            // We designate an "unsafe position/rotation", which corresponds to our leaning transform, depending on our leaning-confidence value.
            Vector3 unsafe_pos = Vector3.Lerp(stpos, bdpos, lean_conf);
            Quaternion unsafe_rot = Quaternion.Lerp(stout, bdout, lean_conf);

            // The unsafe position corresponds to the position/rotation we believe the body has *based on our lean_conf value*.
            // PROBLEM: we may end up with the players feet hovering in the air. Here we correct for this.
            // We interpolate between our "unsafe" (the higher) position and the regular, standing-upright (lower) position, based on the distance of the body to the ground (pivot)
            {
                float sh = stpos.y - pivotTransform.position.y;     // safe height
                float uh = unsafe_pos.y - pivotTransform.position.y;// unsafe height

                float lamb = Mathf.Clamp((default_body_height - sh) / (uh - sh), 0.0f, 1.0f);

                unsafe_pos = Vector3.Lerp(stpos, unsafe_pos, lamb);
                unsafe_rot = Quaternion.Lerp(stout, unsafe_rot, lamb);
            }

            // Apply pos and rot to transform
            bodyTransform.position = unsafe_pos;
            bodyTransform.rotation = unsafe_rot;

            // Update pivot based on new body position
            UpdatePivot();

            // Clamp the distance top the ground to be sure
            bodyTransform.position = new Vector3(
                bodyTransform.position.x,
                pivotTransform.position.y + Mathf.Clamp(bodyTransform.position.y - pivotTransform.position.y, min_body_height, max_body_height),
                bodyTransform.position.z
            );
        }

        void Update()
        {
            #if AVR_NET
            if (!IsOwner) return;
            else if(synchronizePose)
            {
                PoseProviderState state;
                state.lookAtPos = lookAtPos;
                state.leftHandRot = leftHandRot;
                state.rightHandRot = rightHandRot;
                state.leftFootPos = leftFootPos;
                state.leftFootRot = leftFootRot;
                state.rightFootPos = rightFootPos;
                state.rightFootRot = rightFootRot;
                state.pivotPos = pivotPos;
                state.pivotRot = pivotRot;
                state.bodyPos = bodyPos;
                state.bodyRot = bodyRot;
                state.eyePos = eyePos;
                state.eyeRot = eyeRot;
                state.leftHandPos = leftHandPos;
                state.rightHandPos = rightHandPos;
                UpdateToClientsServerRpc(state);
            }
            #endif

            SetBody();

            // This is something we need in "CorrectBodyYawAngle".
            last_yaw = Mathf.MoveTowardsAngle(last_yaw, bodyTransform.rotation.eulerAngles.y, 9999.0f);

            UpdateLeftFoot();
            UpdateRightFoot();
        }

        [ServerRpc(RequireOwnership = false)]
        void UpdateToClientsServerRpc(PoseProviderState state)
        {
            m_ReplicatedState.Value = state;
        }

        void GetStandingTransform(out Quaternion rot, out Vector3 pos) {
            // Standing upright position is fairly simple. Just go to neck and straight down from there.
            Vector3 NeckPos = eyeTransform.position + eyeTransform.TransformVector(local_eye_to_neck_offset);

            pos = NeckPos + neck_body_distance * -Vector3.up;

            rot = CorrectBodyYawAngle(Quaternion.LookRotation(bodyTransform.forward, Vector3.up));
        }

        void GetLeanTransform(out Quaternion rot, out Vector3 pos) {
            //Lean position is the same as standing upright, but instead of straight down we go downwards by MainCamera.transform.down
            Vector3 NeckPos = eyeTransform.position + eyeTransform.TransformVector(local_eye_to_neck_offset);

            pos = NeckPos + neck_body_distance * -playerRig.MainCamera.transform.up;

            rot = CorrectBodyYawAngle(Quaternion.LookRotation(eyeTransform.forward, NeckPos - pos));
        }
        
        void OnDrawGizmos() {
            if(Application.isPlaying) {
                #if AVR_NET
                if (!IsOwner) return;
                #endif

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(eyeTransform.position, 0.1f);

                Gizmos.DrawRay(eyeTransform.position, eyeTransform.forward);

                Gizmos.color = Color.white;

                Vector3 local_eye_to_neck_offset = new Vector3(0.0f, -0.1f, -0.1f);
                Vector3 NeckPos = eyeTransform.position + eyeTransform.TransformVector(local_eye_to_neck_offset);
                Gizmos.DrawLine(eyeTransform.position, NeckPos);
                Gizmos.DrawCube(NeckPos, new Vector3(0.05f, 0.05f, 0.05f));

                Gizmos.DrawLine(NeckPos, bodyTransform.position);
                Gizmos.DrawCube(bodyTransform.position, new Vector3(0.05f, 0.05f, 0.05f));

                Gizmos.DrawCube(bodyTransform.position, new Vector3(0.2f, 0.2f, 0.2f));
                Gizmos.DrawCube(pivotTransform.position, new Vector3(0.05f, 0.05f, 0.05f));
                Gizmos.DrawRay(pivotTransform.position, pivotTransform.forward);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(bodyTransform.position, leftFootTarget.position);
                Gizmos.DrawCube(leftFootTarget.position, new Vector3(0.05f, 0.05f, 0.05f));
                Gizmos.DrawLine(bodyTransform.position, rightFootTarget.position);
                Gizmos.DrawCube(rightFootTarget.position, new Vector3(0.05f, 0.05f, 0.05f));

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(NeckPos, leftHandPos);
                Gizmos.DrawLine(NeckPos, rightHandPos);
            }
        }

        Quaternion CorrectBodyYawAngle(Quaternion rot) {
            float head_yaw = eyeTransform.localRotation.eulerAngles.y;
            float body_yaw = last_yaw;

            // Get the difference in yaw between body and eyes/head
            float yaw_diff = Mathf.DeltaAngle(head_yaw, body_yaw);

            // Account for max_head_yaw
            yaw_diff = Mathf.Sign(yaw_diff) * Mathf.Max(0, Mathf.Abs(yaw_diff) - max_head_yaw);

            // Calcualte the speed at which we'll adapt.
            float yaw_adapt_speed = Mathf.Abs(yaw_diff) * Time.deltaTime * 2.0f;

            // Adapt the rotation to the resulting yaw
            return Quaternion.Euler(
                bodyTransform.localRotation.eulerAngles.x,
                Mathf.MoveTowardsAngle(body_yaw, head_yaw, yaw_adapt_speed),
                bodyTransform.localRotation.eulerAngles.z
            );
        }

        void UpdateLeftFoot() {
            Vector3 thPos = pivotTransform.position + new Vector3(-foot_offset_from_pivot.x, foot_offset_from_pivot.y, foot_offset_from_pivot.z);

            leftFootTarget.forward = Vector3.Lerp(leftFootTarget.forward, pivotTransform.forward - 0.5f * pivotTransform.right, foot_follow_speed * Time.deltaTime);

            leftFootTarget.position = Vector3.Lerp(leftFootTarget.position, thPos, foot_follow_speed * Time.deltaTime);
        }

        void UpdateRightFoot() {
            Vector3 thPos = pivotTransform.position + foot_offset_from_pivot;

            rightFootTarget.forward = Vector3.Lerp(rightFootTarget.forward, pivotTransform.forward + 0.5f * pivotTransform.right, foot_follow_speed * Time.deltaTime);

            rightFootTarget.position = Vector3.Lerp(rightFootTarget.position, thPos, foot_follow_speed * Time.deltaTime);
        }

        void UpdatePivot() {
            Vector3 r_origin = bodyTransform.position;

            if (Physics.Raycast(r_origin, Vector3.down, out RaycastHit hit, 2.0f, groundCollisionMask))
            {
                pivotTransform.position = hit.point;
            }

            pivotTransform.forward = playerRig.XZPlaneFacingDirection;
        }

#if AVR_NET
        [HideInInspector]
        [AVR.Core.Attributes.ShowInNetPrompt]
        public bool synchronizePose = true;

        private readonly NetworkVariable<PoseProviderState> m_ReplicatedState = new NetworkVariable<PoseProviderState>(NetworkVariableReadPermission.Everyone, new PoseProviderState());

        internal struct PoseProviderState : INetworkSerializable
        {
            public Vector3 lookAtPos;
            public Quaternion leftHandRot;
            public Quaternion rightHandRot;
            public Vector3 leftFootPos;
            public Quaternion leftFootRot;
            public Vector3 rightFootPos;
            public Quaternion rightFootRot;
            public Vector3 pivotPos;
            public Quaternion pivotRot;
            public Vector3 bodyPos;
            public Quaternion bodyRot;
            public Vector3 eyePos;
            public Quaternion eyeRot;
            public Vector3 leftHandPos;
            public Vector3 rightHandPos;
           

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref lookAtPos);
                serializer.SerializeValue(ref leftHandRot);
                serializer.SerializeValue(ref rightHandRot);
                serializer.SerializeValue(ref leftFootPos);
                serializer.SerializeValue(ref leftFootRot);
                serializer.SerializeValue(ref rightFootPos);
                serializer.SerializeValue(ref rightFootRot);
                serializer.SerializeValue(ref pivotPos);
                serializer.SerializeValue(ref pivotRot);
                serializer.SerializeValue(ref bodyPos);
                serializer.SerializeValue(ref bodyRot);
                serializer.SerializeValue(ref eyePos);
                serializer.SerializeValue(ref eyeRot);
                serializer.SerializeValue(ref leftHandPos);
                serializer.SerializeValue(ref rightHandPos);
            }
        }
#endif
    }
}
