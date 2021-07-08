using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AVR.UEditor.Phys {
    public class AVR_Phys_EditorUtility
    {
        public static Dictionary<string, string> settings = new Dictionary<string, string>()
        {
            { "editor/defaultPrefabPaths/basicGrabProvider", "Packages/com.avr.phys/Editor/DefaultPrefabs/BGrabProvider.prefab" },
            { "editor/defaultPrefabPaths/offsetGrabProvider", "Packages/com.avr.phys/Editor/DefaultPrefabs/OGrabProvider.prefab" },
            { "editor/defaultPrefabPaths/advancedOffsetGrabProvider", "Packages/com.avr.phys/Editor/DefaultPrefabs/AOGrabProvider.prefab" },
            { "editor/defaultPrefabPaths/leftHandVisual", "Packages/com.avr.phys/Editor/DefaultPrefabs/HandVisual_left.prefab" },
            { "editor/defaultPrefabPaths/rightHandVisual", "Packages/com.avr.phys/Editor/DefaultPrefabs/HandVisual_right.prefab" },
            { "editor/defaultPrefabPaths/HandVisual_Controller", "Packages/com.avr.phys/Editor/DefaultPrefabs/ViveController.prefab" },
        };

        public static GameObject InstantiatePrefabAsChild(Transform parent, string settings_token)
        {
            Object prefab = AssetDatabase.LoadAssetAtPath(AVR_Phys_EditorUtility.settings[settings_token], typeof(GameObject));
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
