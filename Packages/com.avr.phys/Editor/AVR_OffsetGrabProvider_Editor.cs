using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using AVR.Core;
using AVR.Phys;

namespace AVR.UEditor.Phys {
    [CustomEditor(typeof(AVR_OffsetGrabProvider))]
    public class AVR_OffsetGrabProvider_Editor : AVR.UEditor.Core.AVR_Component_Editor
    {
        void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            DrawToolbar("class_a_v_r___offset_grab_provider.html");
            
            DrawDefaultInspector();
        }
    }
}
