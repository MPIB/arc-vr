using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using AVR.Motion;
using AVR.Core;

namespace AVR.UEditor.Motion {
    [CustomEditor(typeof(AVR_LocomotionProvider))]
    [CanEditMultipleObjects]
    public class AVR_LocomotionProvider_Editor : AVR.UEditor.Core.AVR_Component_Editor
    {
        protected bool rigHasCharacterController = false;

        void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
            DrawToolbar(target);

            AVR_PlayerRig rig = FindObjectOfType<AVR_PlayerRig>();
            if(rig==null) {
                EditorGUILayout.HelpBox("No AVR_PlayerRig object could be found!", MessageType.Error);
            }
            else if(rig.GetComponent<CharacterController>() == null) {
                EditorGUILayout.HelpBox("Parent AVR_PlayerRig does not have a Charactercontroller component. AVR_LocomotionProvider requires a Charactercontroller to work.", MessageType.Warning);
                if(GUILayout.Button("FIX NOW: Add Charactercontroller to "+rig.gameObject.name)) {
                    rig.gameObject.AddComponent<CharacterController>();
                }
            }

            DrawDefaultInspector();
        }
    }
}
