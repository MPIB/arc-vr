using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AVR.UI.Utils {
    /// <summary>
    /// Inputfield that the value of which can be increased/decreased by function call. The value of the inputfield ought to be numerical.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_u_i_1_1_utils_1_1_a_v_r_u_i___input_field___plus_minus.html")]
    public class AVRUI_InputField_PlusMinus : AVR.Core.AVR_Behaviour
    {
        public float increase_amount = 0.1f;
        public float decrease_amount = 0.1f;

        private InputField input;
        private TMPro.TMP_InputField input_tmp;

        void Awake() {
            input = GetComponent<InputField>();
            input_tmp = GetComponent<TMPro.TMP_InputField>();
            if(!input && !input_tmp) {
                AVR.Core.AVR_DevConsole.cerror("Numeric Inputfield need either a InputField or a TMP_InputField component!", this);
                Destroy(this);
            }
        }
        
        public void increase_value() {
            try {
                if(input) {
                    input.text = (float.Parse(input.text) + increase_amount).ToString();
                }
                else {
                    input_tmp.text = (float.Parse(input_tmp.text) + increase_amount).ToString();
                }
            } catch(System.Exception) { }
        }

        public void decrease_value() {
            try {
                if (input)
                {
                    input.text = (float.Parse(input.text) - decrease_amount).ToString();
                }
                else
                {
                    input_tmp.text = (float.Parse(input_tmp.text) - decrease_amount).ToString();
                }
            } catch(System.Exception) { }
        }
    }
}
