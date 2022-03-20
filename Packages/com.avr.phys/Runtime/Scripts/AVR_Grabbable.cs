using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AVR.Phys {
    /// <summary>
    /// Represents a grabbable object.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_phys_1_1_a_v_r___grabbable.html")]
    public class AVR_Grabbable : AVR.Core.AVR_Component
    {
        /// <summary>
        /// Type that describes the objects behaviour when grabbed.
        /// </summary>
        public GrabbableObjectType objectType;
        private GrabbableObjectType _objType;

        /// <summary>
        /// List of GrabNodes that are attatched to this object
        /// </summary>
        public List<AVR_GrabNode> grabNodes => nodes;
        private readonly List<AVR_GrabNode> nodes = new List<AVR_GrabNode>();

        /// <summary>
        /// Rigidbody of this grabbable object.
        /// </summary>
        public Rigidbody rb;

        /// <summary>
        /// Optional AudioSource to play sounds from GrabbableObjectType data
        /// </summary>
        public AudioSource source;

        /// <summary>
        /// List of colliders that describe the outer/grabbable surface. All colliders must be convex.
        /// </summary>
        public List<Collider> colliders = new List<Collider>();

        [HideInInspector]
        /// <summary>
        /// List of hands that are currently grabbing this object.
        /// </summary>
        public List<AVR_BasicGrabProvider> AttachedHands = new List<AVR_BasicGrabProvider>();

        private Transform old_parent;

        private List<Vector3> velocities = new List<Vector3>();
        const int velocity_count = 3;

        private float last_dist = 0.0f;

        /// <summary>
        /// True if the object is being grabbed, otherwise false.
        /// </summary>
        public bool isGrabbed {
            get { return AttachedHands.Count>0; }
        }

        /// <summary>
        /// True if the object is being grabbed by 2 or more hands, otherwise false.
        /// </summary>
        public bool isGrabbedByMultipleHands {
            get { return AttachedHands.Count>1; }
        }

        void Reset() {
            #if AVR_NET
            this.destroyOnRemote = false;
            #endif
        }

        protected override void Awake()
        {
            base.Awake();
            if (rb == null) rb = GetComponent<Rigidbody>();
            if (colliders==null || colliders.Count<1) colliders.AddRange(GetComponentsInChildren<Collider>());
            if (nodes == null || nodes.Count < 1) nodes.AddRange(GetComponentsInChildren<AVR_GrabNode>());
            if (objectType == null) objectType = GrabbableObjectType.defaultObjectType();
            if (source == null) source = GetComponent<AudioSource>();

            //Throw warnings if audiosource isnt a 3D blending source
            if (source != null)
            {
                if (source.spatialBlend == 0.0f)
                {
                    Debug.Log(this.name +
                        "'s Audio Source has no spatial blend. 3D audio will not work. Consider setting spatial blend to 1");
                }
            }
        }

        void FixedUpdate()
        {
            if (isGrabbed)
            {
                UpdateRBVelocity();
            }
            else {
                force = Vector3.zero;
            }
        }

        public void Grab(AVR_BasicGrabProvider hand)
        {
            #if AVR_NET
            // If we are online and this object is not owned by the grabbing player or the server -> dont allow grab.
            if(IsOnline && !IsOwnedByServer && !IsOwner)
            {
                return;
            }
            #endif

            if(!isGrabbed) {
                old_parent = this.transform.parent;
                // while its being grabbed, set the rig as its parent, so that when the rig teleport we dont break the grab-joint.
                transform.SetParent(AVR.Core.AVR_PlayerRig.Instance.transform);
            }
            else if(isGrabbed && !_objType.allowTwoHanded) {
                // flush AttatchedHands.
                while(AttachedHands.Count>0) AttachedHands[0].makeRelease();
            }
            AttachedHands.Add(hand);
            hand.controller.HapticPulse(0.3f, 0.05f);

            //Play pickup sound
            if (source != null && objectType.soundData.pickupSound != null)
            {
                source.PlayOneShot(objectType.soundData.pickupSound, objectType.soundData.volumeMultiplier);
            }

#if AVR_NET
            if (IsOnline)
            {
                NetworkObject.ChangeOwnership(this.OwnerClientId);
            }
#endif
        }

        public void Release(AVR_BasicGrabProvider hand)
        {
            #if AVR_NET
            if (IsOnline) NetworkObject.RemoveOwnership();
            #endif

            if(AttachedHands.Contains(hand)) AttachedHands.Remove(hand);
            if(AttachedHands.Count<1) {
                transform.SetParent(old_parent);
                if(velocities.Count>0) rb.velocity = new Vector3(velocities.Average(v => v.x), velocities.Average(v => v.y), velocities.Average(v => v.z));
                velocities.Clear();

                //Play release sound
                if (source != null && objectType.soundData.releaseSound != null)
                {
                    source.PlayOneShot(objectType.soundData.releaseSound, objectType.soundData.volumeMultiplier);
                }
            }
        }

        Vector3 lastVel = Vector3.zero;
        Vector3 wacc = Vector3.zero;
        Vector3 worldvel = Vector3.zero;

        Vector3 force = Vector3.zero;
        Vector3 cvel = Vector3.zero;

        void UpdateRBVelocity()
        {
            #if AVR_NET
            if(IsOnline && !IsOwner) return;
            #endif

            // Get pos-rot of hand and item
            Vector3 targetItemPosition = transform.position;
            Quaternion targetItemRotation = transform.rotation;
            Vector3 targetHandPosition = this.getTargetPosition();
            Quaternion targetHandRotation = this.getTargetRotation();

            // Set the current objecttype to the regular object type or the nested one
            _objType = (isGrabbedByMultipleHands && objectType.changeObjectTypeOnTwoHanded) ? objectType.typeOnTwoHanded : objectType;

            // Break joint if the distance is too far & it is growing.
            if (Vector3.Distance(targetItemPosition, targetHandPosition) > last_dist && last_dist > _objType.Break_grab_distance)
            {
                while (AttachedHands.Count > 0) AttachedHands[0].makeRelease();
                return;
            }
            last_dist = Vector3.Distance(targetItemPosition, targetHandPosition);

            // Follow aglorithms
            switch(_objType.followType) {
                case GrabbableObjectType.FollowType.FREE : {
                    //pos
                    Vector3 pDelta = (targetHandPosition - targetItemPosition);
                    Vector3 vel = pDelta / Time.fixedDeltaTime;

                    rb.velocity = vel * _objType.Lightness;
                    lastVel = rb.velocity;

                    //ang
                    Quaternion rotationDelta = targetHandRotation * Quaternion.Inverse(targetItemRotation);

                    rotationDelta.ToAngleAxis(out float angle, out Vector3 axis);
                    while (angle > 180) angle -= 360; // Prevent object from doing sudden >180° turns instead of negative <180° ones

                    rb.maxAngularVelocity = 99999.0f;

                    Vector3 angvel = (angle * axis * Mathf.Deg2Rad) / Time.fixedDeltaTime;
                    if (!float.IsNaN(angvel.z)) rb.angularVelocity = angvel * _objType.Angular_Lightness;
                    break;
                }
                case GrabbableObjectType.FollowType.STATIC : {
                    break;
                }
                case GrabbableObjectType.FollowType.CONSTRAINED : {
                    Vector3 pDelta = (targetHandPosition - targetItemPosition);
                    Vector3 vel = pDelta / Time.fixedDeltaTime;

                    // The current world acceleration is rb.velocity - lastVel. However, due to sudden changes in the objects position, we prevent the acceleration from jumping to
                    // dramatically by smoothing out the world acceleration throuhg a simple linear interpolation
                    // Lower the value of 0.5 to make the world acceleration more delayed/elastic, but also lower the jitter an object may experience
                    wacc = Vector3.Lerp(wacc, rb.velocity - lastVel, 0.5f);

                    worldvel = wacc / Time.fixedDeltaTime; //We scale wacc similarly to pDelta, to compare with vel

                    worldvel -= worldvel.normalized * Mathf.Min(worldvel.magnitude, 10);      //10 == minimum force applied by hand
                    vel = Vector3.ClampMagnitude(vel, 30);                                  //30 == maximum force applied by hand

                    if (!float.IsNaN(worldvel.x) && !float.IsNaN(worldvel.y) && !float.IsNaN(worldvel.z))
                    {
                        rb.velocity = vel + worldvel;
                    }
                    else
                    {
                        rb.velocity = Vector3.zero;
                    }

                    lastVel = rb.velocity;


                    //ang
                    Quaternion rotationDelta = targetHandRotation * Quaternion.Inverse(targetItemRotation);

                    rotationDelta.ToAngleAxis(out float angle, out Vector3 axis);
                    while (angle > 180) angle -= 360; // Prevent object from doing sudden >180° turns instead of negative <180° ones

                    rb.maxAngularVelocity = 99999.0f;

                    Vector3 angvel = (angle * axis * Mathf.Deg2Rad) / Time.fixedDeltaTime;
                    if (!float.IsNaN(angvel.z)) rb.angularVelocity = (angvel * objectType.Angular_Lightness);
                    break;
                }
                case GrabbableObjectType.FollowType.HEAVY : {
                    // Point where object was grabbed
                    Vector3 closestp = AttachedHands[0].getWorldGrabLocation();

                    // Difference between point where object was grabbed at and current hand/palm position (normalized over a few frames)
                    force = Vector3.Lerp(force, AttachedHands[0].grabPoint.position - closestp, 0.25f);

                    // Velocity of rigidbody at grabbed position (normalized)
                    cvel = Vector3.Lerp(cvel, rb.GetPointVelocity(closestp), 0.25f);

                    float k = 0.3f;
                    // delta = the force we want to apply minus the objects current velocity
                    Vector3 delta = force - Vector3.Lerp(Vector3.zero, cvel, k * Vector3.Distance(AttachedHands[0].grabPoint.position, closestp));
                    Vector3 f = delta * Time.fixedDeltaTime * 100.0f * _objType.Heavy_force_multiplier;

                    rb.velocity *= 0.8f;
                    rb.AddForceAtPosition(f, closestp, ForceMode.Impulse);
                    break;
                }
            }
            velocities.Add(rb.velocity);
            while (velocities.Count > velocity_count) velocities.RemoveAt(0);
        }

        Vector3 getTargetPosition() {
            if(!isGrabbed) {
                return transform.position;
            }
            else if(AttachedHands.Count == 1) {
                return AttachedHands[0].getTargetPosition();
            }
            else {
                Vector3 sum = Vector3.zero;
                foreach (AVR_BasicGrabProvider h in AttachedHands) sum += h.getTargetPosition();
                return sum/AttachedHands.Count;
            }
        }

        Quaternion getTargetRotation() {
            if (!isGrabbed) {
                return transform.rotation;
            }
            else if (AttachedHands.Count == 1) {
                return AttachedHands[0].getTargetRotation();
            }
            else {
                // TODO: This sometimes leads to weird rotations.
                // This is the simplest way to get the average of multiple quaternions:
                float w = 1f / AttachedHands.Count;
                Quaternion avg = Quaternion.identity;
                for (int i = 0; i < AttachedHands.Count; i++)
                {
                    Quaternion q = AttachedHands[i].getTargetRotation();
                    avg *= Quaternion.Slerp(Quaternion.identity, q, w);
                }
                return avg;
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            // Haptic feedback
            foreach(AVR_BasicGrabProvider h in AttachedHands)
            {
                h.controller.HapticPulse(Mathf.Lerp(0, 0.5f, 0.2f * collision.relativeVelocity.magnitude), 0.05f);
            }

            //Play collision sound
            if (source != null && objectType.soundData.collideSounds != null && objectType.soundData.collideSounds.Count != 0 && rb != null)
            {
                //Calculate a sound multiplier based off the velocity and mass of the collision
                float soundMultiplier = objectType.soundData.volumeMultiplier;

                float mass = rb.mass;
                float speed = rb.velocity.magnitude;
                float drag = rb.drag == 0.0f ? 0.5f : rb.drag; //If the rigidbody drag is 0, use 0.5. Otherwise, use that drag.

                const float frontalArea = 0.5f;
                const float airDrag = 1.1f;

                //Equation adapted and modified from https://www.grc.nasa.gov/WWW/K-12/rocket/termvr.html
                float terminalSpeed = Mathf.Sqrt(2.0f * mass * Physics.gravity.magnitude / airDrag * frontalArea * drag);

                //When you grab an object, it teleports.
                //This creates these wildly large speed values when grabbed. This will ignore them.
                if (speed > terminalSpeed)
                {
                    return;
                }

                //Using the adapted terminal velocity, create a ratio from 0 - 1 of speed.
                //Use this ratio as the sound multiplier. The faster the object moves, the louder the sound.
                float physicsSoundMultiplier = speed / terminalSpeed;

                soundMultiplier *= physicsSoundMultiplier;

                AudioClip randomClip = objectType.soundData.collideSounds[Random.Range(0, objectType.soundData.collideSounds.Count)];
                source.PlayOneShot(randomClip, soundMultiplier);
            }
        }
    }
}
