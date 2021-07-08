using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using AVR.Core;
using AVR.Phys;

namespace AVR.UEditor.Phys {
    [CustomEditor(typeof(AVR_BasicGrabProvider))]
    public class AVR_BasicGrabProvider_Editor : AVR.UEditor.Core.AVR_Component_Editor
    {
        void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            DrawToolbar("class_a_v_r_1_1_phys_1_1_a_v_r___basic_grab_provider.html");
            
            DrawDefaultInspector();
        }
    }
}
