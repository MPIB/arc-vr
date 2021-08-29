using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVR.Phys {
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_phys_1_1_a_v_r___advanced_offset_grab_provider.html")]
    public class AVR_AdvancedOffsetGrabProvider : AVR_OffsetGrabProvider
    {
        private Transform virtualGrabPoint;

        private Vector3 lastPos;
        private float distance_travelled_since_grab = 0f;
        private float normalization_distance = 0.5f;

        private Vector3 pos_offset;
        private Quaternion rot_offset;

        public override Vector3 getTargetPosition() {
            return virtualGrabPoint.position;
        }

        public override Quaternion getTargetRotation() {
            return virtualGrabPoint.rotation;
        }

        public override Transform getTargetTransform() {
            return virtualGrabPoint;
        }

        protected override void Start()
        {
            base.Start();
            virtualGrabPoint = new GameObject("virtualGrabPoint").transform;
            virtualGrabPoint.SetParent(base.getTargetTransform());
            virtualGrabPoint.localRotation = Quaternion.identity;
            virtualGrabPoint.localPosition = Vector3.zero;
        }

        public override void makeGrab(GrabLocation location)
        {
            base.makeGrab(location);
            if (grabbedObject != null)
            {
                if(grabbedObject.objectType.handToObject) {
                    if(handVisual!=null) handVisual.glove_transform.position += grabLocation.location - grabPoint.position;

                    virtualGrabPoint.position = grabbedObject.transform.position; //The target position is equal to the  object position == dont move the object directly on-grab
                    virtualGrabPoint.rotation = grabbedObject.transform.rotation;
                }

                // pos_offset is equal to the local position of virtualGrabPoint == difference to base.getTargetPosition()
                pos_offset = virtualGrabPoint.localPosition;
                rot_offset = virtualGrabPoint.localRotation;

                distance_travelled_since_grab = 0f;
            }
        }

        public override void makeRelease()
        {
            base.makeRelease();
            if(handVisual) handVisual.glove_transform.localPosition = Vector3.zero; //cheaty
            virtualGrabPoint.localPosition = Vector3.zero; //virtualgrabPoint is a child of base.getTargetTransform() so this is fine
            virtualGrabPoint.localRotation = Quaternion.identity;
            pos_offset = Vector3.zero;
            rot_offset = Quaternion.identity;
        }

        protected override void Update() {
            base.Update();

            if(grabbedObject!=null) {
                float factor = distance_travelled_since_grab / normalization_distance;

                virtualGrabPoint.localPosition = Vector3.Lerp(pos_offset, Vector3.zero, factor);
                virtualGrabPoint.localRotation = Quaternion.Lerp(rot_offset, Quaternion.identity, factor);

                distance_travelled_since_grab += 
                    Vector3.Distance(lastPos, virtualGrabPoint.position) +
                    0f
                ;
            }
            else {
                distance_travelled_since_grab = 0f;
            }
            lastPos = virtualGrabPoint.position;
        }

        protected override IEnumerator setHand()
        {
            yield return StartCoroutine(base.setHand());
        }

        protected override IEnumerator unsetHand()
        {
            yield return StartCoroutine(base.unsetHand());
        }
    }
}
