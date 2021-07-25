using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using AVR.UEditor.Core;

namespace AVR.UEditor.Net
{
    public class AVR_Net_MenuItems
    {
        [MenuItem("AVR/Create Networked Player Rig", false, 20)]
        public static void createNetworkedPlayerRig()
        {
            AVR_EditorUtility.InstantiatePrefabAsChild(null, "/editor/defaultPrefabPaths/networkedPlayerRig");
        }

        [MenuItem("AVR/Create NetworkManager", false, 20)]
        public static void createRootObject()
        {
            AVR_EditorUtility.InstantiatePrefabAsChild(null, "/editor/defaultPrefabPaths/networkManager");
        }
    }
}

