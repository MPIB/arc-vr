using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.Core;

namespace AVR.Phys {
    /// <summary>
    /// Represents an (attempted) grab at a given location of an object.
    /// </summary>
    public class GrabLocation
    {
        public AVR_Grabbable grabbable;    // Owner object
        public Vector3 location => isNode ? node.transform.position : grabbable.transform.TransformPoint(_llocation); // Location being grabbed
        public Vector3 localLocation => _llocation;
        public float distance;             // Distance between grabbed location and grab-point/hand

        private Vector3 _llocation;

        public bool isNode;                // Are we grabbing a node or a collider?
        public Collider collider;          // Collider we are grabbing. Null of we grab a node
        public AVR_GrabNode node;          // Node we are grabbing. Null if we grab a collider.

        public GrabLocation(AVR_Grabbable grabbable, Vector3 location, float distance, Collider collider)
        {
            this.grabbable = grabbable;
            this._llocation = grabbable.transform.InverseTransformPoint(location);
            this.distance = distance;
            this.collider = collider;
            this.node = null;
            isNode = false;
        }

        public GrabLocation(AVR_Grabbable grabbable, Vector3 location, float distance, AVR_GrabNode node)
        {
            this.grabbable = grabbable;
            this._llocation = grabbable.transform.InverseTransformPoint(location);
            this.distance = distance;
            this.collider = null;
            this.node = node;
            isNode = true;
        }
    }

    /// <summary>
    /// Class to retrieve a Grabbable object from a location, volume or similar.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_phys_1_1_a_v_r___grabbable_finder.html")]
    public abstract class AVR_GrabbableFinder : AVR_Component
    {
        public abstract bool getGrabLocation(out GrabLocation location);

        // Returns the closest location inside the "valid grabzone". This is used by the likes of OffsetGrabProvider
        // for pre-grab hand movement and such
        public virtual Vector3 closestPoint(Vector3 pos) {
            return transform.position;
        }
    }
}
