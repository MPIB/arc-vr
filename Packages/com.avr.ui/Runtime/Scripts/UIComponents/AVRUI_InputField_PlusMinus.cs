using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AVRUI_InputField_PlusMinus : MonoBehaviour
{
    public float increase_amount = 0.1f;
    public float decrease_amount = 0.1f;

    private InputField input;

    void Awake() {
        input = GetComponent<InputField>();
    }
    
    public void increase_value() {
        input.text = (float.Parse(input.text) + increase_amount).ToString();
    }

    public void decrease_value() {
        input.text = (float.Parse(input.text) - decrease_amount).ToString();
    }
}
