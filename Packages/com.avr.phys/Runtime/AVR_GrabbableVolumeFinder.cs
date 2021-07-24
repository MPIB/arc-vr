using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.Phys;

[RequireComponent(typeof(Collider))]
public class AVR_GrabbableVolumeFinder : AVR_GrabbableFinder
{
    public Transform GrabPoint;
    List<Collider> colliders = new List<Collider>();

    private Collider col;

    void Start()
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
        if (other.GetType() == typeof(MeshCollider) && !((MeshCollider)other).convex) return; //Dont add non-convex colliders
        if (!colliders.Contains(other)) colliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
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

        foreach(var node in grb.grabNodes) {
            if(Vector3.Distance(GrabPoint.position, node.transform.position) < node.override_radius) {
                location = new GrabLocation(grb, closest_point, smallest_dist, node);
                return true;
            }
        }

        location = new GrabLocation(grb, closest_point, smallest_dist, smallest_coll);
        return true;
    }
}
