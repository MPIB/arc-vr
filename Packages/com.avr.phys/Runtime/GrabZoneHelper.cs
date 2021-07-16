using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GrabZoneHelper : MonoBehaviour
{
    List<Collider> colliders = new List<Collider>();

    private Collider col;

    void Start() {
        if(!col) col = GetComponent<Collider>();
    }

    public Collider getTriggerCollider() {
        return col;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetType()==typeof(MeshCollider) && !((MeshCollider)other).convex) return; //Dont add non-convex colliders
        if(!colliders.Contains(other)) colliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if(colliders.Contains(other)) colliders.Remove(other);
    }

    // Finds the closest collider to aPos and returns it with some extra info
    public bool getPoint(Vector3 atPos, out Collider collider, out float dist, out Vector3 point) {
        float smallest_dist = float.PositiveInfinity;
        Collider smallest_coll = null;
        Vector3 closest_point = Vector3.zero;

        for(int i=0; i<colliders.Count; i++) {
            if(!colliders[i]) {
                colliders.RemoveAt(i);
                i--;
                continue;
            }

            Collider c = colliders[i];
            Vector3 p = c.ClosestPoint(atPos);
            if(Vector3.Distance(p, atPos) < smallest_dist) {
                smallest_dist = Vector3.Distance(p, atPos);
                smallest_coll = c;
                closest_point = p;
            }
        }
        dist = smallest_dist;
        collider = smallest_coll;
        point = closest_point;
        return collider!=null;
    }
}
