using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVR.Phys
{
    [ExecuteInEditMode]
    public class AVR_DebugHand : AVR.Core.AVR_Behaviour
    {
        public Animator animator;
        public AVR_GrabNode node;
        public Transform grab_point;

        // Start is called before the first frame update
        void Start()
        {
            if(Application.isPlaying) {
                GameObject.DestroyImmediate(gameObject);
            }

            if(!grab_point) {
                grab_point = AVR.Core.Utils.Misc.CreateEmptyGameObject("GrabPoint", transform);
            }

            try {
                AVR_BasicGrabProvider gp = FindObjectOfType<AVR_BasicGrabProvider>();

                grab_point.localPosition = gp.transform.InverseTransformPoint(gp.grabPoint.position);
            } catch(System.Exception) { }

            node = GetComponentInParent<AVR_GrabNode>();

            transform.position = transform.parent.position - (grab_point.position - transform.position);
        }

        void Update()
        {
            node = GetComponentInParent<AVR_GrabNode>();

            transform.localRotation = Quaternion.identity * Quaternion.Euler(node.yaw_test * node.allowed_yaw, node.pitch_test * node.allowed_pitch, node.roll_test * node.allowed_roll);

            transform.position = node.transform.position - (grab_point.position - transform.position);

            //foreach (AVR_Finger f in fingers) f.Update();
            animator.SetLayerWeight(1, 1.0f);
            animator.SetLayerWeight(2, 1.0f);
            animator.SetLayerWeight(3, 1.0f);
            animator.SetLayerWeight(4, 1.0f);
            animator.SetLayerWeight(5, 1.0f);

            animator.Play("Proc_IndexFinger", 1, node.index_pose);
            animator.Play("Proc_MiddleFinger", 2, node.middle_pose);
            animator.Play("Proc_RingFinger", 3, node.ring_pose);
            animator.Play("Proc_PinkyFinger", 4, node.pinky_pose);
            animator.Play("Proc_ThumbFinger", 5, node.thumb_pose);
            animator.Update(0.0f);
        }
    }
}
