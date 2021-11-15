using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using AVR.Core;

public class CapsulePlayerKeyboardControls : AVR_Component
{
    void Update()
    {
        float speed = Time.deltaTime * 3.0f;

        if (Keyboard.current[Key.W].isPressed) {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, speed);
        }
        else if (Keyboard.current[Key.S].isPressed) {
            transform.position = Vector3.MoveTowards(transform.position, transform.position - transform.forward, speed);
        }
        if (Keyboard.current[Key.A].isPressed) {
            transform.Rotate(0.0f, -speed*30.0f, 0.0f, Space.Self);
        }
        if (Keyboard.current[Key.D].isPressed) {
            transform.Rotate(0.0f, speed*30.0f, 0.0f, Space.Self);
        }
    }
}
