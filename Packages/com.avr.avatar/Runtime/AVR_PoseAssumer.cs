using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AVR.Avatar {
    /// <summary>
    /// Applies a pose provided by an AVR_PosePorvider to a rigged 3d model with an avatar.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_avatar_1_1_a_v_r___pose_assumer.html")]
    public class AVR_PoseAssumer : AVR.Core.AVR_Component
    {
        /// <summary>
        /// PoseProvider to get pose information from
        /// </summary>
        public AVR_PoseProvider provider;

        /// <summary>
        /// 3d models' transform of the head
        /// </summary>
        public Transform headTransform;

        /// <summary>
        /// 3d models' transform of the neck
        /// </summary>
        public Transform neckTransform;

        /// <summary>
        /// Swap the X and Y axes of the head and neck. (If the head is inverted on your avatar, toggle this)
        /// </summary>
        public bool switchAxisXZ = true;

        /// <summary>
        /// Swap left and right feet targets. (If the avatar consistently has crossed legs, toggle this)
        /// </summary>
        public bool swap_feet = false;

        private Animator animator;
        private bool IKPass_is_enabled = false;

        /// <summary>
        /// If true, we automatically blend the weight of the IK layer to inverse proportion of the playerRigs' speed
        /// </summary>
        public bool autoLayerBlend = false;

        /// <summary>
        /// The animation parameter in the animation controller we set to the playerrigs speed when the rig is teleporting
        /// </summary>
        [AVR.Core.Attributes.ConditionalHideInInspector("autoLayerBlend", true)]
        public string speedAnimationParameter = "Speed";

        /// <summary>
        /// Speed of how quickly we blend in and out of the IK layer (if autoLayerBlend is true)
        /// </summary>
        [AVR.Core.Attributes.ConditionalHideInInspector("autoLayerBlend", true)]
        public float layerBlend_speed = 5.0f;
        private float layerBlend = 1.0f;

        [AVR.Core.Attributes.FoldoutGroup("Weights")]
        public float lookAtWeight_body = 0.0f;
        [AVR.Core.Attributes.FoldoutGroup("Weights")]
        public float lookAtWeight_head = 1.0f;
        [AVR.Core.Attributes.FoldoutGroup("Weights")]
        public float lookAtWeight_eyes = 1.0f;
        [AVR.Core.Attributes.FoldoutGroup("Weights")]
        public float lookAtWeight_clamp = 0.0f;
        [AVR.Core.Attributes.FoldoutGroup("Weights")]
        public float lookAtWeight = 1.0f;
        [AVR.Core.Attributes.FoldoutGroup("Weights")]
        public float HandPosWeight = 1.0f;
        [AVR.Core.Attributes.FoldoutGroup("Weights")]
        public float HandRotWeight = 0.5f;
        [AVR.Core.Attributes.FoldoutGroup("Weights")]
        public float FootPosWeight = 1.0f;
        [AVR.Core.Attributes.FoldoutGroup("Weights")]
        public float FootRotWeight = 1.0f;

        protected override void Start() {
            base.Start();
            animator = GetComponent<Animator>();
            if(!animator.avatar) {
                AVR.Core.AVR_DevConsole.warn("AVR_PoseAssumer requires an Animator component WITH an Avatar set. " + animator.name + " has none.");
            }
            Invoke("CheckIKPass", 3.0f);

            animator.logWarnings = false; //TODO: This disables warning-spam if parameters (like "Speed") dont exist. Perhaps this should be optional based on a setting.
        }

        void CheckIKPass() {
            if(!IKPass_is_enabled) {
                AVR.Core.AVR_DevConsole.warn("The OnAnimatorIK() method of AVR_PoseAssumer " + gameObject.name + " is not being called. Does the respective Animator have IKPass enabled on any layers?");
            }
        }

        void LateUpdate() {
            if(!headTransform || !neckTransform) {
                AVR.Core.AVR_DevConsole.cwarn("headTransform and neckTransform are not set!", this);
                return;
            }
            // Head linkage
            Vector3 headAng = headTransform.eulerAngles;
            Vector3 neckAng = neckTransform.eulerAngles;
            float ang = Mathf.DeltaAngle(360.0f, provider.eyeRot.eulerAngles.z);
            if (switchAxisXZ)
            {
                headAng.x = ang;
                neckAng.x = ang * 0.5f;
            }
            else
            {
                headAng.z = ang;
                neckAng.z = ang * 0.5f;
            }
            headTransform.eulerAngles = headAng;
            neckTransform.eulerAngles = neckAng;
        }

        void Update() {
            #if AVR_NET
            if (!playerRig) return; // Sometimes this happens on MP for 1 frame.
            #endif


            if(autoLayerBlend) {
                layerBlend = Mathf.Lerp(layerBlend, Mathf.SmoothStep(1.0f, 0.0f, playerRig.AvgMotion.magnitude), layerBlend_speed * Time.deltaTime);
            }
        }

        protected void setWeights(int layerIndex) {
            float layerWeight = animator.GetLayerWeight(layerIndex);

            animator.SetLookAtWeight(lookAtWeight*layerWeight, lookAtWeight_body*layerWeight, lookAtWeight_head*layerWeight, lookAtWeight_eyes*layerWeight, lookAtWeight_clamp*layerWeight);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, HandPosWeight*layerWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, HandRotWeight*layerWeight);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, HandPosWeight*layerWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, HandRotWeight*layerWeight);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, FootPosWeight*layerWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, FootRotWeight*layerWeight);
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, FootPosWeight*layerWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, FootRotWeight*layerWeight);
        }

        protected void setPosRot(int layerIndex) {
            float layerWeight = animator.GetLayerWeight(layerIndex);

            {
                animator.SetLookAtPosition(transform.TransformPoint(provider.lookAtPos));
            }
            {
                animator.bodyPosition = transform.TransformPoint(provider.bodyPos);
                animator.bodyRotation = transform.rotation * provider.bodyRot;
            }
            if(playerRig.leftHandController!=null)
            {
                animator.SetIKPosition(AvatarIKGoal.LeftHand, transform.TransformPoint(provider.leftHandPos));
                animator.SetIKRotation(AvatarIKGoal.LeftHand, transform.rotation * provider.leftHandRot);
            }
            if(playerRig.rightHandController != null)
            {
                animator.SetIKPosition(AvatarIKGoal.RightHand, transform.TransformPoint(provider.rightHandPos));
                animator.SetIKRotation(AvatarIKGoal.RightHand, transform.rotation * provider.rightHandRot);
            }
            if(!swap_feet)
            {
                animator.SetIKPosition(AvatarIKGoal.LeftFoot, transform.TransformPoint(provider.leftFootPos));
                animator.SetIKRotation(AvatarIKGoal.LeftFoot, transform.rotation * provider.leftFootRot);
                animator.SetIKPosition(AvatarIKGoal.RightFoot, transform.TransformPoint(provider.rightFootPos));
                animator.SetIKRotation(AvatarIKGoal.RightFoot, transform.rotation * provider.rightFootRot);
            }
            else {
                animator.SetIKPosition(AvatarIKGoal.LeftFoot, transform.TransformPoint(provider.rightFootPos));
                animator.SetIKRotation(AvatarIKGoal.LeftFoot, transform.rotation * provider.rightFootRot);
                animator.SetIKPosition(AvatarIKGoal.RightFoot, transform.TransformPoint(provider.leftFootPos));
                animator.SetIKRotation(AvatarIKGoal.RightFoot, transform.rotation * provider.leftFootRot);
            }
        }

        void OnAnimatorIK(int layerIndex)
        {
            IKPass_is_enabled = true;

            #if AVR_NET
            if (!playerRig) return; // Sometimes this happens on MP for one frame.
            #endif

            setPosRot(layerIndex);

            // This paragraph handles everything regarding basic movement animations and stuff
            if(autoLayerBlend) {   
                animator.SetFloat(speedAnimationParameter, playerRig.AvgMotion.magnitude);
                animator.SetLayerWeight(layerIndex, layerBlend);
                Vector3 defaultBodyPos = transform.TransformPoint(provider.pivotPos + Vector3.up * 1.0f);
                animator.bodyPosition = Vector3.Lerp(defaultBodyPos, animator.bodyPosition, layerBlend);
                animator.bodyRotation = Quaternion.Lerp(
                    // The 0.0001*forward is to avoid quaternion zero-look errors. These don't actually break anything, but they do spam the debug output.
                    transform.rotation * Quaternion.LookRotation(playerRig.AvgMotion + 0.0001f*Vector3.forward, Vector3.up),
                    animator.bodyRotation,
                    layerBlend
                );
            }

            setWeights(layerIndex);
        }
    }
}