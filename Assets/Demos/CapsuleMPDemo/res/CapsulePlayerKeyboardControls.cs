using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.Core;

public class CapsulePlayerKeyboardControls : AVR_Component
{
    void Update()
    {
        float speed = Time.deltaTime * 3.0f;

        if(Input.GetKey(KeyCode.W)) {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, speed);
        }
        else if(Input.GetKey(KeyCode.S)) {
            transform.position = Vector3.MoveTowards(transform.position, transform.position - transform.forward, speed);
        }
        if(Input.GetKey(KeyCode.A)) {
            transform.Rotate(0.0f, -speed*30.0f, 0.0f, Space.Self);
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.Rotate(0.0f, speed*30.0f, 0.0f, Space.Self);
        }
    }
}
