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
        
        [Header("Shape")]
        /// <summary> What type of ray should be used </summary>
        public RayMode mode = RayMode.STRAIGHT;

        // These only concern rays of mode=PROJECTILE
        /// <summary> How many vertices per unit distance to use for projectile motion ray </summary>
        [AVR.Core.Attributes.ConditionalHideInInspector("mode", ((int)RayMode.PROJECTILE_MOTION), invertCondition: true)]
        public int proj_resolution = 3;

        /// <summary> Max. amount of vertices to use for projectile motion ray </summary>
        [AVR.Core.Attributes.ConditionalHideInInspector("mode", ((int)RayMode.PROJECTILE_MOTION), invertCondition: true)]
        public float proj_max_verts = 150;

        /// <summary> Starting velocity of the projectile motion </summary>
        [AVR.Core.Attributes.ConditionalHideInInspector("mode", ((int)RayMode.PROJECTILE_MOTION), invertCondition: true)]
        [Range(0.5f, 6.0f)]
        public float proj_velocity = 3;

        [Header("Settings")]
        /// <summary> Hide or show the ray immediately on Awake() </summary>
        public bool start_hidden = true;
        /// <summary> Maximum length of the ray </summary>
        public float max_length = 25;
        /// <summary> Will restrict the length of the ray along the xz-Plane to a given value. </summary>
        public float max_horizontal_distance = 10;
        /// <summary> Will restrict the minium angle of the Ray with the y-Axis. </summary>
        public float min_y_angle = 0;

        // Private / Protected:
        protected LineRenderer lr;
        protected bool _hidden;
        protected Vector3[] positions = new Vector3[0];
        protected Vector3 RayForward = Vector3.forward;

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

            // This paragraph deals with the min_y_angle parameter.
            /// An brief explanation on the math done here: 
            /// Say alpha is the angle between RayForward and the y-axis. We want a minium alpha of 30°.
            /// Basic trigonometry tells us, that alpha >= 30° is equivalent with RayForward.y <= cos(30°).
            /// Consequently, RayForward.y will be clamped to exaclty cos(30°). What remains is setting the xz vector accordingly.
            /// Since the whole vector is normalized, the hypotenuse is 1. So pythagoras => 1^2 = |RayForward.xz|^2 + Rayforward.y^2
            /// As a result, the length of RayForward.xz is equal to |RayForward.xz| = sqrt(1-RayForward.y^2)
            /// And since RayForward.y = cos(30°): |RayForward.xz| = sqrt(1-cos(30°)^2) = sin(30°)
            /// And this is what we do here. If the condition is violated, we set y=0, then multiply the normalized vector with sin(alpha)
            /// and finally set y to cos(alpha).
            RayForward = transform.forward;
            float ang = Mathf.Deg2Rad * min_y_angle;
            if(RayForward.y > Mathf.Cos(ang)) {
                RayForward.y = 0;
                RayForward = RayForward.normalized * Mathf.Sin(ang);
                RayForward.y = Mathf.Cos(ang);
            }

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

            // Se which condition determines the endpoint, max_length or max_horizontal_distance. Semi-TODO: This seems like an unelegant solution. Find a better one.
            Vector3 dest = RayForward * max_length;
            if(new Vector2(dest.x, dest.z).magnitude > max_horizontal_distance){
                dest *= max_horizontal_distance / Mathf.Max(new Vector2(dest.x, dest.z).magnitude, 0.0001f);
            }

            // Initialize between here & max_length
            this.positions[0] = transform.position;
            this.positions[1] = transform.position + dest;

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

                Vector3 dest = RayForward * dist;

                // Add new vertex to line
                posl.Add(transform.position + dest - Vector3.up * (dist * dist) / (proj_velocity * proj_velocity));

                // Check if we're within distance limitations. NOTE: We are only restricting distance in the direction of transform.forward, not up or down.
                if (dist >= max_length) break;

                if (new Vector2(dest.x, dest.z).magnitude > max_horizontal_distance) break;
            }

            this.positions = posl.ToArray();
            lr.useWorldSpace = true;
            lr.positionCount = posl.Count;
            lr.SetPositions(this.positions);
        }

        /// <summary> Hides the ray. A ray is not updated while hidden. </summary>
        public virtual void hide() {
            setHidden(true);
        }

        /// <summary> Shows the ray. A ray is not updated while hidden. </summary>
        public virtual void show() {
            setHidden(false);
        }

        /// <summary>
        /// Set the hidden status of the ray
        /// </summary>
        /// <param name="hidden">True to hide, false to show</param>
        public virtual void setHidden(bool hidden) {
            _hidden = hidden;
            lr.enabled = !hidden;
        }
#if AVR_NET
        [HideInInspector]
        [AVR.Core.Attributes.ShowInNetPrompt]
        public bool synchronizeHidden = false;

        [Unity.Netcode.ServerRpc(RequireOwnership = false)]
        private void syncServerRpc(InternalState state)
        {
            m_ReplicatedState.Value = state;
        }

        private void sync()
        {
            if (!synchronizeHidden) return;
            if (IsOwner)
            {
                InternalState state = new InternalState();
                state.FromReference(this);
            }
            else
            {
                m_ReplicatedState.Value.ApplyState(this);
            }
        }

        private readonly Unity.Netcode.NetworkVariable<InternalState> m_ReplicatedState = new Unity.Netcode.NetworkVariable<InternalState>(Unity.Netcode.NetworkVariableReadPermission.Everyone, new InternalState());

        private struct InternalState : IInternalState<AVR_Ray>
        {
            public bool Hidden;

            public void FromReference(AVR_Ray reference)
            {
                Hidden = reference.isHidden;
            }

            public void ApplyState(AVR_Ray reference)
            {
                reference.setHidden(Hidden);
            }

            public void NetworkSerialize<T>(Unity.Netcode.BufferSerializer<T> serializer) where T : Unity.Netcode.IReaderWriter
            {
                serializer.SerializeValue(ref Hidden);
            }
        }
#endif
    }
}
