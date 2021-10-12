using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Namespace of the arc-vr-core (UnityEditor) package
/// </summary>
namespace AVR.UEditor.Core {
    [ExecuteInEditMode]
    public static class AVR_EditorUtility
    {
        private static Font fa_cache => _fa_cache!=null ? _fa_cache : _fa_cache = GetFont("/editor/fonts/font-awesome");
        private static Font _fa_cache;

        private static Font fab_cache => _fab_cache != null ? _fab_cache : _fab_cache = GetFont("/editor/fonts/font-awesome-brands");
        private static Font _fab_cache;

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

        public static string Unicode_to_String(int unicode) {
            return ((char)unicode).ToString();
        }

        public static bool FAButton(string unicode, bool isBrandIcon=false, int buttonSize=25) {
            var style = new GUIStyle(GUI.skin.button);
            style.font = isBrandIcon ? fab_cache : fa_cache;
            style.fontSize = Mathf.Max(7, buttonSize - 12);
            return GUILayout.Button(Unicode_to_String(unicode), style, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize));
        }

        public static void FALabel(string unicode, bool isBrandIcon = false, int buttonSize = 25)
        {
            var style = new GUIStyle(GUI.skin.label);
            style.font = isBrandIcon ? fab_cache : fa_cache;
            style.fontSize = Mathf.Max(7, buttonSize - 12);
            GUILayout.Label(Unicode_to_String(unicode), style, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize));
        }

        public static void Documentation_Url(string page) {
            if(FAButton("f02d")) {
                string path = System.IO.Path.GetFullPath(AVR.Core.AVR_Settings.get_string("/editor/documentationPath") + page);
                Application.OpenURL("file:///"+path); 
            }
        }

        public static void EventsSettings_Button(AVR.Core.AVR_Component component) {
            if (FAButton("f0e7"))
            {
                AVR_Component_EventsWizard.CreateWizard(component);
            }
        }

#if AVR_NET
        public static void NetworkSetting_Button(AVR.Core.AVR_Component component) {
            if(FAButton("f6ff")) {
                AVR_Component_NetworkWizard.CreateWizard(component);
            }
        }
#endif
    }
}
