using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AVR.Core {
    /// <summary>
    /// Contains constan values, settings etc. which can all be set through *.avr files. Duplicate settings will be overwritten in alphabetical order of the filename.
    /// </summary>
    [ExecuteInEditMode]
    public class AVR_Settings
    {
        private static bool initialized = false;

        /// <summary>
        /// Initialize settings. Calling multiple times will have no effect, use reconfigure() if you want to re-parse all settingfiles.
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        #if UNITY_EDITOR
        [InitializeOnLoadMethod]
        #endif
        public static void initialize() {
            if(initialized) return;
            settings = AVR_SettingsParser.AutoParseSettingFiles();
            initialized = true;
        }

        /// <summary>
        /// Re-initializes settings. Note: This will re-parse all settings files and might take a little.
        /// </summary>
        public static void reconfigure() {
            initialized = false;
            initialize();
        }

        /// <summary>
        /// Dictionary containing all settings by their key. A setting declared as domain = { setting = true } will be retrievable with the token /domain/setting
        /// </summary>
        public static Dictionary<string, string> settings = new Dictionary<string, string>();

        /// <summary>
        /// Get a registered setting of the type KeyCode.
        /// </summary>
        /// <param name="token"> Token identifying the given setting. A setting declared as domain = { setting = true } will be retrievable with the token /domain/setting. </param>
        /// <returns> Registered setting if a valid is found, otherwise KeyCode.None. </returns>
        public static KeyCode get_key(string token) {
            KeyCode key;
            if (System.Enum.TryParse(AVR.Core.AVR_Settings.get_string(token), out key))
            {
                return key;
            }
            else {
                AVR.Core.AVR_DevConsole.cerror(token + " does not contain a valid key or does not exist!", "AVR_Settings");
                return KeyCode.None;
            }
        }

        /// <summary>
        /// Get a registered setting of the type int.
        /// </summary>
        /// <param name="token"> Token identifying the given setting. A setting declared as domain = { setting = true } will be retrievable with the token /domain/setting. </param>
        /// <returns> Registered setting if a valid is found, otherwise 0. </returns>
        public static int get_int(string token)
        {
            if (settings.TryGetValue(token, out string ret))
            {
                return int.Parse(ret, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            }
            else if(!initialized) {
                initialize();
                return get_int(token);
            }
            else
            {
                AVR_DevConsole.warn("Settings do not contain key \"" + token + "\". Using default value 0.");
                return 0;
            }
        }

        /// <summary>
        /// Get a registered setting of the type string.
        /// </summary>
        /// <param name="token"> Token identifying the given setting. A setting declared as domain = { setting = true } will be retrievable with the token /domain/setting. </param>
        /// <returns> Registered setting if a valid is found, otherwise an empty string. </returns>
        public static string get_string(string token) {
            if (settings.TryGetValue(token, out string ret))
            {
                return ret;
            }
            else if (!initialized)
            {
                initialize();
                return get_string(token);
            }
            else
            {
                AVR_DevConsole.warn("Settings do not contain key \"" + token + "\". Using default value (empty string).");
                return "";
            }
        }

        /// <summary>
        /// Get a registered setting of the type float.
        /// </summary>
        /// <param name="token"> Token identifying the given setting. A setting declared as domain = { setting = true } will be retrievable with the token /domain/setting. </param>
        /// <returns> Registered setting if a valid is found, otherwise 1.0. </returns>
        public static float get_float(string token) {
            if(settings.TryGetValue(token, out string ret)) {
                return float.Parse(ret, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            }
            else if (!initialized)
            {
                initialize();
                return get_float(token);
            }
            else {
                AVR_DevConsole.warn("Settings do not contain key \""+token+"\". Using default value of 1.0f.");
                return 1.0f;
            }
        }

        /// <summary>
        /// Get a registered setting of the type bool.
        /// </summary>
        /// <param name="token"> Token identifying the given setting. A setting declared as domain = { setting = true } will be retrievable with the token /domain/setting. </param>
        /// <returns> Registered setting if a valid is found, otherwise false. </returns>
        public static bool get_bool(string token) {
            if (settings.TryGetValue(token, out string ret)) {
                return bool.Parse(ret);
            }
            else if (!initialized)
            {
                initialize();
                return get_bool(token);
            }
            else
            {
                AVR_DevConsole.warn("Settings do not contain key \"" + token + "\". Using default value of false.");
                return false;
            }
        }

        /// <summary>
        /// Check if a given token is registered as a setting.
        /// </summary>
        /// <param name="token"> Token to check for </param>
        /// <returns> True if a setting with this token exists, otherwise false. </returns>
        public static bool token_exists(string token) {
            return settings.ContainsKey(token);
        }

        /// <summary>
        /// Set a setting with a specific token to a specific value.
        /// </summary>
        public static void set(string token, object value) {
            settings[token] = value.ToString();
        }

        /// <summary>
        /// Builds a tree-structure that contains all settings. Note: This may take a while to complete.
        /// </summary>
        /// <returns> Root node of the tree </returns>
        public static SettingsTreeNode build_tree() {
            AVR.Core.AVR_DevConsole.print("Building SettingsTree...");
            SettingsTreeNode root = new SettingsTreeNode("");

            foreach(string key in settings.Keys) {
                string[] dir = key.Split('/').Skip(1).ToArray();
                add_node(dir, root, settings[key]);
            }

            return root;
        }

        private static void add_node(string[] dir, SettingsTreeNode node, string value) {
            foreach(string d in dir) {
                if(!node.has_child(d)) {
                    node.add_child(new SettingsTreeNode(d));
                }
                node = node.get_children().First((c) => c.name==d);
            }
            node.value = value;
        }
    }

    /// <summary>
    /// Class representing a node from a settings-tree returned by AVR_Settings.build_tree()
    /// </summary>
    public class SettingsTreeNode {
        public string fullname;
        public string name;
        private List<SettingsTreeNode> children;
        public string value;

        public bool foldout;

        public SettingsTreeNode(string name) {
            this.fullname = this.name = name;
            this.children = new List<SettingsTreeNode>();
            this.foldout = false;
            this.value = "";
        }

        public bool is_leaf() {
            return this.children.Count<1;
        }

        public bool has_child(string c) {
            return children.Any((child) => child.name==c);
        }

        public void add_child(SettingsTreeNode n) {
            this.children.Add(n);
            n.fullname = this.fullname+"/"+n.fullname;
        }

        public SettingsTreeNode[] get_children() {
            return children.ToArray();
        }

        public void traverse(System.Action<SettingsTreeNode> func) {
            func.Invoke(this);
            foreach(SettingsTreeNode c in children) {
                c.traverse(func);
            }
        }
    }
}
