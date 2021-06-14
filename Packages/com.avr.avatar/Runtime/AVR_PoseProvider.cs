using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AVR.Avatar {
    public class AVR_PoseProvider : AVR.Core.AVR_Component
    {
        [AVR.Core.Attributes.FoldoutGroup("Targets")]
        public Transform lookAtTarget;
        [AVR.Core.Attributes.FoldoutGroup("Targets")]
        public Transform eyeTransform;
        [AVR.Core.Attributes.FoldoutGroup("Targets")]
        public Transform leftHandTarget;
        [AVR.Core.Attributes.FoldoutGroup("Targets")]
        public Transform rightHandTarget;
        [AVR.Core.Attributes.FoldoutGroup("Targets")]
        public Transform leftFootTarget;
        [AVR.Core.Attributes.FoldoutGroup("Targets")]
        public Transform rightFootTarget;
        [AVR.Core.Attributes.FoldoutGroup("Targets")]
        public Transform pivotTransform;


        public float offsetY = 0.5f;
        public float offsetZ = 0.0f;
        public float maxTorsionAngle = 45.0f;
        public float armLength = 0.4f;
        public LayerMask collisionMask;

        public Transform bodyTransform;

        public float foot_offset_from_pivot;
        public float foot_spring_dist;
        public float foot_follow_speed;

        [InitializeOnLoadMethod]
        private void init_in_editor() {
            lookAtTarget = Camera.main.transform.GetChild(0);
            eyeTransform = Camera.main.transform;
            leftHandTarget = ((GameObject)AVR.Core.Utils.Misc.GlobalFind("LeftHandController", typeof(GameObject))).transform;
            rightHandTarget = ((GameObject)AVR.Core.Utils.Misc.GlobalFind("RightHandController", typeof(GameObject))).transform;
        }

        void Update()
        {
            UpdatePivot();
            SetBodyPosition();
            SetBodyRotation();
        }

        void UpdateLeftFoot() {
            Vector3 thPos = pivotTransform.position - pivotTransform.right * foot_offset_from_pivot;

            if (Vector3.Distance(thPos, leftFootTarget.position) > foot_spring_dist)
            {
                leftFootTarget.forward = Vector3.Lerp(leftFootTarget.forward, pivotTransform.forward, foot_follow_speed * Time.deltaTime);

                leftFootTarget.position = Vector3.Lerp(leftFootTarget.position, thPos, foot_follow_speed * Time.deltaTime);
            }
        }

        void UpdateRightFoot() {
            Vector3 thPos = pivotTransform.position + pivotTransform.right * foot_offset_from_pivot;

            if (Vector3.Distance(thPos, rightFootTarget.position) > foot_spring_dist)
            {
                rightFootTarget.forward = Vector3.Lerp(rightFootTarget.forward, pivotTransform.forward, foot_follow_speed * Time.deltaTime);

                rightFootTarget.position = Vector3.Lerp(rightFootTarget.position, thPos, foot_follow_speed * Time.deltaTime);
            }
        }

        void UpdatePivot() {
            Vector3 r_origin = eyeTransform.position;

            if (Physics.Raycast(r_origin, Vector3.down, out RaycastHit hit, 3.0f, collisionMask))
            {
                pivotTransform.position = hit.point;
            }

            pivotTransform.forward = eyeTransform.forward;
        }

        void SetBodyPosition() {
            Vector3 bodyPos = bodyTransform.position;
            // Linked with eye position.
            bodyPos.y = eyeTransform.position.y - offsetY;
            // Linked with "IK_Pivot" position.
            bodyPos.x = pivotTransform.position.x;
            bodyPos.z = pivotTransform.position.z;
            // Set position.
            bodyTransform.position = bodyPos;
        }

        void SetBodyRotation ()
        {
            // LEFT HAND:
            // Get pivot
            Transform pivot = pivotTransform;
            // Get the location of the hand relative to the pivot
            Vector3 l_hand_local = pivot.InverseTransformPoint(leftHandTarget.position);
            // Angle between pivot.left and left hand (trigonometry)
            float l_hand_angle = l_hand_local.x / armLength;
            // ???
            l_hand_angle = Mathf.LerpAngle(0.0f, maxTorsionAngle, l_hand_angle);

            // RIGHT HAND: (Repeat same steps)
            float r_hand_angle = Mathf.LerpAngle (0.0f, -maxTorsionAngle, -pivotTransform.InverseTransformPoint (rightHandTarget.position).x / armLength);

            // ???
            float handLinkageAng = l_hand_angle + r_hand_angle;

            // HEAD
            // body relative to pivot
            Vector3 thisLocalPos = pivotTransform.InverseTransformPoint (bodyTransform.position);
            // eyes realtive to pivot
            Vector3 eyeLocalPos = pivotTransform.InverseTransformPoint (eyeTransform.position);

            float deltaY = eyeLocalPos.y - thisLocalPos.y;
            float deltaX = eyeLocalPos.x - thisLocalPos.x;
            float deltaZ = eyeLocalPos.z - thisLocalPos.z - offsetZ;

            // From trigonometry we know that tan(angX)=deltaY/deltaX, so this way we calculate angX.
            float angX = Mathf.Atan2 (deltaY, deltaZ);
            float angZ = Mathf.Atan2 (deltaY, deltaX);

            // We transform into degs and subtract 90
            angX = angX * Mathf.Rad2Deg - 90.0f;
            angZ = angZ * Mathf.Rad2Deg - 90.0f;

            // ???
            float headLinkageAng = Mathf.DeltaAngle (0.0f, eyeTransform.localEulerAngles.y) * 0.25f;

            // Set rotation.
            Vector3 bodyAng = Vector3.zero;
            bodyAng.x = -angX;
            bodyAng.y = pivotTransform.localEulerAngles.y + handLinkageAng + headLinkageAng;
            bodyAng.z = angZ;
            bodyTransform.localEulerAngles = bodyAng;
        }
    }
}
