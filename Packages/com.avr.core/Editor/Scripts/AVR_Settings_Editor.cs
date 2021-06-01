using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using AVR.Core;

namespace AVR.UEditor.Core {
    public class AVR_Settings_Editor : EditorWindow
    {
        private static SettingsTreeNode root;
        Vector2 scrollPos;

        [MenuItem("AVR/Settings")]
        public static void ShowWindow()
        {
            root = AVR_Settings.build_tree();
            EditorWindow.GetWindow(typeof(AVR_Settings_Editor), true, "AVR Settings");
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            foreach (SettingsTreeNode n in root.get_children()) OnGUISettingsNode(n);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            if(GUILayout.Button("Save")) {
                // All settings excluding any changes made now as well as changes in ~overridesettings.avr
                Dictionary<string, string> entries_pre = AVR_SettingsParser.AutoParseSettingFiles(true);

                // This will be all settings entries *including* any changes made now as well as in ~overridesettings.avr
                Dictionary<string, string> entries_post = AVR_SettingsParser.AutoParseSettingFiles(false);

                // Scan SettingsTree for any changes in entries_post. Apply these if found.
                root.traverse((SettingsTreeNode n) => { if(n.is_leaf() && entries_post[n.fullname]!=n.value) entries_post[n.fullname] = n.value; });

                // Keys that differ between pre and post
                Dictionary<string, string> entries_diff = new Dictionary<string, string>();

                // Look for differences
                foreach(string k in entries_post.Keys) {
                    //if k is not a key in entries or the entry is different
                    if(!entries_pre.ContainsKey(k) || entries_pre[k]!=entries_post[k]) {
                        entries_diff.Add(k, entries_post[k]);
                    }                        
                }


                // Re-insert these into ~overridesettings.avr
                string content = "";
                string filepath = "";
                foreach (string k in entries_diff.Keys)
                {
                    content += k.Substring(1) + " = \"" + entries_diff[k] + "\"\n";
                }
                try {
                    filepath = System.IO.Directory.GetFiles(Application.dataPath+"/..", "~overridesettings.avr", System.IO.SearchOption.AllDirectories)[0];
                } catch(System.Exception) {
                    AVR_DevConsole.cwarn("Could not find ~overridesettings.avr. Creating new file at directory: " + Application.dataPath, "AVR_Settings_Editor");
                    filepath = Application.dataPath + "/~overridesettings.avr";
                    System.IO.File.Create(filepath);
                }
                System.IO.File.WriteAllText(filepath, content);

                // reload settings
                AVR_Settings.settings = AVR_SettingsParser.AutoParseSettingFiles();
            }
        }

        void OnGUISettingsNode(SettingsTreeNode node) {
            if(node.is_leaf()) {
                string data = node.value;
                if(bool.TryParse(data, out bool b)) {
                    node.value = EditorGUILayout.Toggle(node.name, b).ToString();
                }
                else if(float.TryParse(data, out float f)) {
                    node.value = EditorGUILayout.FloatField(node.name, f).ToString();
                }
                else {
                    node.value = EditorGUILayout.TextField(node.name, data);
                }
            }
            else {
                node.foldout = EditorGUILayout.Foldout(node.foldout, node.name, true);
                EditorGUI.indentLevel++;
                if(node.foldout) foreach(SettingsTreeNode n in node.get_children()) OnGUISettingsNode(n);
                EditorGUI.indentLevel--;
            }
        }
    }
}