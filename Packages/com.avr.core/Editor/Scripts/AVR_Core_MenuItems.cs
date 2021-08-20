using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AVR.UEditor.Core {
    public class AVR_Core_MenuItems
    {
        [MenuItem("AVR/Create Player Rig", false, 1)]
        public static void createPlayerRig() {
            AVR_EditorUtility.InstantiatePrefabAsChild(null, "/editor/defaultPrefabPaths/playerRig");
        }

        [MenuItem("AVR/Create Root Object", false, 1)]
        public static void createRootObject()
        {
            AVR_EditorUtility.InstantiatePrefabAsChild(null, "/editor/defaultPrefabPaths/rootObject");
        }

        [MenuItem("AVR/Documentation", false, -100)]
        public static void openDocumentation()
        {
            string path = System.IO.Path.GetFullPath(AVR.Core.AVR_Settings.get_string("/editor/documentationPath") + "index.html");
            Application.OpenURL("file:///" + path);
        }

        [MenuItem("AVR/arc-vr", false, -999)]
        public static void openContextScreen()
        {
            EditorWindow.GetWindow(typeof(AVR_ArcVRWindow), true, "ARC-VR");
        }
    }
}
