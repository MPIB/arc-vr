using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using AVR.Core;

namespace AVR.Avatar {
    public class AVR_PoseProvider : AVR.Core.AVR_Component
    {
        public Vector3 lookAtPos => _eyeTransform.position + _eyeTransform.forward;
        public Transform eyeTransform => _eyeTransform;//AVR_PlayerRig.Instance.MainCamera.transform;
        public Transform leftHandTarget => AVR_PlayerRig.Instance.leftHandController.transform;
        public Transform rightHandTarget => AVR_PlayerRig.Instance.rightHandController.transform;
        public Transform leftFootTarget => _leftFootTarget;
        public Transform rightFootTarget => _rightFootTarget;
        public Transform pivotTransform => _pivotTransform;
        public Transform bodyTransform => _bodyTransform;

        protected Transform _leftFootTarget;
        protected Transform _rightFootTarget;
        protected Transform _pivotTransform;
        protected Transform _bodyTransform;

        protected Transform _eyeTransform;


        public float offsetY = 0.5f;
        public float offsetZ = 0.0f;
        public float maxTorsionAngle = 45.0f;
        public float armLength = 0.4f;
        public LayerMask collisionMask;

        public float foot_offset_from_pivot;
        public float foot_spring_dist;
        public float foot_follow_speed;

        private float lean_blend = 0.0f;

        float bend_conf = 0.0f;

        protected override void Awake()
        {
            base.Awake();
            _leftFootTarget = AVR.Core.Utils.Misc.CreateEmptyGameObject("leftFootTarget", transform);
            _rightFootTarget = AVR.Core.Utils.Misc.CreateEmptyGameObject("rightFootTarget", transform);
            _pivotTransform = AVR.Core.Utils.Misc.CreateEmptyGameObject("pivotTarget", transform);
            _bodyTransform = AVR.Core.Utils.Misc.CreateEmptyGameObject("bodyTarget", transform);
            _eyeTransform = AVR.Core.Utils.Misc.CreateEmptyGameObject("eyeTransform", transform);
        }

        int c = 0;

        void SetBody() {
            if (AVR.Core.AVR_PlayerRig.Instance.isLeaningForwards())
            {
                //bend_conf = Mathf.Lerp(bend_conf, 1.0f, Time.deltaTime * 5.0f);
            }
            else
            {
                bend_conf = Mathf.Lerp(bend_conf, 0.0f, Time.deltaTime * 5.0f);
            }

            bend_conf *= AVR.Core.AVR_PlayerRig.Instance.isLeaningForwardsConfidence();



            _eyeTransform.position = AVR_PlayerRig.Instance.MainCamera.transform.position;

            float max_roll = 30.0f;
            float max_pitch = 30.0f;

            //NOTE / TODO: This could lead to problems, as we use localRotation of the camera here. If, say, the camera was in a child object of the GenericXRDevice object, the local rotation would be zero.
            Quaternion r = playerRig.MainCamera.transform.localRotation;
            r = AVR.Core.Utils.Geom.ClampQuaternionRotation(r, new Vector3(max_pitch, 360, max_roll));
            _eyeTransform.localRotation = r;



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

        void Update()
        {
            SetBody();
            last_yaw = Mathf.MoveTowardsAngle(last_yaw, bodyTransform.rotation.eulerAngles.y, 9999.0f);
            //last_yaw = bodyTransform.rotation.eulerAngles.y;
            UpdateLeftFoot();
            UpdateRightFoot();
            //BodyNeckYawSet();
        }

        void GetStandingTransform(out Quaternion rot, out Vector3 pos) {
            Vector3 local_eye_to_neck_offset = new Vector3(0.0f, -0.1f, -0.0f);

            Vector3 NeckPos = eyeTransform.position + eyeTransform.TransformVector(local_eye_to_neck_offset);

            float neck_body_offset = -0.4f;

            Vector3 global_down_pos = Vector3.up;

            pos = NeckPos + neck_body_offset * global_down_pos;

            rot = CorrectBodyYawAngle(Quaternion.LookRotation(bodyTransform.forward, Vector3.up));
        }

        void GetBendTransform(out Quaternion rot, out Vector3 pos)
        {
            Vector3 local_eye_to_neck_offset = new Vector3(0.0f, -0.1f, 0.0f);

            Vector3 NeckPos = eyeTransform.position + eyeTransform.TransformVector(local_eye_to_neck_offset);

            float neck_body_offset = -0.4f;

            Vector3 global_down_pos = eyeTransform.up;

            pos = NeckPos + neck_body_offset * global_down_pos;

            rot = CorrectBodyYawAngle(Quaternion.LookRotation(eyeTransform.forward, NeckPos - pos));
        }
        
        void OnDrawGizmos() {
            if(Application.isPlaying) {
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

                Gizmos.DrawLine(bodyTransform.position, pivotTransform.position);
                Gizmos.DrawCube(pivotTransform.position, new Vector3(0.05f, 0.05f, 0.05f));
            }
        }

        float last_yaw = 0.0f;

        Quaternion CorrectBodyYawAngle(Quaternion rot) {
            float head_yaw = eyeTransform.localRotation.eulerAngles.y + 360.0f; // is the +360 necessary?

            float body_yaw = last_yaw + 360.0f;//rot.eulerAngles.y + 360.0f;

            const float max_yaw_diff = 45;

            float yaw_diff = Mathf.DeltaAngle(head_yaw, body_yaw);

            yaw_diff = Mathf.Sign(yaw_diff) * Mathf.Max(0, Mathf.Abs(yaw_diff) - max_yaw_diff);

            if(AVR_PlayerRig.Instance.MainCamera.transform.up.y <= 0.0f) yaw_diff = 0.0f;

            {
                //bodyTransform.rotation.eulerAngles

                //float rot = Mathf.MoveTowardsAngle(body_yaw, head_yaw, 999) - body_yaw;
                //bodyTransform.Rotate(0, Time.deltaTime * rot, 0, Space.Self);

                float yaw_adapt_speed = Mathf.Abs(yaw_diff) * Time.deltaTime * 2.0f;

                return Quaternion.Euler(
                    bodyTransform.localRotation.eulerAngles.x,
                    Mathf.MoveTowardsAngle(body_yaw, head_yaw, yaw_adapt_speed),
                    bodyTransform.localRotation.eulerAngles.z
                );
            }
        }

        void UpdateLeftFoot() {
            Vector3 thPos = pivotTransform.position - pivotTransform.right * foot_offset_from_pivot;

            leftFootTarget.forward = Vector3.Lerp(leftFootTarget.forward, pivotTransform.forward, foot_follow_speed * Time.deltaTime);

            leftFootTarget.position = Vector3.Lerp(leftFootTarget.position, thPos, foot_follow_speed * Time.deltaTime);
        }

        void UpdateRightFoot() {
            Vector3 thPos = pivotTransform.position + pivotTransform.right * foot_offset_from_pivot;

            rightFootTarget.forward = Vector3.Lerp(rightFootTarget.forward, pivotTransform.forward, foot_follow_speed * Time.deltaTime);

            rightFootTarget.position = Vector3.Lerp(rightFootTarget.position, thPos, foot_follow_speed * Time.deltaTime);
        }

        void UpdatePivot() {
            Vector3 r_origin = bodyTransform.position;//eyeTransform.position;

            if (Physics.Raycast(r_origin, Vector3.down, out RaycastHit hit, 3.0f, collisionMask))
            {
                pivotTransform.position = hit.point;
            }

            pivotTransform.forward = eyeTransform.forward;
        }
    }
}
