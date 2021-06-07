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
        [Header("Physics")]
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
            checkHit(positions);
        }

        // Raycasting functionality for projectile rays:
        // NOTE: We're rewriting the base function from AVR_Ray at this point so we can calculate collisions and positions side-by-side and not back-to-back
        protected override void UpdateProjectileRay()
        {
            List<Vector3> posl = new List<Vector3>();

            for (int i = 0; i < this.proj_max_verts; i++)
            {
                float dist = (float)i / this.proj_resolution;

                Vector3 dest = RayForward * dist;

                // Add new vertex to line
                posl.Add(transform.position + dest - Vector3.up * (dist * dist) / (proj_velocity * proj_velocity));

                // If we have 2 or more vertices check for collisions
                if(checkHit(posl)) break;

                // Check if we're within distance limitations. NOTE: We are only restricting distance in the direction of RayForward, not up or down.
                if (dist >= max_length) break;

                // Deal with max_horizontal_distance
                if (new Vector2(dest.x, dest.z).magnitude > max_horizontal_distance) {
                    // We merely Linecast 100 units downward.
                    posl.Add(posl[posl.Count-1] + Vector3.down*100);
                    checkHit(posl);
                    break;
                }
            }

            this.positions = posl.ToArray();
            lr.useWorldSpace = true;
            lr.positionCount = posl.Count;
            lr.SetPositions(this.positions);
        }

        // Checks if there is a hit between the last 2 positions.
        private bool checkHit(List<Vector3> positions) {
            if (positions.Count > 1 && AVR.Core.Utils.Phys.LineCast(positions[positions.Count - 2], positions[positions.Count - 1], out RaycastHit hit, hit_layerMask))
            {
                _objectHit = true;
                _hitPosition = hit;
                positions[positions.Count-1] = _hitPosition.point;
                return true;
            }
            return false;
        }

        // Checks if there is a hit between the last 2 positions.
        private bool checkHit(Vector3[] positions)
        {
            if (positions.Length > 1 && AVR.Core.Utils.Phys.LineCast(positions[positions.Length - 2], positions[positions.Length - 1], out RaycastHit hit, hit_layerMask))
            {
                _objectHit = true;
                _hitPosition = hit;
                positions[positions.Length - 1] = _hitPosition.point;
                return true;
            }
            return false;
        }
    }
}
