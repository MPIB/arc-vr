using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVR.Phys {
    /// <summary>
    /// Retrieves an AVR_Grabbable from a given volume (convex collider). The object closest to the given GrabPoint takes priority.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_phys_1_1_a_v_r___grabbable_volume_finder.html")]
    public class AVR_GrabbableVolumeFinder : AVR_GrabbableFinder
    {
        public Transform GrabPoint;
        List<Collider> colliders = new List<Collider>();

        private Collider col;

        protected override void Start()
        {
            if (!col) col = GetComponent<Collider>();
            if (!GrabPoint) GrabPoint = GetComponentInParent<AVR_BasicGrabProvider>().grabPoint;
        }

        public override Vector3 closestPoint(Vector3 pos)
        {
            return col.ClosestPoint(pos);
        }

        private void OnTriggerEnter(Collider other)
        {
#if AVR_NET
            if (IsOnline && !IsOwner) return;
#endif
            if (other.GetType() == typeof(MeshCollider) && !((MeshCollider)other).convex) return; //Dont add non-convex colliders
            if (!colliders.Contains(other)) colliders.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
#if AVR_NET
            if (IsOnline && !IsOwner) return;
#endif
            if (colliders.Contains(other)) colliders.Remove(other);
        }

        public override bool getGrabLocation(out GrabLocation location) {
            float smallest_dist = float.PositiveInfinity;
            Collider smallest_coll = null;
            Vector3 closest_point = Vector3.zero;

            for (int i = 0; i < colliders.Count; i++)
            {
                if (!colliders[i])
                {
                    colliders.RemoveAt(i);
                    i--;
                    continue;
                }

                Collider c = colliders[i];
                Vector3 p = c.ClosestPoint(GrabPoint.position);
                if (Vector3.Distance(p, GrabPoint.position) < smallest_dist)
                {
                    smallest_dist = Vector3.Distance(p, GrabPoint.position);
                    smallest_coll = c;
                    closest_point = p;
                }
            }

            if(smallest_coll==null) {
                location = null;
                return false;
            }

            AVR_Grabbable grb = smallest_coll.GetComponentInParent<AVR_Grabbable>();

            if(grb==null) {
                AVR.Core.AVR_DevConsole.cwarn("Attempted to grab a collider without an AVR_Grabbable component. Either 1) attatch an AVR_Grabbable to the respective object, or 2), disable collisions between the volumefinder and the given object.", this);
                location = null;
                return false;
            }

            if(grb.grabNodes!=null) {
                foreach(var node in grb.grabNodes) {
                    if(Vector3.Distance(GrabPoint.position, node.transform.position) < node.override_radius) {
                        location = new GrabLocation(grb, closest_point, smallest_dist, node);
                        return true;
                    }
                }
            }

            location = new GrabLocation(grb, closest_point, smallest_dist, smallest_coll);
            return true;
        }
    }
}
