using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AVR.Avatar
{
    /// <summary>
    /// Avatar component that doesn't require controllers.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_avatar_1_1_a_v_r___simple_avatar.html")]
    public class AVR_SimpleAvatar : AVR.Core.AVR_Component
    {
        private Animator animator;
        private Vector3 lastPos = Vector3.zero;

        /// <summary>
        /// Animation parameter that corresponds to the speed the player is moving at.
        /// </summary>
        public string speedAnimationParameter = "Speed";

        protected override void Start()
        {
            base.Start();
            animator = GetComponent<Animator>();

            animator.logWarnings = false; //TODO: This disables warning-spam if parameters (like "Speed") dont exist. Perhaps this should be optional based on a setting.

            lastPos = playerRig.FeetInWorldSpace;
        }

        void Update()
        {
            if(!this.enabled) Debug.Log("????????????");

            animator.SetFloat(speedAnimationParameter, playerRig.AvgMotion.magnitude);

            if(playerRig.AvgMotion.magnitude > 0.3f) {
                transform.forward = Vector3.Lerp(transform.forward, playerRig.AvgMotion, 0.05f);
            }
            else
            {
                transform.forward = Vector3.Lerp(transform.forward, playerRig.XZPlaneFacingDirection, 0.05f);
            }

            if(Vector3.Distance(lastPos, playerRig.FeetInWorldSpace) > 0.1f) {
                lastPos = Vector3.Lerp(lastPos, playerRig.FeetInWorldSpace, 0.1f);
            }

            transform.position = lastPos;
        }
    }
}