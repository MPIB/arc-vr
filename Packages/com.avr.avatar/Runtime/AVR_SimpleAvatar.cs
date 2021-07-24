using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AVR.Avatar
{
    [RequireComponent(typeof(Animator))]
    public class AVR_SimpleAvatar : AVR.Core.AVR_Component
    {
        private Animator animator;

        public string speedAnimationParameter = "Speed";

        protected override void Start()
        {
            base.Start();
            animator = GetComponent<Animator>();

            animator.logWarnings = false; //TODO: This disables warning-spam if parameters (like "Speed") dont exist. Perhaps this should be optional based on a setting.
        }

        void Update()
        {
            animator.SetFloat(speedAnimationParameter, playerRig.AvgMotion.magnitude);

            if(playerRig.AvgMotion.magnitude > 0.1f) {
                transform.forward = Vector3.Lerp(transform.forward, playerRig.AvgMotion, 0.1f);
            }
            else
            {
                transform.forward = Vector3.Lerp(transform.forward, playerRig.XZPlaneFacingDirection, 0.1f);
            }

            transform.position = playerRig.FeetInWorldSpace;
        }
    }
}