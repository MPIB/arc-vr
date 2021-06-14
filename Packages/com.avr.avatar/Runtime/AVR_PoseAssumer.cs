using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AVR.Avatar {
    [RequireComponent(typeof(Animator))]
    public class AVR_PoseAssumer : AVR.Core.AVR_Component
    {
        public AVR_PoseProvider provider;

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


        public Transform headTransform;
        public Transform neckTransform;
        public bool switchAxisXZ = true;

        private Animator animator;
        private bool IKPass_is_enabled = false;

        private Vector3 lastRigPos = Vector3.zero;
        private Vector3 momentum = Vector3.zero;

        private NaturalizationFilter filter;
        public AVR.Core.AVR_ControllerInputManager inp;
        private bool fil = true;

        protected override void Start() {
            base.Start();
            animator = GetComponent<Animator>();
            if(!animator.avatar) {
                AVR.Core.AVR_DevConsole.warn("AVR_PoseAssumer requires an Animator component WITH an Avatar set. " + animator.name + " has none.");
            }
            Invoke("CheckIKPass", 3.0f);

            filter = new NaturalizationFilter(30, 3);
        }

        void CheckIKPass() {
            if(!IKPass_is_enabled) {
                AVR.Core.AVR_DevConsole.warn("The OnAnimatorIK() method of AVR_PoseAssumer " + gameObject.name + " is not being called. Does the respective Animator have IKPass enabled on any layers?");
            }
        }

        void LateUpdate() {
            // Head linkage
            Vector3 head_angle = headTransform.localRotation.eulerAngles;
            
            head_angle = new Vector3(head_angle.x%360.0f, head_angle.y%360.0f, head_angle.z%360.0f);
            head_angle = new Vector3(Mathf.Clamp(head_angle.x, -160, 115), Mathf.Clamp(head_angle.y, -160, 160), head_angle.z);

            //headTransform.localRotation = Quaternion.Euler(head_angle);
            headTransform.LookAt(provider.leftFootTarget.position);

            return;
            Vector3 headAng = headTransform.eulerAngles;
            Vector3 neckAng = neckTransform.eulerAngles;
            float ang = Mathf.DeltaAngle(360.0f, provider.eyeTransform.eulerAngles.z);
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
            const float framesmoothing = 0.1f;
            momentum = Vector3.Lerp(momentum, (AVR.Core.AVR_PlayerRig.Instance.RigInWorldSpace - lastRigPos) / Time.deltaTime, framesmoothing);
            lastRigPos = AVR.Core.AVR_PlayerRig.Instance.RigInWorldSpace;

            if(inp.triggerDown) fil = !fil;
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

            if (provider.lookAtTarget != null)
            {
                animator.SetLookAtPosition(provider.lookAtTarget.position);
            }
            if (provider.bodyTransform != null)
            {
                Vector3 defaultBodyPosition = AVR.Core.AVR_PlayerRig.Instance.FeetInWorldSpace + Vector3.up * 1.0f;
                animator.bodyPosition = Vector3.Lerp(defaultBodyPosition, provider.bodyTransform.position, layerWeight);
            }
            if (provider.leftHandTarget != null)
            {
                animator.SetIKPosition(AvatarIKGoal.LeftHand, provider.leftHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, provider.leftHandTarget.rotation);
            }
            if (provider.rightHandTarget != null)
            {
                if(fil) animator.SetIKPosition(AvatarIKGoal.LeftHand, filter.naturalize_point(provider.rightHandTarget.position));
                else if (!fil) animator.SetIKPosition(AvatarIKGoal.LeftHand, provider.rightHandTarget.position);
                //animator.SetIKPosition(AvatarIKGoal.RightHand, provider.rightHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, provider.rightHandTarget.rotation);
            }
            if (provider.leftFootTarget != null)
            {
                animator.SetIKPosition(AvatarIKGoal.LeftFoot, provider.leftFootTarget.position + Vector3.up * 0.1f); //TODO: NOTE: we cheat a bit here to account for the offset of the foot bone
                animator.SetIKRotation(AvatarIKGoal.LeftFoot, provider.leftFootTarget.rotation);
            }
            if (provider.rightFootTarget != null)
            {
                animator.SetIKPosition(AvatarIKGoal.RightFoot, provider.rightFootTarget.position + Vector3.up * 0.1f); //TODO: NOTE: we cheat a bit here to account for the offset of the foot bone
                animator.SetIKRotation(AvatarIKGoal.RightFoot, provider.rightFootTarget.rotation);
            }
        }

        void OnAnimatorIK(int layerIndex)
        {
            IKPass_is_enabled = true;

            // This paragraph handles everything regarding basic movement animations and stuff
            transform.LookAt(transform.position + momentum, Vector3.up);
            animator.SetFloat("Speed", momentum.magnitude);
            animator.SetLayerWeight(layerIndex, 1.0f - Mathf.Clamp(momentum.magnitude, 0.5f, 1.0f));

            setWeights(layerIndex);
            setPosRot(layerIndex);
        }
    }
}