using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AVR.UEditor.UI
{
    public class AVR_UI_EditorUtility
    {
        public static Dictionary<string, string> settings = new Dictionary<string, string>()
        {
            { "editor/defaultPrefabPaths/UIInteractionProvider", "Packages/com.avr.ui/Editor/DefaultPrefabs/UIInteractionProvider.prefab" },
            { "editor/defaultPrefabPaths/eventSystem", "Packages/com.avr.ui/Editor/DefaultPrefabs/EventSystem.prefab" },
        };

        public static GameObject InstantiatePrefabAsChild(Transform parent, string settings_token)
        {
            Object prefab = AssetDatabase.LoadAssetAtPath(AVR_UI_EditorUtility.settings[settings_token], typeof(GameObject));
            if (prefab == null)
            {
                Debug.LogError("AVR: There is a prefab missing at: " + settings[settings_token]);
                return null;
            }
            else
            {
                GameObject o = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                o.transform.SetParent(parent);
                o.transform.localPosition = Vector3.zero;
                o.transform.localRotation = Quaternion.identity;
                return o;
            }
        }
    }
}
