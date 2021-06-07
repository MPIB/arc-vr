using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.Core;

namespace AVR.Motion {
    /// <summary>
    /// AVR_SolidRay but, when an object is hit, the hit is either valid or invalid.
    /// The validity of the hit is determined by AVR_MovementRay.filter.
    /// A different color-gradient and/or reticule is displayed depending on the validity of the ray.
    /// If no object is hit, the ray always defaults to invalid.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_motion_1_1_a_v_r___movement_ray.html")]
    [RequireComponent(typeof(LineRenderer))]
    public class AVR_MovementRay : AVR_SolidRay
    {
        // Filter that determine whether a hit is valid. By default always return true.
        public System.Func<RaycastHit, bool> filter = (RaycastHit h) => true;

        // Public
        [Header("HitReticules")]
        /// <summary>
        /// Reticule object displayed when a valid surface is hit
        /// </summary>
        public GameObject reticule;
        /// <summary>
        /// Reticule object displayed when an invalid surface is hit
        /// </summary>
        public GameObject invalid_reticule;

        // COLOR GRADIENTS
        [Header("Gradients")]
        [SerializeField]
        Gradient m_ValidColorGradient = new Gradient()
        {
            colorKeys = new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
            alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1f, 0.0f), new GradientAlphaKey(1f, 1.0f) }
        };
        /// <summary>Gets or sets the color of the line as a gradient from start to end to indicate a valid state.</summary>	
        public Gradient validColorGradient { get { return m_ValidColorGradient; } set { m_ValidColorGradient = value; } }

        [SerializeField]
        Gradient m_InvalidColorGradient = new Gradient()
        {
            colorKeys = new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.red, 1.0f) },
            alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1f, 0.0f), new GradientAlphaKey(1f, 1.0f) }
        };
        /// <summary>Gets or sets the color of the line as a gradient from start to end to indicate an invalid state.</summary>	
        public Gradient invalidColorGradient { get { return m_InvalidColorGradient; } set { m_InvalidColorGradient = value; } }

        
        /// <summary>
        /// Is this ray valid.
        /// The validity of a Raycasthit is determined on whether if AVR_MovementRay.filter satisfies the current hit.
        /// </summary>
        public bool isValid {
            get { return _valid; }
        }
        private bool _valid;

        public override void hide() {
            base.hide();
            if(reticule) reticule.SetActive(false);
            if(invalid_reticule) invalid_reticule.SetActive(false);
        }

        protected override void UpdateRay() {
            if(isHidden) return;

            base.UpdateRay();

            // Check if object is valid
            _valid = objectHit && this.filter(hitPosition);

            // Set reticule
            if(reticule) {
                reticule.transform.position = hitPosition.point;
                reticule.transform.forward = new Vector3(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z);
                reticule.SetActive(objectHit && isValid);
            }
            else {
                reticule.SetActive(false);
            }

            // Set invalid reticule
            if(invalid_reticule) {
                invalid_reticule.transform.position = hitPosition.point;
                invalid_reticule.transform.forward = new Vector3(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z);
                invalid_reticule.SetActive(objectHit && !isValid);
            }
            else {
                invalid_reticule.SetActive(false);
            }

            // Set gradient
            if(isValid) {
                lr.colorGradient = m_ValidColorGradient;
            }
            else {
                lr.colorGradient = m_InvalidColorGradient;
            }
        }
    }
}
