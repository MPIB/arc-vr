using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AVR.Avatar {
    [RequireComponent(typeof(Animator))]
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_avatar_1_1_a_v_r___pose_assumer.html")]
    public class AVR_PoseAssumer : AVR.Core.AVR_Component
    {
        public AVR_PoseProvider provider;

        public Transform headTransform;
        public Transform neckTransform;
        public bool switchAxisXZ = true;

        private Animator animator;
        private bool IKPass_is_enabled = false;

        public bool autoLayerBlend = false;
        [AVR.Core.Attributes.ConditionalHideInInspector("autoLayerBlend", true)]
        public string speedAnimationParameter = "Speed";
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
            {
                animator.SetIKPosition(AvatarIKGoal.LeftFoot, transform.TransformPoint(provider.leftFootPos));
                animator.SetIKRotation(AvatarIKGoal.LeftFoot, transform.rotation * provider.leftFootRot);
            }
            {
                animator.SetIKPosition(AvatarIKGoal.RightFoot, transform.TransformPoint(provider.rightFootPos));
                animator.SetIKRotation(AvatarIKGoal.RightFoot, transform.rotation * provider.rightFootRot);
            }
        }

        void OnAnimatorIK(int layerIndex)
        {
            IKPass_is_enabled = true;

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