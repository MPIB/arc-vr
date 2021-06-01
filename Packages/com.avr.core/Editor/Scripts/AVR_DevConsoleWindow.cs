using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using AVR.Core;

namespace AVR.UEditor.Core {
    public class AVR_DevConsoleWindow : EditorWindow
    {
        private string cmd_string = "Enter Command...";
        private Vector2 scrollPos;
        private int refocus = 0;
        private int history = -1;

        [MenuItem("AVR/Open DevConsole")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(AVR_DevConsoleWindow), false, "ARC-VR Console");
        }

        void OnGUI()
        {
            GUILayout.BeginVertical();

            //// SETTINGS BUTTON
            GUIStyle sb = new GUIStyle(GUI.skin.button);
            sb.font = AVR_Core_EditorUtility.GetFont("/editor/fonts/font-awesome");
            sb.fontSize = 11;

            string cog_symbol = AVR_Core_EditorUtility.Unicode_to_String("f013");
            if (GUILayout.Button(cog_symbol, sb, GUILayout.Width(25), GUILayout.Height(25)))
            {
                AVR_DevConsole.print("Settings Button doesnt do anything! :(");
            }
            

            //// OUTPUT TEXT AREA
            // Output font style
            GUIStyle s0 = new GUIStyle(GUI.skin.textArea);
            s0.wordWrap = true;
            s0.richText = true;
            s0.alignment = TextAnchor.LowerLeft;
            s0.fontSize = 16;
            s0.font = AVR_Core_EditorUtility.GetFont("/editor/fonts/inconsolata");

            // Output area color/bounds
            GUI.contentColor = new Color(1f, 1f, 1f, 1f);
            GUI.backgroundColor = new Color(0.288f, 0.296f, 0.319f, 1f);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(Screen.height - 85), GUILayout.Width(Screen.width - 10));
            EditorGUILayout.TextArea(AVR_DevConsole.get_text(), s0, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
            

            GUI.backgroundColor = new Color(0.088f, 0.096f, 0.019f, 1f);
            GUI.SetNextControlName("InputField");
            cmd_string = EditorGUILayout.TextField(cmd_string, s0, GUILayout.Height(20), GUILayout.Width(Screen.width - 10));

            if(Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return) {
                AVR_DevConsole.command(cmd_string);

                // This big "refocus" mess is here because for cmd_string="" to actually register, the textarea HAS to be out of focus for a coupla of frames.
                // To accomplish this, we switch focus away with "GUIUtility.keyboardControl = 0", then 2 frames later (refocus>3) we focus back on the InputField with GUI.FocusControl("InputField")
                // This way, the input-textarea actually clears when we run a command, but gets refocused so the user may input another command immediately aferwards.
                cmd_string = "";
                GUIUtility.keyboardControl = 0;
                refocus = 1;
                history = -1;

                scrollPos = new Vector2(0, float.PositiveInfinity); // Set scrollbar to bottom
                Repaint();
            }

            if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.UpArrow)
            {
                history += 1;
                cmd_string = AVR.Core.AVR_DevConsole.history(history);
                GUIUtility.keyboardControl = 0;
                refocus = 1;
                Repaint();
            }

            if(refocus>0) {
                refocus++;
                if(refocus>3) {
                    GUI.FocusControl("InputField");
                    refocus = 0;
                }
            }

            GUILayout.EndVertical();
        }
    }
}
