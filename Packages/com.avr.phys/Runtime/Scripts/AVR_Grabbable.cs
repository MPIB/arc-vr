using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AVR.Phys {
    public class AVR_Grabbable : AVR.Core.AVR_Component
    {
        public GrabbableObjectType objectType;
        private GrabbableObjectType _objType;
        private List<AVR_GrabNode> nodes = new List<AVR_GrabNode>();

        public List<AVR_GrabNode> grabNodes => nodes;

        public Rigidbody rb;
        public List<Collider> colliders = new List<Collider>();

        [HideInInspector]
        public List<AVR_BasicGrabProvider> AttachedHands = new List<AVR_BasicGrabProvider>();

        private Transform old_parent;

        private List<Vector3> velocities = new List<Vector3>();
        int velocity_count = 2;

        private float last_dist = 0.0f;

        public bool isGrabbed {
            get { return AttachedHands.Count>0; }
        }

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
            if(objectType==null) objectType = GrabbableObjectType.defaultObjectType();
        }

        void FixedUpdate()
        {
            if (isGrabbed)
            {
                UpdateVelocities();
            }
            else {
                force = Vector3.zero;
            }
        }

        public void Grab(AVR_BasicGrabProvider hand)
        {
            #if AVR_NET
            // If we are online and this object is not owned by the grabbing player or the server -> dont allow grab.
            if(networkAPI.isOnline() && !networkAPI.isOwnedByServer(this) && !networkAPI.isOwner(this)) {
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
        }

        public void Release(AVR_BasicGrabProvider hand)
        {
            #if AVR_NET
            networkAPI.removeOwner(this);
            #endif

            if(AttachedHands.Contains(hand)) AttachedHands.Remove(hand);
            if(AttachedHands.Count<1) {
                transform.SetParent(old_parent);
                if(velocities.Count>0) rb.velocity = new Vector3(velocities.Average(v => v.x), velocities.Average(v => v.y), velocities.Average(v => v.z));
                velocities.Clear();
            }
        }

        Vector3 lastVel = Vector3.zero;
        Vector3 wacc = Vector3.zero;
        Vector3 worldvel = Vector3.zero;
        Vector3 vel = Vector3.zero;

        Vector3 force = Vector3.zero;
        Vector3 cvel = Vector3.zero;

        void UpdateVelocities()
        {
            #if AVR_NET
            if(!networkAPI.isOwner(this)) return;
            #endif

            // NOTE: use rb.worldCenterofMass etc isntead of transform pos + rotation ?
            // TODO: Break grab if the object is too far away from hand

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
            else if(AttachedHands.Count==1) {
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
                // This is the simples way to get the average of multiple quaternions:
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
    }
}
