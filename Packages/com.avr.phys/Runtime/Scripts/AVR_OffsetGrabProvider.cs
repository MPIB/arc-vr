using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVR.Phys {
    /// <summary>
    /// Same as BasicGrabProvider but additionally allows grabbing objects at an offset. (Eg. you may grab a pan by its handle instead of just its center)
    /// Also allows the usage of an AVR_Hand.
    /// Use this if you require grabbing larger / more complex objects.
    /// </summary>
    public class AVR_OffsetGrabProvider : AVR_BasicGrabProvider
    {
        /// <summary>
        /// Location an rotation the grabbed objects transform will strive to adopt.
        /// Eg: if we grab a pan by its handle, this Transform represents the virtual location the pans transform will move towards.
        /// </summary>
        private Transform grabbedObjectCenter;

        /// <summary>
        /// HandVisual
        /// </summary>
        public AVR_Hand handVisual;

        protected override void Start() {
            base.Start();
            if(!handVisual) handVisual = transform.parent.GetComponentInChildren<AVR_Hand>();
            grabbedObjectCenter = new GameObject("grabbedObjectCenter").transform;
            grabbedObjectCenter.SetParent(grabPoint);
        }

        public override Vector3 getTargetPosition()
        {
            return grabbedObjectCenter.position;
        }

        public override Quaternion getTargetRotation()
        {
            return grabbedObjectCenter.rotation;
        }

        public override Transform getTargetTransform()
        {
            return grabbedObjectCenter;
        }

        public override void makeRelease()
        {
            base.makeRelease();
            if (handVisual != null) StartCoroutine(unsetHand());
        }

        public override AVR_Grabbable makeGrab(Collider c, float d, Vector3 p) {
            AVR_Grabbable g = base.makeGrab(c, d, p);
            if(g!=null)
            {
                // Set desired object location and rotation
                grabbedObjectCenter.position = grabPoint.position + (c.transform.position - p);
                grabbedObjectCenter.rotation = c.transform.rotation;

                if(handVisual) StartCoroutine(setHand());
            }
            return g;
        }

        protected override void Update() {
            // Pre-grab hand
            if (grabbedObject == null && handVisual != null && grabZone.getPoint(grabPoint.position, out Collider c, out float d, out Vector3 p)) //NOTE: we run getPoint twice in 1 frame if we do makeGrab()
            {
                handVisual.SqueezeOn(c, grabPoint.position - p);

                // This is a hacky/bad way of calculating the "entry-distance" (maximum distance inside the grabzone-collider in direction grabzone_p)
                Vector3 far_away = 100.0f * (p - grabPoint.position);
                Vector3 smth = grabZone.getTriggerCollider().ClosestPoint(far_away);
                float entrydist = Vector3.Distance(smth, grabPoint.position);
                float currentdist = Vector3.Distance(grabPoint.position, p);

                float max_pregrab_adapt_factor = 0.35f; // maximum AdaptWeigth used for pregrab. (weight when grabPoint=p)
                float max_pregrab_grace_dist = 0.03f; // Distance at which we consider "grabPoint=p" for purposes stated above
                float w = Mathf.Max(0, max_pregrab_adapt_factor - (Mathf.Max(0, currentdist - max_pregrab_grace_dist) / entrydist));

                handVisual.SetSqueezeWeight(w);

            }
            else if (grabbedObject == null && handVisual)
            {
                handVisual.Relax();
            }

            // Regular update
            base.Update();
        }

        // Set the handVisual appropriately to currently grabbed Grabbable
        protected virtual IEnumerator setHand()
        {
            handVisual.disableColliders();
            // Wait a bit to ensure at least 1 physics-frame has passed
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime + 0.01f);
            // Wait until the object is close enough
            yield return new WaitUntil(() => Vector3.Distance(grabbedObject.transform.position, getTargetPosition()) < 0.05f); //TODO: grabbedObject == null?

            //TODO: disable hand colliders if the hand is physical

            handVisual.SqueezeOn(grabbedObject);
            handVisual.SetSqueezeWeight(1.0f);
            handVisual.SetFakeParent(grabbedObject.transform);
        }

        protected virtual IEnumerator unsetHand()
        {
            handVisual.enableColliders();
            handVisual.Relax();
            handVisual.UnsetFakeParent();

            //TODO: re-enable colliders if hand is physical

            yield return new WaitForEndOfFrame();
        }
    }
}
