using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using AVR.Core;

namespace AVR.UEditor.Core {
    [CustomEditor(typeof(AVR_PlayerRig))]
    [CanEditMultipleObjects]
    public class AVR_PlayerRig_Editor : AVR_Component_Editor
    {
        protected bool showInputSettings = true;

        void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            DrawToolbar(target);
            
            DrawDefaultInspector();

            AVR_PlayerRig rig = (AVR_PlayerRig)target;

            //ObjectBuilderScript myScript = (ObjectBuilderScript)target;
            if (GUILayout.Button("Add Module"))
            {
                //myScript.BuildObject();
                //AVR_PlayerRig_ModuleWizard.CreateWizard(rig);
                AVR_PlayerRig_ModuleWizard.ShowWindow(rig.gameObject, "Player Rig Module Wizard");
            }
        }
    }
}
