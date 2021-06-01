using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVR.Core {
    /// <summary>
    /// A rudimentary UI version of the AVR_DevConsole. If you wish to implement a better one, take a look at AVR_DevConsoleMirror.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___basic_dev_console_u_i.html")]
    public class AVR_BasicDevConsoleUI : AVR_Behaviour
    {
        public GUIStyle style;
        public Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);
        public Color textBackgroundColor = new Color(0.07f, 0.07f, 0.07f, 1.0f);
        public Vector2 position = new Vector2(0, 0);
        public Vector2 size = new Vector2(820, 420);

        private string cmd;
        private Vector2 scrollPosition;
        private Texture2D background, textBackground;
        private KeyCode toggleKey;
        private KeyCode offKey;
        private KeyCode submitKey;
        private bool active = false;
        private int history = -1;

        void Awake() {
            background = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
            background.SetPixel(0, 0, backgroundColor);
            background.Apply();

            textBackground = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
            textBackground.SetPixel(0, 0, textBackgroundColor);
            textBackground.Apply();

            toggleKey = AVR.Core.AVR_Settings.get_key("/settings/core/devConsole/toggleConsoleKey");
            offKey = AVR.Core.AVR_Settings.get_key("/settings/core/devConsole/disableConsoleKey");
            submitKey = AVR.Core.AVR_Settings.get_key("/settings/core/devConsole/submitConsoleKey");
        }

        private void runCommand() {
            AVR.Core.AVR_DevConsole.command(cmd);
            cmd = "";
            scrollPosition = new Vector2(0, float.PositiveInfinity);
            history = -1;
        }

        void OnGUI()
        {
            if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == toggleKey || Event.current.keyCode == offKey))
            {
                active = !active && Event.current.keyCode != offKey;
                history = -1;
            }
            if(active) {
                // We need check for submitKey *before* running FocusControl, or it wont work.
                if (Event.current.type == EventType.KeyDown && Event.current.keyCode == submitKey) {
                    runCommand();
                }
                if(Event.current.type==EventType.KeyDown && Event.current.keyCode == KeyCode.UpArrow) {
                    history += 1;
                    cmd = AVR.Core.AVR_DevConsole.history(history);
                }

                GUI.FocusControl("BasicDevConsoleUI_input");

                // Background box
                style.normal.background = background;
                GUI.Box(new Rect(position.x, position.y, size.x, size.y), "", style);

                // Content area
                GUILayout.BeginArea(new Rect(position.x+10, position.y+10, size.x-20, size.y-20), style);

                // Console Output
                style.normal.background = textBackground;
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, style);
                GUILayout.TextArea(AVR.Core.AVR_DevConsole.get_text(), style);
                style.normal.background = background;
                GUILayout.EndScrollView();

                // Command input and run button
                GUILayout.BeginHorizontal(style);
                GUI.SetNextControlName("BasicDevConsoleUI_input");
                cmd = GUILayout.TextField(cmd, style);
                if(GUILayout.Button("Run", style, GUILayout.Width(50))) {
                    runCommand();
                }
                GUILayout.EndHorizontal();

                GUILayout.EndArea();
            }
        }
    }
}