using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

using AVR.Core;
using UnityEngine.XR.Management;

namespace AVR.Core {
    [ExecuteInEditMode]
    /// <summary>
    /// Declares a set of base DevConsole commands.
    /// </summary>
    public class AVR_Core_ConsoleCommands
    {
        #if UNITY_EDITOR
        [InitializeOnLoadMethod]
        #endif
        [RuntimeInitializeOnLoadMethod]
        static void InitCommands()
        {
            AVR_DevConsole.register_command("echo", (s) => AVR_DevConsole.print(string.Join(" ", s)), 1, "Prints a given message back to the user.");

            AVR_DevConsole.register_command("packages", (s) => print_packages(), "Prints a list of all installed arc-vr packages");

            AVR_DevConsole.register_command("clear", (s) => AVR_DevConsole.clear(), "Clears console output");
            AVR_DevConsole.register_command("clr", (s) => AVR_DevConsole.clear(), "Clears console output");

            AVR_DevConsole.register_command("toggle", (s) => toggle_obj(s), 1, "Toggles a scene object with a given name.");

            AVR_DevConsole.register_command("prefabs", (s) => prefabSearch(s), 2, "Searches within the UnityEditor for prefabs. args[0] should be \"ui\" or \"props\" with the remainder being the search term.");

            AVR_DevConsole.register_command("spam", (s) => { for(int i=0; i<100; i++) AVR_DevConsole.print("output"+i); }, "Outputs a bunch of console messages. Used for console testing/debugging.");

            AVR_DevConsole.register_command("reconfigure", (s) => { AVR_Settings.reconfigure(); }, "Re-initializes AVR_Settings.");

            AVR_DevConsole.register_command("get", (s) => AVR_DevConsole.print(AVR_Settings.get_string(s[0])), 1, "Prints a setting with the given settings-token");

            AVR_DevConsole.register_command("set", (s) => AVR_Settings.settings[s[0]]=s[1], 2, "Sets a setting with settings-token arg[0] to the value of arg[1]");
        }

        private static void toggle_obj(string[] args) {
            string remainder = string.Join(" ", args);
            GameObject o = (GameObject)AVR.Core.Utils.Misc.GlobalFind(remainder, typeof(GameObject));
            if (o == null)
            {
                AVR_DevConsole.error("Could not find object with name " + remainder);
            }
            else
            {
                o.SetActive(!o.activeSelf);
                AVR_DevConsole.success("Toggled object " + o.name);
            }
        }

        private static void print_packages() {
            AVR_DevConsole.success("Installed packages:");
            #if AVR_CORE
            AVR_DevConsole.success("> AVR_CORE");
            #endif
            #if AVR_AVATAR
            AVR_DevConsole.success("> AVR_AVATAR");
            #endif
            #if AVR_MOTION
            AVR_DevConsole.success("> AVR_MOTION");
            #endif
            #if AVR_PHYS
            AVR_DevConsole.success("> AVR_PHYS");
            #endif
            #if AVR_UI
            AVR_DevConsole.success("> AVR_UI");
            #endif
            #if AVR_NET
            AVR_DevConsole.success("> AVR_NET");
            #endif
        }

        private static void prefabSearch(string[] args) {
            string remainder = string.Join(" ", args.Skip(1));
            if (args[0] == "props")
            {
                AVR_DevConsole.print("Displaying Avr_Prop labeled prefabs with query: " + remainder);
                SetSearch("l:Avr_Prop " + remainder);
            }
            if (args[0] == "ui")
            {
                AVR_DevConsole.print("Displaying Avr_Ui labeled prefabs with query: " + remainder);
                SetSearch("l:Avr_Ui " + remainder);
            }
        }

        // To check how to set certain searchfilters, check: https://github.com/Unity-Technologies/UnityCsReference/blob/61f92bd79ae862c4465d35270f9d1d57befd1761/Editor/Mono/ProjectWindow/SearchFilter.cs#L278
        // or alternatively: SearchUtility.cs
        // At the time of writing it is:
        // t:<query>    for classnames
        // l:<query>    for assetlabels
        // v:<query>    for versioncontrolstates
        // b:<query>    for assetbundles
        // glob:<query> for ???
        private static void SetSearch(string searchstring)
        {
            try
            {
                // Get type UnityEditor.ProjectBrowser from the UnityEditor Assembly
                // Check https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/ProjectBrowser.cs
                // for source code on this class
                System.Type upb_t = System.Type.GetType("UnityEditor.ProjectBrowser, UnityEditor");

                // Get the first object of this type
                Object projectBrowser = Resources.FindObjectsOfTypeAll(upb_t)[0];
                if (projectBrowser == null)
                {
                    AVR_DevConsole.error("Could not get an active ProjectBrowser.");
                    return;
                }

                //// Set Searchstring
                // get method SeatSearch(string) from UnityEditor.ProjectBrowser object
                System.Reflection.MethodInfo setSearchInfo = upb_t.GetMethod("SetSearch", new[] { typeof(string) });

                setSearchInfo.Invoke(projectBrowser, new object[] { searchstring });
            }
            catch (System.Exception e)
            {
                AVR_DevConsole.error("Encountered an exception while setting search-string. Do you not have a ProjectBrowser open, or are you using a different Unity version?");
                AVR_DevConsole.error("Stacktrace: " + e.StackTrace);
            }
        }
    }
}
