using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AVR.UEditor.Core {
    public class AVR_Core_EditorUtility
    {
        public static GameObject InstantiatePrefabAsChild(Transform parent, string settings_token) {
            Object prefab = AssetDatabase.LoadAssetAtPath(AVR.Core.AVR_Settings.get_string(settings_token), typeof(GameObject));
            if (prefab == null)
            {
                AVR.Core.AVR_DevConsole.error("There is a prefab missing at: " + AVR.Core.AVR_Settings.get_string(settings_token));
                return null;
            }
            else
            {
                GameObject o = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                o.transform.SetParent(parent);
                o.transform.localPosition = Vector3.zero;
                o.transform.localRotation = Quaternion.identity;
                AVR.Core.AVR_DevConsole.success("Instantiated Prefab. Token: " + settings_token);
                return o;
            }
        }

        public static Font GetFont(string settings_token) {
            return (Font)AssetDatabase.LoadAssetAtPath(AVR.Core.AVR_Settings.get_string(settings_token), typeof(Font));
        }

        public static string Unicode_to_String(string unicode) {
            return ((char)int.Parse(unicode, System.Globalization.NumberStyles.HexNumber)).ToString();
        }

        public static void Documentation_Url(string page) {
            var style = new GUIStyle(GUI.skin.button); 
            style.font = GetFont("/editor/fonts/font-awesome");
            if(GUILayout.Button(Unicode_to_String("f02d"), style, GUILayout.Width(25), GUILayout.Height(25))) {
                string path = System.IO.Path.GetFullPath("Packages/com.avr.core/Documentation/docs/html/" + page);
                Application.OpenURL("file:///"+path);
                
            }
        }

        public static void EventsSettings_Button(AVR.Core.AVR_Component component) {
            var style = new GUIStyle(GUI.skin.button);
            style.font = GetFont("/editor/fonts/font-awesome");
            if (GUILayout.Button(Unicode_to_String("f0e7"), style, GUILayout.Width(25), GUILayout.Height(25)))
            {
                AVR_Component_EventsWizard.CreateWizard(component);
            }
        }

#if AVR_NET
        public static void NetworkSetting_Button(AVR.Core.AVR_Component component) {
            var style = new GUIStyle(GUI.skin.button); 
            style.font = GetFont("/editor/fonts/font-awesome");
            if(GUILayout.Button(Unicode_to_String("f6ff"), style, GUILayout.Width(25), GUILayout.Height(25))) {
                AVR_Component_NetworkWizard.CreateWizard(component);
            }
        }
#endif
    }
}
