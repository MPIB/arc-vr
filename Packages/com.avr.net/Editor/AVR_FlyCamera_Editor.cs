using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using AVR.UEditor.Core;

namespace AVR.UEditor.Net
{
    [CustomEditor(typeof(AVR.Net.AVR_FlyCamera))]
    [CanEditMultipleObjects]
    public class AVR_FlyCamera_Editor : AVR_Component_Editor
    {
        protected bool showInputSettings = true;

        void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            DrawToolbar("TODO");

            DrawDefaultInspector();
        }
    }
}

