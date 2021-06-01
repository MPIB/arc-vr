using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AVR.UEditor.Core {
    public class AVR_Core_MenuItems : MonoBehaviour
    {
        [MenuItem("AVR/Create Player Rig")]
        public static void createPlayerRig() {
            AVR_Core_EditorUtility.InstantiatePrefabAsChild(null, "/editor/defaultPrefabPaths/playerRig");
        }

        [MenuItem("AVR/Create Root Object")]
        public static void createRootObject()
        {
            AVR_Core_EditorUtility.InstantiatePrefabAsChild(null, "/editor/defaultPrefabPaths/rootObject");
        }
    }
}
