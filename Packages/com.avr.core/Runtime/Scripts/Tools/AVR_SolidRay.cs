using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVR.Core {
    /// <summary>
    /// A more advanced type of AVR_Ray. The ray can be used to get the first object hit by it.
    /// To limit collidable objects by certain types, use AVR_SolidRay.passThrough_layerMask.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___solid_ray.html")]
    [RequireComponent(typeof(LineRenderer))]
    public class AVR_SolidRay : AVR_Ray
    {
        //// MEMBERS ===================================================================================================
        /// <summary> Layermask used to selectively ignore colliders when casting a ray. </summary>
        public LayerMask hit_layerMask;

        /// <summary>
        /// Raycasthit where the ray hit something.
        /// Contains the last hit the ray did if AVR_SolidRay.objectHit is false.
        /// null if the ray hasn't hit a single object in its lifetime.
        ///</summary>
        public RaycastHit hitPosition
        {
            get { return _hitPosition; }
        }
        private RaycastHit _hitPosition;

        /// <summary> True if the ray hit some object this frame. Use AVR_SolidRay.hitPosition to get the respective RaycastHit. </summary>
        public bool objectHit
        {
            get { return _objectHit; }
        }
        private bool _objectHit;


        //// METHODS ===================================================================================================

        protected override void UpdateRay()
        {
            _objectHit = false;
            base.UpdateRay();
        }

        // Raycasting functionality for straight rays:
        protected override void UpdateStraightRay()
        {
            base.UpdateStraightRay();
            // If we hit something along the way, cut beam off there
            if (AVR.Core.Utils.Phys.PathCast(positions, out RaycastHit hit, hit_layerMask))
            {
                this.positions[1] = hit.point;
                _objectHit = true;
                _hitPosition = hit;
            }
        }

        // Raycasting functionality for projectile rays:
        // NOTE: We're rewriting the base function from AVR_Ray at this point so we can calculate collisions and positions side-by-side and not back-to-back
        protected override void UpdateProjectileRay()
        {
            List<Vector3> posl = new List<Vector3>();

            for (int i = 0; i < this.proj_max_verts; i++)
            {
                float dist = (float)i / this.proj_resolution;

                // Add new vertex to line
                posl.Add(transform.position + transform.forward * dist - Vector3.up * (dist * dist) / (proj_velocity * proj_velocity));

                // If we have 2 or more vertices check for collisions
                if (posl.Count > 1 && AVR.Core.Utils.Phys.LineCast(posl[posl.Count - 2], posl[posl.Count - 1], out RaycastHit hit, hit_layerMask))
                {
                    _objectHit = true;
                    _hitPosition = hit;
                    break;
                }

                // Check if we're within distance limitations. NOTE: We are only restricting distance in the direction of transform.forward, not up or down.
                if (dist >= max_length) break;
            }

            this.positions = posl.ToArray();
            lr.useWorldSpace = true;
            lr.positionCount = posl.Count;
            lr.SetPositions(this.positions);
        }
    }
}
