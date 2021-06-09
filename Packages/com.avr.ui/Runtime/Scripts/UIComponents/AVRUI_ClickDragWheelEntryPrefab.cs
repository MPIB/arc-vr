using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AVR.UI.Utils {
    public class AVRUI_ClickDragWheelEntryPrefab : MonoBehaviour
    {
        private RectTransform rect;
        private Image img;
        private Text txt;

        void Awake() {
            rect = GetComponent<RectTransform>();
            img = GetComponentInChildren<Image>();
            txt = GetComponentInChildren<Text>();
        }

        public void markSelected(bool selected) {
            if(selected) {
                rect.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            }
            else {
                rect.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }

        public void initalize(AVRUI_ClickDragWheel.ClickDragWheelEntry entry, float textradius) {
            img.color = entry.color;
            txt.text = entry.name;
            
            float center_angle = (entry.min_angle + 0.5f * (entry.max_angle - entry.min_angle)) * Mathf.Deg2Rad;
            Vector2 textPos = textradius * new Vector2(-Mathf.Sin(center_angle), -Mathf.Cos(center_angle));
            txt.transform.localPosition = new Vector3(textPos.x, textPos.y, 0.0f);

            img.fillAmount = (entry.max_angle-entry.min_angle) / 360.0f;
            img.transform.localRotation = Quaternion.AngleAxis(entry.min_angle, Vector3.back);
        }
    }
}
