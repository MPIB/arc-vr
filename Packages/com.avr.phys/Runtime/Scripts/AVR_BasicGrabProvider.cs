using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.Core;

namespace AVR.Phys {
    /// <summary>
    /// Simplest GrabProvider. Grabbed objects will move their center (obj.transform.position) towards the respective grabpoint.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_phys_1_1_a_v_r___basic_grab_provider.html")]
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
        public AVR_GrabbableFinder grabbableFinder;

        /// <summary>
        /// Object that is currently being grabbed. Null if no object is being grabbed
        /// </summary>
        protected AVR_Grabbable grabbedObject => grabLocation!=null ? grabLocation.grabbable : null;

        protected GrabLocation grabLocation;

        /// <summary>
        /// Offset at which an object is being grabbed. Meaning: grab-position in local coordiantes relative to the
        /// grabbed object. For instance: If we grab a Pan by its handle, this value will correspond to the pans handle
        /// in local coordiantes.
        /// If no object is grabbed, returns Vector3.zero.
        /// </summary>
        public virtual Vector3 getLocalGrabLocation() {
            return grabbedObject ? grabLocation.localLocation : Vector3.zero;
        }

        /// <summary>
        /// Location at which the object is being grabbed in world coordinates.
        /// Example: If we grab a pan by its handle, the handles world coordinates will correspond to this.
        /// Returns Vector3.zero if no object is being grabbed.
        /// </summary>
        public virtual Vector3 getWorldGrabLocation() {
            return grabbedObject ? grabLocation.location : Vector3.zero;
        }

        protected override void Start() {
            if(grabPoint==null) grabPoint = transform;

            if (grabbableFinder == null) grabbableFinder = GetComponentInChildren<AVR_GrabbableFinder>();
            if (grabbableFinder == null) {
                AVR_DevConsole.error("Grabprovider " + gameObject.name + " has no grabbableFinder assigned! Destroying " + gameObject.name);
                Destroy(gameObject);
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
        /// Performs a grab on whichever object the GrabbableFinder returns. Is called when the respective "grabEvent" is true.
        /// </summary>
        public virtual void makeGrab() {
            // Get the collider that is closest to the grabPoint
            if(grabbableFinder.getGrabLocation(out GrabLocation location)) makeGrab(location);
            else grabLocation = null;
        }

        /// <summary>
        /// Perform a grab on with parameters given in a GrabLocation struct
        /// </summary>
        public virtual void makeGrab(GrabLocation location) {
            grabLocation = location;
            grabbedObject.Grab(this);
        }

        /// <summary>
        /// Ends a grab. Is called when the respective "releaseEvent" is true.
        /// </summary>
        public virtual void makeRelease() {
            if(grabbedObject !=null) {
                grabbedObject.Release(this);
                grabLocation = null;
            }
        }

        protected virtual void Update() {
#if AVR_NET
            if (IsOnline && !IsOwner) return;
#endif
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
