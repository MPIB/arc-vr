using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AVR.UI.Utils {
    /// <summary>
    /// Inputfield that the value of which can be increased/decreased by function call. The value of the inputfield ought to be numerical.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_u_i_1_1_utils_1_1_a_v_r_u_i___input_field___plus_minus.html")]
    public class AVRUI_TMPInputField_PlusMinus : AVR.Core.AVR_Behaviour
    {
        public float increase_amount = 0.1f;
        public float decrease_amount = 0.1f;

        private TMPro.TMP_InputField input;

        void Awake() {
            input = GetComponent<TMPro.TMP_InputField>();
        }
        
        public void increase_value() {
            input.text = (float.Parse(input.text) + increase_amount).ToString();
        }

        public void decrease_value() {
            input.text = (float.Parse(input.text) - decrease_amount).ToString();
        }
    }
}
