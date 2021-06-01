using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace  AVR.Core {
    // TODO: This class is outdated. It makes no use of settings-keybindings and the history is wrong. Look to AVR_BasicDevConsole.

    /// <summary>
    /// Mirrors the output of the DevConsole onto a Text component.
    /// To use, simply assign "DevConsoleOutput" to a Text Component that represents the output-terminal.
    /// Assign "DevConsoleInput" to an inputfield representing the user input.
    /// To submit a command, the user may use the keybinding specified as the /settings/core/devConsole/submitConsoleKey setting.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___dev_console_mirror.html")]
    public class AVR_DevConsoleMirror : AVR_Behaviour
    {
        public Text DevConsoleOutput;

        public InputField DevConsoleInput;

        private int history_index = 0;
        private KeyCode toggleKey;
        private KeyCode offKey;
        private KeyCode submitKey;

        void Awake() {
            if(!DevConsoleOutput) DevConsoleOutput = GetComponent<Text>();
            if(!DevConsoleOutput) AVR_DevConsole.error("DevConsoleMirror " + gameObject.name + " does not have a Text Component!");
        }

        void OnEnable() {
            if(DevConsoleInput) {
                // Focus inputfield automatically when we show this window. We do this on a slight delay. Otherwise it doesn't work.
                Invoke("FocusInputField", 0.01f);
            }
        }

        private void FocusInputField() {
            DevConsoleInput.Select();
            DevConsoleInput.ActivateInputField();
        }

        void OnGUI() {
            DevConsoleOutput.text = AVR_DevConsole.get_text();

            if(DevConsoleInput) {
                if(Input.GetKeyDown(KeyCode.UpArrow)) {
                    DevConsoleInput.text = AVR_DevConsole.history(history_index);
                    history_index++;
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    history_index--;
                    DevConsoleInput.text = AVR_DevConsole.history(history_index);
                }
                if(DevConsoleInput.isFocused && DevConsoleInput.text != "" && Input.GetKey(AVR.Core.AVR_Settings.get_key("/settings/core/devConsole/submitConsoleKey")))
                {
                    run_command();
                    FocusInputField(); // Re-focus inputfield for further commands
                }
            }
        }

        /// <summary>
        /// Run the command that is written in DevConsoleInput.
        /// </summary>
        public void run_command() {
            history_index = 0;
            if(!DevConsoleInput) {
                AVR_DevConsole.error("DevConsoleMirror " + gameObject.name + " tried to run a command, but no input component was assigned!");
                return;
            }
            AVR_DevConsole.command(DevConsoleInput.text);
            DevConsoleInput.text = "";
        }
    }
}
