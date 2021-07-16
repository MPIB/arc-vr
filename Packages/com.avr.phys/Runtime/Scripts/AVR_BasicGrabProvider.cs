using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.Core;

namespace AVR.Phys {
    /// <summary>
    /// Simplest GrabProvider. Grabbed objects will move their center (obj.transform.position) towards the respective grabpoint.
    /// </summary>
    public class AVR_BasicGrabProvider : AVR_ControllerComponent
    {
        /// <summary>
        /// Returns true if this provider is currently grabbing an object.
        /// </summary>
        public bool isGrabbing => grabbedObject != null;

        /// <summary>
        /// Event that *commences* a grab. (Such as ONTRIGGERDOWN)
        /// </summary>
        public AVR_ControllerInputManager.BoolEvent grabEvent;

        /// <summary>
        /// Event that *ends* a grab. (Such as ONTRIGGERUP)
        /// </summary>
        public AVR_ControllerInputManager.BoolEvent releaseEvent;

        /// <summary>
        /// Transform of the location an object is grabbed towards (Typically the palm of your hand).
        /// Will be set automatically to the local transfrom if null.
        /// </summary>
        public Transform grabPoint;

        /// <summary>
        /// Area from which the player may grab an object.
        /// Required for the grabprovider to work.
        /// </summary>
        public GrabZoneHelper grabZone;

        /// <summary>
        /// Object that is currently being grabbed. Null if no object is being grabbed
        /// </summary>
        protected AVR_Grabbable grabbedObject;

        /// <summary>
        /// Offset at which an object is being grabbed. Meaning: grab-position in local coordiantes relative to the
        /// grabbed object. For instance: If we grab a Pan by its handle, this value will correspond to the pans handle
        /// in local coordiantes.
        /// </summary>
        protected Vector3 localGrabLocation = Vector3.zero;

        /// <summary>
        /// Offset at which an object is being grabbed. Meaning: grab-position in local coordiantes relative to the
        /// grabbed object. For instance: If we grab a Pan by its handle, this value will correspond to the pans handle
        /// in local coordiantes.
        /// If no object is grabbed, returns Vector3.zero.
        /// </summary>
        public virtual Vector3 getLocalGrabLocation() {
            return localGrabLocation;
        }

        /// <summary>
        /// Location at which the object is being grabbed in world coordinates.
        /// Example: If we grab a pan by its handle, the handles world coordinates will correspond to this.
        /// Returns Vector3.zero if no object is being grabbed.
        /// </summary>
        public virtual Vector3 getWorldGrabLocation() {
            if(!grabbedObject) return Vector3.zero;
            return grabbedObject.transform.TransformPoint(localGrabLocation);
        }

        protected override void Start() {
            if(grabPoint==null) grabPoint = transform;
            if(grabZone==null) grabZone = GetComponentInChildren<GrabZoneHelper>();
            if(grabZone==null) {
                AVR_DevConsole.error("Grabprovider "+gameObject.name+" has no grabZone assigned! Deactivating "+gameObject.name);
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Target position the grabbed object should "strive" towards in world coordiantes.
        /// There will be force pulling getWorldGrabPosition and getTargetPosition together.
        /// </summary>
        public virtual Vector3 getTargetPosition() {
            return grabPoint.position;
        }

        /// <summary>
        /// Target rotation the grabbed object should "strive" towards in world space.
        /// There will be force trying to make the objects rotation and this rotation equal.
        /// </summary>
        public virtual Quaternion getTargetRotation() {
            return grabPoint.rotation;
        }

        /// <summary>
        /// getTargetPosition() and getTargetRotation() combined into one transform.
        /// The grabbed objects transform will strive to adopt these values over time.
        /// </summary>
        public virtual Transform getTargetTransform() {
            return grabPoint;
        }

        /// <summary>
        /// Performs a grab. Is called when the respective "grabEvent" is true.
        /// </summary>
        public virtual void makeGrab() {
            // Get the collider that is closest to the grabPoint
            grabZone.getPoint(grabPoint.position, out Collider c, out float d, out Vector3 p);
            makeGrab(c, d, p);
        }

        /// <summary>
        /// Perform a grab on collider c, at distance d, at world position p. Is called when the respective "grabEvent" is true.
        /// </summary>
        public virtual AVR_Grabbable makeGrab(Collider c, float d, Vector3 p) {
            // Error message, this *should* theoretically not happen ever
            if(grabbedObject!=null) {
                AVR_DevConsole.error("Attempted to Grab object while another is already grabbed!");
                return null;
            }

            if (c == null) return null;

            grabbedObject = c.GetComponentInParent<AVR_Grabbable>();
            if(grabbedObject != null) {
                localGrabLocation = grabbedObject.transform.InverseTransformPoint(p);
                grabbedObject.Grab(this);
            }

            return grabbedObject;
        }

        /// <summary>
        /// Ends a grab. Is called when the respective "releaseEvent" is true.
        /// </summary>
        public virtual void makeRelease() {
            if(grabbedObject !=null) {
                grabbedObject.Release(this);
                grabbedObject = null;
                localGrabLocation = Vector3.zero;
            }
        }

        protected virtual void Update() {
            // Make Grab
            if(controller.inputManager.getEventStatus(grabEvent)) {
                makeGrab();
            }
            // Make Release
            else if(controller.inputManager.getEventStatus(releaseEvent)) {
                makeRelease();
            }
        }
    }
}
