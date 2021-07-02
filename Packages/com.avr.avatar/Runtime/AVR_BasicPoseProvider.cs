using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVR.Avatar
{
    public class AVR_BasicPoseProvider : AVR.Core.AVR_Component
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

        private float lean_blend = 0.0f;

        void Update()
        {
            //UpdatePivot();
            //SetBodyPosition();
            //SetBodyRotation();
            //TakeTwo();
            //TakeThree();
            TakeFour();
        }

        void SetBody()
        {
            bodyTransform.position = eyeTransform.position - Vector3.up * offsetY;
            //bodyTransform.up = Vector3.up;

            const float lean_blend_speed = 1.8f;

            if (true)//(isLeaningForwards())
            {
                lean_blend = Mathf.Clamp(lean_blend + Time.deltaTime * lean_blend_speed, 0, 1);
            }
            else
            {
                lean_blend = Mathf.Clamp(lean_blend - Time.deltaTime * lean_blend_speed, 0, 1);
            }

            float interp = Mathf.Sqrt(lean_blend);

            bodyTransform.position = Vector3.Lerp(bodyTransform.position, eyeTransform.position - eyeTransform.up * offsetY, interp);

            return;

            //bodyTransform.rotation = Quaternion.Euler(bodyTransform.rotation.x, eyeTransform.rotation.y, bodyTransform.rotation.z);

            //bodyTransform.up = Vector3.Lerp(bodyTransform.up, eyeTransform.position - bodyTransform.position, interp);



            // TODO: deal with right == up case, or forwardDir = (0,1,0)
            Vector3 forwardDir = Vector3.Cross(eyeTransform.right, Vector3.up);
            forwardDir.y = 0;
            forwardDir.Normalize();

            bodyTransform.forward = Vector3.Lerp(forwardDir, eyeTransform.forward, interp);
            //bodyTransform.rotation = Quaternion.Euler(eyeTransform.rotation.eulerAngles.x, bodyTransform.rotation.y, bodyTransform.rotation.z);


            //bodyTransform.rotation = Quaternion.AngleAxis(Mathf.Sin(forwardDir.z), Vector3.up);
            //bodyTransform.Rotate(new Vector3(0, bodyTransform.rotation.eulerAngles.y, 0), Space.World);
            //bodyTransform.Rotate(new Vector3(0, Mathf.Sin(forwardDir.x)*Mathf.Rad2Deg, 0), Space.World);
            //bodyTransform.Rotate(new Vector3(0, Time.time*10, 0), Space.Self);


            //bodyTransform.localRotation = Quaternion.Euler(bodyTransform.localRotation.x, eyeTransform.localRotation.y, bodyTransform.localRotation.z);
            //bodyTransform.forward = eyeTransform.forward;
        }

        void TakeFour()
        {
            if (AVR.Core.AVR_PlayerRig.Instance.isLeaningForwards())
            {
                bend_conf = Mathf.Lerp(bend_conf, 1.0f, Time.deltaTime * 5.0f);
            }
            else
            {
                bend_conf = Mathf.Lerp(bend_conf, 0.0f, Time.deltaTime * 5.0f);
            }

            bend_conf *= AVR.Core.AVR_PlayerRig.Instance.isLeaningForwardsConfidence();



            // Reset pos and rot:
            bodyTransform.up = Vector3.up;

            GetStandingTransform(out Quaternion stout, out Vector3 stpos);
            GetBendTransform(out Quaternion bdout, out Vector3 bdpos);

            Vector3 unsafe_pos = Vector3.Lerp(stpos, bdpos, bend_conf);
            Quaternion unsafe_rot = Quaternion.Lerp(stout, bdout, bend_conf);

            {
                float val = 1.0f;
                float sh = stpos.y - pivotTransform.position.y;
                float uh = unsafe_pos.y - pivotTransform.position.y;

                float lamb = Mathf.Clamp((val - sh) / (uh - sh), 0.0f, 1.0f);
                Debug.Log(lamb + " : " + bend_conf);

                unsafe_pos = Vector3.Lerp(stpos, unsafe_pos, lamb);
                unsafe_rot = Quaternion.Lerp(stout, unsafe_rot, lamb);
            }

            bodyTransform.position = unsafe_pos;
            bodyTransform.rotation = unsafe_rot;


            // Corect height
            UpdatePivot();

            float max_body_pivot_height = 1.1f;
            float min_body_pivot_height = 0.2f;

            bodyTransform.position = new Vector3(
                bodyTransform.position.x,
                pivotTransform.position.y + Mathf.Clamp(bodyTransform.position.y - pivotTransform.position.y, min_body_pivot_height, max_body_pivot_height),
                bodyTransform.position.z
            );

            // Get pos
            //BodyPosSet(bend_conf);

            // Get rotation
            //BodyNeckYawSet();
            //Quaternion ang = asdfghRot();//Quaternion.Euler(GetSomeBodyRotation());

            //bodyTransform.rotation = Quaternion.Lerp(bodyTransform.rotation, ang, bend_conf);
        }

        float bend_conf = 0.0f;

        void GetStandingTransform(out Quaternion rot, out Vector3 pos)
        {
            Vector3 local_eye_to_neck_offset = new Vector3(0.0f, -0.1f, -0.1f);

            Vector3 NeckPos = eyeTransform.position + eyeTransform.TransformVector(local_eye_to_neck_offset);

            float neck_body_offset = -0.4f;

            Vector3 global_down_pos = Vector3.up;

            pos = NeckPos + neck_body_offset * global_down_pos;

            rot = Quaternion.LookRotation(bodyTransform.forward, Vector3.up);
        }

        void GetBendTransform(out Quaternion rot, out Vector3 pos)
        {
            Vector3 local_eye_to_neck_offset = new Vector3(0.0f, -0.1f, -0.1f);

            Vector3 NeckPos = eyeTransform.position + eyeTransform.TransformVector(local_eye_to_neck_offset);

            float neck_body_offset = -0.4f;

            Vector3 global_down_pos = eyeTransform.up;

            pos = NeckPos + neck_body_offset * global_down_pos;

            rot = Quaternion.LookRotation(eyeTransform.forward, NeckPos - pos);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(eyeTransform.position, 0.1f);

            Gizmos.color = Color.white;

            Vector3 local_eye_to_neck_offset = new Vector3(0.0f, -0.1f, -0.1f);
            Vector3 NeckPos = eyeTransform.position + eyeTransform.TransformVector(local_eye_to_neck_offset);
            Gizmos.DrawLine(eyeTransform.position, NeckPos);
            Gizmos.DrawCube(NeckPos, new Vector3(0.05f, 0.05f, 0.05f));

            Gizmos.DrawLine(NeckPos, bodyTransform.position);
            Gizmos.DrawCube(bodyTransform.position, new Vector3(0.05f, 0.05f, 0.05f));

            Gizmos.DrawLine(bodyTransform.position, pivotTransform.position);
            Gizmos.DrawCube(pivotTransform.position, new Vector3(0.05f, 0.05f, 0.05f));
        }

        void BodyPosSet(float interp)
        {
            Vector3 local_eye_to_neck_offset = new Vector3(0.0f, -0.1f, -0.1f);

            Vector3 NeckPos = eyeTransform.position + eyeTransform.TransformVector(local_eye_to_neck_offset);

            float neck_body_offset = -0.4f;

            Vector3 local_down_pos = eyeTransform.up;

            Vector3 global_down_pos = Vector3.up;

            bodyTransform.position = NeckPos + neck_body_offset * Vector3.Lerp(global_down_pos, local_down_pos, Mathf.Pow(interp, 2.0f));

            UpdatePivot();

            float max_body_pivot_height = 1.1f;
            float min_body_pivot_height = 0.2f;

            bodyTransform.position = new Vector3(
                bodyTransform.position.x,
                pivotTransform.position.y + Mathf.Clamp(bodyTransform.position.y - pivotTransform.position.y, min_body_pivot_height, max_body_pivot_height),
                bodyTransform.position.z
            );
        }

        void BodyNeckPitchRollSet()
        {

        }

        void BodyNeckYawSet()
        {
            float head_yaw = eyeTransform.localRotation.eulerAngles.y + 360.0f; // is the +360 necessary?

            float body_yaw = bodyTransform.localRotation.eulerAngles.y + 360.0f;

            const float max_yaw_diff = 60;

            float yaw_diff = Mathf.DeltaAngle(head_yaw, body_yaw);

            if (Mathf.Abs(yaw_diff) > max_yaw_diff)
            {
                //bodyTransform.rotation.eulerAngles

                //float rot = Mathf.MoveTowardsAngle(body_yaw, head_yaw, 999) - body_yaw;
                //bodyTransform.Rotate(0, Time.deltaTime * rot, 0, Space.Self);

                float yaw_adapt_speed = Mathf.Abs(yaw_diff) * Time.deltaTime * 2.0f;

                bodyTransform.localRotation = Quaternion.Euler(
                    bodyTransform.localRotation.eulerAngles.x,
                    Mathf.MoveTowardsAngle(body_yaw, head_yaw, yaw_adapt_speed),
                    bodyTransform.localRotation.eulerAngles.z
                );
            }
        }

        void TakeTwo()
        {
            UpdatePivot();
            UpdateRightFoot();
            UpdateLeftFoot();

            //HandPredicament(showguess);
            //NeckPredicament(showguess);
            //GroundPredicament(showguess);

            //PreviousPredicament(showguess);

            //bodyTransform.position = showguess.position;
            //bodyTransform.rotation = showguess.rotation;
            //bodyTransform.forward = showguess.forward;
        }

        void GroundPredicament(Transform guess)
        {
            if (Physics.Raycast(eyeTransform.position, Vector3.down, out RaycastHit hit, 3.0f, collisionMask))
            {
                float dist = Vector3.Distance(hit.point, eyeTransform.position);

                Vector3 upos = eyeTransform.position - Vector3.up * offsetY;

                float confidence = Mathf.Max(dist - 1.3f, 0.0f) / 1.0f;

                if (dist > 1.8) confidence = 1.0f;

                guess.position = Vector3.Lerp(guess.position, upos, confidence);
                guess.up = Vector3.Lerp(guess.up, Vector3.up, confidence);
            }
        }

        void PreviousPredicament(Transform guess)
        {
            const float speed = 10.0f;
            guess.position = Vector3.MoveTowards(bodyTransform.position, guess.position, Time.deltaTime * speed);

            //guess.rotation = Quaternion.
        }

        void NeckPredicament(Transform guess)
        {
            //Vector3 local_body_dir = eyeTransform.InverseTransformPoint(guess.position).normalized;
            Vector3 eyes_to_body = guess.position - eyeTransform.position;

            float h_ang = Vector3.Angle(eyes_to_body, -eyeTransform.up);

            h_ang -= 30;
            h_ang = Mathf.Max(h_ang, 0.0f);
            h_ang /= 15;

            Vector3 targetPos = eyeTransform.position - eyeTransform.up * offsetY;
            guess.position = Vector3.Lerp(guess.position, targetPos, h_ang);
            guess.up = Vector3.Lerp(guess.up, eyeTransform.up, h_ang + 0.3f * Time.deltaTime);
        }

        void HandPredicament(Transform guess)
        {
            Vector3 lhl = eyeTransform.InverseTransformPoint(leftHandTarget.position);
            Vector3 rhl = eyeTransform.InverseTransformPoint(rightHandTarget.position);
            float lhc = Vector3.Cross(Vector3.left, lhl).magnitude;
            float rhc = Vector3.Cross(Vector3.right, rhl).magnitude;

            bool trigger = lhc > 0.1f && rhc > 0.1f && Mathf.Abs(lhc - rhc) < 1.2f;
            trigger = true;

            if (trigger)
            {
                float confidence = Vector3.Cross(rhl.normalized, eyeTransform.right).magnitude;

                //guess.position = leftHandTarget.position + 0.5f * (rightHandTarget.position - leftHandTarget.position);
                guess.position = Vector3.Lerp(guess.position, leftHandTarget.position + 0.5f * (rightHandTarget.position - leftHandTarget.position), confidence);
                guess.position = new Vector3(guess.position.x, eyeTransform.position.y - offsetY, guess.position.z);
                //guess.forward = Vector3.Lerp(guess.forward, Vector3.Lerp(leftHandTarget.forward, rightHandTarget.forward, 0.5f), confidence);
                //guess.forward = eyeTransform.forward;
                //guess.up = Vector3.up;
                //guess.right = (rightHandTarget.position - leftHandTarget.position);
            }
        }

        void UpdateLeftFoot()
        {
            Vector3 thPos = pivotTransform.position - pivotTransform.right * foot_offset_from_pivot;

            leftFootTarget.forward = Vector3.Lerp(leftFootTarget.forward, pivotTransform.forward, foot_follow_speed * Time.deltaTime);

            leftFootTarget.position = Vector3.Lerp(leftFootTarget.position, thPos, foot_follow_speed * Time.deltaTime);
        }

        void UpdateRightFoot()
        {
            Vector3 thPos = pivotTransform.position + pivotTransform.right * foot_offset_from_pivot;

            rightFootTarget.forward = Vector3.Lerp(rightFootTarget.forward, pivotTransform.forward, foot_follow_speed * Time.deltaTime);

            rightFootTarget.position = Vector3.Lerp(rightFootTarget.position, thPos, foot_follow_speed * Time.deltaTime);
        }

        void UpdatePivot()
        {
            Vector3 r_origin = bodyTransform.position;//eyeTransform.position;

            if (Physics.Raycast(r_origin, Vector3.down, out RaycastHit hit, 3.0f, collisionMask))
            {
                pivotTransform.position = hit.point;
            }

            pivotTransform.forward = eyeTransform.forward;
        }

        void SetBodyPosition()
        {
            Vector3 bodyPos = bodyTransform.position;

            // Linked with eye position.
            bodyPos.y = eyeTransform.position.y - offsetY;
            // Linked with "IK_Pivot" position.
            bodyPos.x = pivotTransform.position.x;
            bodyPos.z = pivotTransform.position.z;
            // Set position.

            bodyPos = eyeTransform.position - eyeTransform.up * offsetY;

            //bodyTransform.rotation = eyeTransform.rotation;

            bodyTransform.position = bodyPos;
        }

        void SetBodyRotation()
        {
            // Get pivot
            Transform pivot = pivotTransform;

            // LEFT HAND:
            // Get the location of the hand relative to the pivot
            Vector3 l_hand_local = pivot.InverseTransformPoint(leftHandTarget.position);
            // Angle between pivot.left and left hand (trigonometry)
            float l_hand_angle = l_hand_local.x / armLength;
            // ???
            l_hand_angle = Mathf.LerpAngle(0.0f, maxTorsionAngle, l_hand_angle);

            // RIGHT HAND: (Repeat same steps)
            float r_hand_angle = Mathf.LerpAngle(0.0f, -maxTorsionAngle, -pivotTransform.InverseTransformPoint(rightHandTarget.position).x / armLength);




            // ???
            float handLinkageAng = l_hand_angle + r_hand_angle;




            // HEAD
            // body relative to pivot
            Vector3 thisLocalPos = pivotTransform.InverseTransformPoint(bodyTransform.position);
            // eyes realtive to pivot
            Vector3 eyeLocalPos = pivotTransform.InverseTransformPoint(eyeTransform.position);

            float deltaY = eyeLocalPos.y - thisLocalPos.y;
            float deltaX = eyeLocalPos.x - thisLocalPos.x;
            float deltaZ = eyeLocalPos.z - thisLocalPos.z - offsetZ;

            // From trigonometry we know that tan(angX)=deltaY/deltaX, so this way we calculate angX.
            float angX = Mathf.Atan2(deltaY, deltaZ);
            float angZ = Mathf.Atan2(deltaY, deltaX);

            // We transform into degs and subtract 90
            angX = angX * Mathf.Rad2Deg - 90.0f;
            angZ = angZ * Mathf.Rad2Deg - 90.0f;

            // ???
            float headLinkageAng = Mathf.DeltaAngle(0.0f, eyeTransform.localEulerAngles.y) * 0.25f;

            // Set rotation.
            Vector3 bodyAng = Vector3.zero;
            bodyAng.x = -angX;
            bodyAng.y = pivotTransform.localEulerAngles.y + handLinkageAng + headLinkageAng;
            bodyAng.z = angZ;
            bodyTransform.localEulerAngles = bodyAng;
        }

        Vector3 GetSomeBodyRotation()
        {
            // Get pivot
            Transform pivot = pivotTransform;

            // LEFT HAND:
            // Get the location of the hand relative to the pivot
            Vector3 l_hand_local = pivot.InverseTransformPoint(leftHandTarget.position);
            // Angle between pivot.left and left hand (trigonometry)
            float l_hand_angle = l_hand_local.x / armLength;
            // ???
            l_hand_angle = Mathf.LerpAngle(0.0f, maxTorsionAngle, l_hand_angle);

            // RIGHT HAND: (Repeat same steps)
            float r_hand_angle = Mathf.LerpAngle(0.0f, -maxTorsionAngle, -pivotTransform.InverseTransformPoint(rightHandTarget.position).x / armLength);




            // ???
            float handLinkageAng = l_hand_angle + r_hand_angle;




            // HEAD
            // body relative to pivot
            Vector3 thisLocalPos = pivotTransform.InverseTransformPoint(bodyTransform.position);
            // eyes realtive to pivot
            Vector3 eyeLocalPos = pivotTransform.InverseTransformPoint(eyeTransform.position);

            float deltaY = eyeLocalPos.y - thisLocalPos.y;
            float deltaX = eyeLocalPos.x - thisLocalPos.x;
            float deltaZ = eyeLocalPos.z - thisLocalPos.z - offsetZ;

            // From trigonometry we know that tan(angX)=deltaY/deltaX, so this way we calculate angX.
            float angX = Mathf.Atan2(deltaY, deltaZ);
            float angZ = Mathf.Atan2(deltaY, deltaX);

            // We transform into degs and subtract 90
            angX = angX * Mathf.Rad2Deg - 90.0f;
            angZ = angZ * Mathf.Rad2Deg - 90.0f;

            // ???
            float headLinkageAng = Mathf.DeltaAngle(0.0f, eyeTransform.localEulerAngles.y) * 0.25f;

            // Set rotation.
            Vector3 bodyAng = Vector3.zero;
            bodyAng.x = -angX;
            bodyAng.y = pivotTransform.localEulerAngles.y + handLinkageAng + headLinkageAng;
            bodyAng.z = angZ;
            //bodyTransform.localEulerAngles = bodyAng;
            return bodyAng;
        }
    }
}
