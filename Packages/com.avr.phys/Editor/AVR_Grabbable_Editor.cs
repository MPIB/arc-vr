using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using AVR.Phys;

namespace AVR.UEditor.Phys
{
    [CustomEditor(typeof(AVR_Grabbable), true)]
    [CanEditMultipleObjects]
    public class AVR_Grabbable_Editor : AVR.UEditor.Core.AVR_Component_Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(GUILayout.Button("Add GrabNode")) {
                Transform t = AVR.Core.Utils.Misc.CreateEmptyGameObject("GrabNode", ((MonoBehaviour)target).transform);
                t.gameObject.AddComponent<AVR_GrabNode>();
            }
        }
    }
}

