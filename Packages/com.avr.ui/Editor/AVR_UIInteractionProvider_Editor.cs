using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using AVR;
using AVR.UI;

namespace AVR.UEditor.UI {
    [CustomEditor(typeof(AVR_UIInteractionProvider))]
    [CanEditMultipleObjects]
    public class AVR_UIInteractionProvider_Editor : AVR.UEditor.Core.AVR_Component_Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            AVR_UIInteractionProvider prov = ((AVR_UIInteractionProvider)target);

            if(prov.UIRay==null) {
                EditorGUILayout.HelpBox("UIInteractionProvider requires a UIRay to work!", MessageType.Warning);
                if (GUILayout.Button("FIX NOW: Add default UIRay"))
                {
                    GameObject ray = AVR.UEditor.Core.AVR_EditorUtility.InstantiatePrefabAsChild(prov.transform, "/editor/defaultPrefabPaths/uiRay");
                    prov.UIRay = ray.GetComponent<AVR_UIRay>();
                }
            }

            // There is no EventSystem
            if(FindObjectOfType<EventSystem>()==null) {
                EditorGUILayout.HelpBox("UIInteractionProvider requires an EventSystem object to work!", MessageType.Warning);
                if (GUILayout.Button("FIX NOW: Create EventSystem Object"))
                {
                    AVR.UEditor.Core.AVR_EditorUtility.InstantiatePrefabAsChild(null, "/editor/defaultPrefabPaths/eventSystem");
                }
            }
            // There is an EventSystem but no VRInput object
            else if(FindObjectOfType<VRInput>()==null) {
                EditorGUILayout.HelpBox("UIInteractionProvider requires a VRInput object to work!", MessageType.Warning);
                EventSystem sys = FindObjectOfType<EventSystem>();
                if (GUILayout.Button("FIX: Add VRInput Component to "+sys.name))
                {
                    sys.gameObject.AddComponent<VRInput>();
                }
            }
        }
    }
}
