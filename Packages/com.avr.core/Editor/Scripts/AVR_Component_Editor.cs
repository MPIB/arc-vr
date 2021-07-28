using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using AVR.Core;

namespace AVR.UEditor.Core
{
    [CustomEditor(typeof(AVR_Component), true)]
    [CanEditMultipleObjects]
    public class AVR_Component_Editor : AVR_Behaviour_Editor
    {
        protected override void DrawToolbar(string docu_url) {
            EditorGUILayout.BeginHorizontal();

            // Documentation button
            AVR.UEditor.Core.AVR_EditorUtility.Documentation_Url(docu_url);

            // Events button
            AVR.UEditor.Core.AVR_EditorUtility.EventsSettings_Button((AVR_Component)target);

            // Network button
            /*
#if AVR_NET
            if(typeof(AVR_Component).IsAssignableFrom(target.GetType())) {
                AVR.UEditor.Core.AVR_EditorUtility.NetworkSetting_Button((AVR_Component)target);
            }
#endif
*/
            EditorGUILayout.EndHorizontal();
        }
    }
}
