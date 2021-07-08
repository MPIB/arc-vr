using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVR.Phys {
    public class AVR_Hand : AVR_HandVisual
    {
        // For fingers:
        private List<AVR_Finger> fingers = new List<AVR_Finger>();
        public Animator animator;
        public Transform index_tip, middle_tip, ring_tip, pinky_tip, thumb_tip;
        public float delta = 0.05f;

        // For hand
        public Transform glove_transform;
        public AVR.Core.AVR_Controller controller;
        private Transform fake_parent;
        private Vector3 def_pos;
        private Quaternion def_rot;
        private Transform handControllerTransform;

        public bool hand_colliders;
        public bool physical_hand;
        public Rigidbody physical_hand_rb;

        public List<Collider> colliders = new List<Collider>();

        // Where Hand is drawn/rendered
        public Transform HandVisualTransform() {
            return glove_transform;
        }

        // Position HandVisual *should* be, or strive towards
        public Transform HandControllerTransform() {
            return handControllerTransform;
        }

        // Where the actual VR Controller itself is
        public Transform ControllerTransform() {
            return controller.transform;
        }

        public void disableColliders() {
            foreach(Collider c in colliders) c.enabled = false;
        }

        public void enableColliders()
        {
            foreach (Collider c in colliders) c.enabled = true;
        }

        void Start()
        {
            if(!controller) controller = GetComponentInParent<AVR.Core.AVR_Controller>();

            colliders.AddRange(GetComponentsInChildren<Collider>());
            if(!hand_colliders) {
                foreach(Collider c in colliders) {
                    Destroy(c);
                }
                colliders.Clear();
            }

            if(physical_hand && !physical_hand_rb) {
                physical_hand_rb = GetComponent<Rigidbody>();
            }

            handControllerTransform = new GameObject("handControllerTransform").transform;
            handControllerTransform.SetParent(ControllerTransform());
            handControllerTransform.position = HandVisualTransform().position;
            handControllerTransform.rotation = HandVisualTransform().rotation;
            def_pos = HandVisualTransform().localPosition;
            def_rot = HandVisualTransform().localRotation;
            fingers.Add(new AVR_Finger(index_tip, 1, "Proc_IndexFinger", animator));
            fingers.Add(new AVR_Finger(middle_tip, 2, "Proc_MiddleFinger", animator));
            fingers.Add(new AVR_Finger(ring_tip, 3, "Proc_RingFinger", animator));
            fingers.Add(new AVR_Finger(pinky_tip, 4, "Proc_PinkyFinger", animator));
            fingers.Add(new AVR_Finger(thumb_tip, 5, "Proc_ThumbFinger", animator));

            foreach (AVR_Finger f in fingers) f.Calibrate(HandVisualTransform(), delta);
        }

        public void SqueezeOn(Collider collider) {
            foreach (AVR_Finger f in fingers) f.SqueezeOn(HandVisualTransform(), collider);
        }

        public void SqueezeOn(Collider collider, Vector3 collider_offset) {
            collider.transform.position += collider_offset;
            SqueezeOn(collider);
            collider.transform.position -= collider_offset;
        }

        public void SqueezeOn(Grabbable g)
        {
            foreach (AVR_Finger f in fingers) f.SqueezeOn(HandVisualTransform(), g.colliders);
        }

        public void SqueezeOn(Grabbable g, Vector3 grabbable_offset)
        {
            g.transform.position += grabbable_offset;
            SqueezeOn(g);
            g.transform.position -= grabbable_offset;
        }

        public void Relax() {
            SetSqueezeWeight(0.0f);
        }

        public void SetSqueezeWeight(float w) {
            foreach (AVR_Finger f in fingers) f.setWeight(w);
        }

        void Update() {
            foreach (AVR_Finger f in fingers) f.Update();
        }

        void LateUpdate() {
            if(fake_parent!=null) {
                HandVisualTransform().position = fake_parent.position;
                HandVisualTransform().rotation = fake_parent.rotation;
            }
        }

        void FixedUpdate() {
            if(fake_parent==null && physical_hand) {
                //TODO: Make HandVisualTransform() follow ControllerTransform() as if it was a grabbed object
                //pos
                Vector3 pDelta = (HandControllerTransform().position - HandVisualTransform().position);
                Vector3 vel = pDelta / Time.fixedDeltaTime;
        
                physical_hand_rb.velocity = vel;
        
                //ang
                Quaternion rotationDelta = HandControllerTransform().rotation * Quaternion.Inverse(HandVisualTransform().rotation);
        
                rotationDelta.ToAngleAxis(out float angle, out Vector3 axis);
                while (angle > 180) angle -= 360; // Prevent object from doing sudden >180° turns instead of negative <180° ones
        
                physical_hand_rb.maxAngularVelocity = 99999.0f;
        
                Vector3 angvel = (angle * axis * Mathf.Deg2Rad) / Time.fixedDeltaTime;
                if (!float.IsNaN(angvel.z)) physical_hand_rb.angularVelocity = angvel;
            }
            else {
                
            }
        }

        public void SetFakeParent(Transform fp) {
            def_pos = HandVisualTransform().localPosition;
            def_rot = HandVisualTransform().localRotation;
            fake_parent = new GameObject("DummyParent").transform;
            fake_parent.position = HandVisualTransform().position;
            fake_parent.rotation = HandVisualTransform().rotation;
            fake_parent.SetParent(fp);
        }

        public void UnsetFakeParent() {
            if(fake_parent==null) return;
            HandVisualTransform().localPosition = def_pos;
            HandVisualTransform().localRotation = def_rot;
            GameObject.Destroy(fake_parent.gameObject);
            fake_parent = null;
        }

        protected class AVR_Finger
        {
            public Animator animator;

            public List<Vector3> positions;
            public Transform tip;
            public int layer;
            public string AnimationState;

            private float state;
            private float weigth;

            private float delta;

            private float state_target;
            private float weigth_target;

            public AVR_Finger(Transform tip, int layer, string AnimationState, Animator anim)
            {
                this.animator = anim;
                this.tip = tip;
                this.layer = layer;
                this.AnimationState = AnimationState;
                this.positions = new List<Vector3>();
            }

            public void setStateImmediate(float state)
            {
                this.state_target = this.state = state;
                animator.Play(this.AnimationState, this.layer, state);
                animator.Update(0.0f);
            }

            public void setState(float state)
            {
                state_target = state;
            }

            public void setWeightImmediate(float w)
            {
                this.weigth_target = this.weigth = w;
                animator.SetLayerWeight(this.layer, w);
                animator.Update(0.0f);
            }

            public void setWeight(float w)
            {
                weigth_target = w;
            }

            public void Update()
            {
                this.weigth = Mathf.Lerp(this.weigth, this.weigth_target, 10f * Time.deltaTime);
                animator.SetLayerWeight(this.layer, this.weigth);

                this.state = Mathf.Lerp(this.state, this.state_target, 10f * Time.deltaTime);
                animator.Play(this.AnimationState, this.layer, this.state);
            }

            public void Calibrate(Transform handVisualTransform, float delta)
            {
                this.delta = delta;
                this.setWeightImmediate(1.0f);
                animator.speed = 0.0f;

                for (float i = 0.0f; i <= 1.0f; i += delta)
                {
                    this.setStateImmediate(i);

                    this.positions.Add(handVisualTransform.InverseTransformPoint(this.tip.position));

                    Debug.DrawLine(handVisualTransform.position, this.tip.position, Color.white, 10.0f);
                }
                this.setWeightImmediate(0.0f);
            }

            public void SqueezeOn(Transform handVisualTransform, Collider coll) {
                int i;
                for (i = 0; i < this.positions.Count; i++)
                {
                    Vector3 pos = handVisualTransform.TransformPoint(this.positions[i]);
                    if (ColliderContains(coll, pos)) break;
                }
                float offset = Mathf.Clamp(-0.5f * delta + (float)(i) / (float)this.positions.Count, 0.0f, 1.0f);
                this.setState(offset);
                this.setWeight(1.0f);
            }

            public void SqueezeOn(Transform handVisualTransform, List<Collider> colls)
            {
                if(colls.Count<1) return;
                if(colls.Count==1) this.SqueezeOn(handVisualTransform, colls[0]);

                int i;
                for (i = 0; i < this.positions.Count; i++)
                {
                    Vector3 pos = handVisualTransform.TransformPoint(this.positions[i]);
                    if (ColliderContains(colls[0], pos)) break;
                }

                if(i>=positions.Count) i--; // Fix: i could be = posiitons.count leading to an out-of-array in the next step.

                // For each of the other colliders (beyond the first one) un-squeeze the finger until it is no longer inside
                for(int j=1; j<colls.Count; j++) {
                    while(i>0 && ColliderContains(colls[j], handVisualTransform.TransformPoint(this.positions[i]))) i--;
                }

                float offset = Mathf.Clamp((float)(i) / (float)this.positions.Count, 0.0f, 1.0f);
                this.setState(offset);
                this.setWeight(1.0f);
            }
        }

        //TODO: Helper method, consider moving it somewhere else, if used elsewhere
        protected static bool ColliderContains(Collider c, Vector3 pos) {
            return Vector3.Distance(pos, c.ClosestPoint(pos)) < 0.001f;
        }
    }
}
