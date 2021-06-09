using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AVR.UI.Utils {
    [ExecuteInEditMode]
    public class AVR_CircularLoadingBar : AVR.Core.AVR_Behaviour
    {
        public Image ringImage;

        public Text percentage_text;

        [Range(0, 1)]
        public float fillAmount = 0.5f;

        void Update()
        {
            ringImage.fillAmount = fillAmount;

            if(percentage_text) {
                percentage_text.text = Mathf.RoundToInt(fillAmount*100).ToString() + "%";
            }
        }
    }
}
