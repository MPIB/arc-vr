using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.Core;

namespace AVR.Net {
    public class AVR_ThirdPersonCharacterController : AVR_Component
    {
        public Animator anim;
        public CharacterController cc;

        public AnimationCurve accel_curve;
        public AnimationCurve dccel_curve;
        public float turnspeed = 100.0f;
        public float max_speed = 3.0f;
        public float min_speed = -3.0f;
        public float jump_force = 5.0f;

        private float speed = 0.0f;
        private float yspeed = 0.0f;
        private float accel_t = 0.0f;
        private float dccel_t = 0.0f;

        // Start is called before the first frame update
        void Start()
        {
            anim = GetComponent<Animator>();
            cc = GetComponent<CharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 movement = Vector3.zero;

            if(Input.GetKey(KeyCode.Mouse1)) {
                transform.Rotate(Vector3.up, Input.GetAxis("Mouse X"), Space.Self);
                //transform.Rotate(Vector3.left, Input.GetAxis("Mouse Y"), Space.Self);
                Camera.main.transform.RotateAround(transform.position, Camera.main.transform.right, -Input.GetAxis("Mouse Y"));
            }

            if (cc.isGrounded)
            {
                // Forward / Backward
                if (Input.GetKey(KeyCode.W))
                {
                    //speed = Mathf.Clamp(speed + accel * Time.deltaTime, 0.0f, max_speed);
                    speed = max_speed * accel_curve.Evaluate(accel_t);
                    accel_t += Time.deltaTime;
                    dccel_t = 0.0f;
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    speed = min_speed * accel_curve.Evaluate(accel_t);
                    accel_t += Time.deltaTime;
                    dccel_t = 0.0f;
                }
                else
                {
                    //speed = Mathf.Clamp(speed - accel * Time.deltaTime, 0.0f, max_speed);
                    speed = max_speed * dccel_curve.Evaluate(dccel_t);
                    dccel_t += Time.deltaTime;
                    accel_t = 0.0f;
                }

                // Turning
                if (Input.GetKey(KeyCode.A))
                {
                    transform.Rotate(new Vector3(0, -turnspeed * Time.deltaTime, 0), Space.Self);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    transform.Rotate(new Vector3(0, turnspeed * Time.deltaTime, 0), Space.Self);
                }

                // Jumping
                if (Input.GetKeyDown(KeyCode.Space) && cc.isGrounded)
                {
                    yspeed = jump_force;
                    anim.SetBool("Jump", true);
                }
                else if (cc.isGrounded && yspeed > 0)
                {
                    yspeed = 0.0f;
                    anim.SetBool("Jump", false);
                }
            }


            movement = transform.forward * speed;
            movement.y = yspeed;

            cc.Move(movement * Time.deltaTime);

            anim.SetFloat("Speed", speed);
        }
    }
}