using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using AVR.Core;

namespace AVR.UEditor.Core
{
    [CustomEditor(typeof(AVR_Behaviour), true)]
    [CanEditMultipleObjects]
    public class AVR_Behaviour_Editor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawToolbar(target);

            DrawDefaultInspector();
        }

        protected void DrawToolbar(object target)
        {
            try {
                DrawToolbar(AVR.Core.Utils.Misc.GetAttribute<AVR.Core.Attributes.DocumentationUrl>(target.GetType()).getDocumentationUrl());
            } catch(System.Exception) {
                AVR_DevConsole.warn("Object does not have a DocumentationUrl set: " + target);
            }
        }

        protected virtual void DrawToolbar(string docu_url) {
            EditorGUILayout.BeginHorizontal();

            // Documentation button
            AVR.UEditor.Core.AVR_EditorUtility.Documentation_Url(docu_url);

            EditorGUILayout.EndHorizontal();
        }
    }
}
