using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using AVR.Core;

namespace AVR.UEditor.Core {
    [CustomEditor(typeof(AVR_Controller), true)]
    [CanEditMultipleObjects]
    public class AVR_Controller_Editor : AVR_Component_Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            AVR_Controller controller = (AVR_Controller) target;

            if (GUILayout.Button("Add Module"))
            {
                AVR_Controller_ModuleWizard.ShowWindow(controller.gameObject, "Controller Module Wizard");
            }
        }
    }
}
