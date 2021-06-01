using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVR.Core {
    /// <summary>
    /// Base class for a ray. A ray is always cast in transform.forward direction of the object it is attatched to.
    /// Rays of this type do not collide with any objects and pass through everything.
    /// Use AVR_SolidRay if collisions are required.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___ray.html")]
    [RequireComponent(typeof(LineRenderer))]
    public class AVR_Ray : AVR_Component
    {
        //// ENUMS =====================================================================================================
        public enum RayMode { STRAIGHT, PROJECTILE_MOTION } //TODO: Implement Bezier, Sine, etc etc


        //// MEMBERS ===================================================================================================
        /// <summary> What type of ray should be used </summary>
        public RayMode mode = RayMode.STRAIGHT;
        /// <summary> Hide or show the ray immediately on Awake() </summary>
        public bool start_hidden = true;
        /// <summary> Maximum length of the ray </summary>
        public float max_length = 25;

        // These only concern rays of mode=PROJECTILE
        /// <summary> How many vertices per unit distance to use for projectile motion ray </summary>
        [AVR.Core.Attributes.ConditionalHideInInspector("mode", ((int)RayMode.PROJECTILE_MOTION), invertCondition:true)]
        public int proj_resolution = 3;

        /// <summary> Max. amount of vertices to use for projectile motion ray </summary>
        [AVR.Core.Attributes.ConditionalHideInInspector("mode", ((int)RayMode.PROJECTILE_MOTION), invertCondition: true)]
        public float proj_max_verts = 150;

        /// <summary> Starting velocity of the projectile motion </summary>
        [AVR.Core.Attributes.ConditionalHideInInspector("mode", ((int)RayMode.PROJECTILE_MOTION), invertCondition: true)]
        [Range(0.5f, 6.0f)]
        public float proj_velocity = 3;

        // Private / Protected:
        protected LineRenderer lr;
        protected bool _hidden;
        protected Vector3[] positions = new Vector3[0];

        /// <summary> Is this ray visible </summary>
        public bool isVisible {
            get { return !_hidden; }
        }

        /// <summary> Is this ray hidden </summary>
        public bool isHidden {
            get { return _hidden; }
        }


        //// METHODS ===================================================================================================

        protected override void Awake()
        {
            base.Awake();

            if(!lr) lr = GetComponent<LineRenderer>();
            if(!lr) {
                AVR_DevConsole.error("AVR_Ray object "+gameObject.name+" has no LineRenderer attatched!");
            }
            if(start_hidden) hide(); else show();
            UpdateRay();
        }

        void Update() {
            UpdateRay(); //This ray needs continous updating
        }

        /// <summary> Updates the ray. Called from Monobehaviour.Update() </summary>
        protected virtual void UpdateRay() {
            if(isHidden) return;

            switch (mode)
            {
                // Straight beam
                case RayMode.STRAIGHT: {
                        UpdateStraightRay();
                        break;
                    }

                // Parabolic projectile motion, affected by gravity
                case RayMode.PROJECTILE_MOTION: {
                        UpdateProjectileRay();
                        break;
                    }
                default: {
                    AVR.Core.AVR_DevConsole.warn("RayMode type of AVR_Ray object "+gameObject.name+" has not been implemented.");
                    break;
                }
            }
        }

        /// <summary> Updates a ray with mode==STRAIGHT </summary>
        protected virtual void UpdateStraightRay() {
            // Make sure we only have 2 positions
            if (positions.Length != 2) positions = new Vector3[2];

            // Initialize between here & max_length
            this.positions[0] = transform.position;
            this.positions[1] = transform.position + (transform.forward * max_length);

            // Set line
            lr.useWorldSpace = true;
            lr.positionCount = 2;
            lr.SetPositions(positions);
        }

        /// <summary> Updates a ray with mode==PROJECTILE </summary>
        protected virtual void UpdateProjectileRay() {
            List<Vector3> posl = new List<Vector3>();

            for (int i = 0; i < proj_max_verts; i++)
            {
                float dist = (float)i / proj_resolution;

                // Add new vertex to line
                posl.Add(transform.position + transform.forward * dist - Vector3.up * (dist * dist) / (proj_velocity * proj_velocity));

                // Check if we're within distance limitations. NOTE: We are only restricting distance in the direction of transform.forward, not up or down.
                if (dist >= max_length) break;
            }

            this.positions = posl.ToArray();
            lr.useWorldSpace = true;
            lr.positionCount = posl.Count;
            lr.SetPositions(this.positions);
        }

        /// <summary> Hides the ray. A ray is not updated while hidden. </summary>
        public virtual void hide() {
            _hidden = true;
            lr.enabled = false;
        }

        /// <summary> Shows the ray. A ray is not updated while hidden. </summary>
        public virtual void show() {
            _hidden = false;
            lr.enabled = true;
        }
    }
}
