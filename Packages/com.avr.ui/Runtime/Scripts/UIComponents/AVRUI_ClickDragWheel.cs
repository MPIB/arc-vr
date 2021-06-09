using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using AVR;
using AVR.Core;

namespace AVR.UI.Utils {
    [RequireComponent(typeof(LineRenderer))]
    public class AVRUI_ClickDragWheel : MonoBehaviour
    {
        public GameObject entryPrefab;
        public bool test = false;
        public float radius = 100.0f;
        public float select_radius = 50.0f;
        public Color overlayColor;

        public ClickDragWheelEntry[] fields;

        public AVR_ControllerInputManager.Vec2Event selectionAxis;
        public AVR_ControllerInputManager.BoolEvent selectionEvent;

        [System.Serializable]
        public class ClickDragWheelEntry {
            public string name;
            public Color color;
            public UnityEvent OnSelectEvent;
            [HideInInspector] public float min_angle;
            [HideInInspector] public float max_angle;
            [HideInInspector] public AVRUI_ClickDragWheelEntryPrefab prefabObject;
        }

        private LineRenderer lr;
        private ClickDragWheelEntry selectedEntry;

        void Awake() {
            lr = GetComponent<LineRenderer>();
            RectTransform rect = GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(radius * 2.0f, radius * 2.0f);

            float delta_angle = 360.0f / fields.Length;

            for (int i = 0; i < fields.Length; i++)
            {
                GameObject go = Instantiate(entryPrefab, this.transform);
                ClickDragWheelEntry entry = fields[i];

                entry.prefabObject = go.GetComponent<AVRUI_ClickDragWheelEntryPrefab>();
                entry.min_angle = i * delta_angle;
                entry.max_angle = (i + 1) * delta_angle;
                entry.prefabObject.initalize(entry, select_radius+(radius-select_radius)*0.5f);
            }

            GameObject overlay = Instantiate(entryPrefab, this.transform);
            ClickDragWheelEntry tmp = new ClickDragWheelEntry();
            tmp.color = overlayColor;
            tmp.min_angle=0.0f;
            tmp.max_angle=360.0f;
            tmp.name = "";
            overlay.GetComponent<AVRUI_ClickDragWheelEntryPrefab>().initalize(tmp,0.0f);
            overlay.transform.localScale=new Vector3(select_radius/radius, select_radius/radius, 1.0f);
        }

        void Start() {
        }

        void Update() {
            Vector2 selec = radius * VRInput.Instance.inputManager.getEventStatus(selectionAxis);

            lr.SetPositions(new Vector3[] {Vector3.zero, new Vector3(selec.x, selec.y)});

            float angle = Vector2.SignedAngle(selec, Vector2.up)+180.0f;

            selectedEntry = null;
            foreach(ClickDragWheelEntry entry in fields) {
                if(selec.magnitude > select_radius && angle>entry.min_angle && angle<entry.max_angle) {
                    entry.prefabObject.markSelected(true);
                    selectedEntry = entry;
                }
                else {
                    entry.prefabObject.markSelected(false);
                }
            }

            if (selectedEntry!=null && VRInput.Instance.inputManager.getEventStatus(selectionEvent))
            {
                selectedEntry.OnSelectEvent.Invoke();
                Cancel();
            }
            else if(VRInput.Instance.inputManager.getEventStatus(AVR_ControllerInputManager.BoolEvent.ANY_CANCEL)) {
                Cancel();
            }
        }

        public void Cancel() {
            gameObject.SetActive(false);
        }
    }
}
