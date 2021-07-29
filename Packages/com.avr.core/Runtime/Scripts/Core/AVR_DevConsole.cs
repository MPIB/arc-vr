using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

using AVR;

namespace AVR.Core {
    /// <summary>
    /// Class for the arc-vr developer's console. Functions as a singleton with only static members. Multiple independent consoles per scene/instance is not possible.
    /// </summary>
    [ExecuteInEditMode]
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___dev_console.html")]
    public class AVR_DevConsole
    {
        private static string output_s;

        private static List<string> output_history = new List<string>();

        private static List<string> command_history = new List<string>();
        private static int command_history_limit = 10;
        private static List<AVR_ConsoleCommand> commands = new List<AVR_ConsoleCommand>();

        private static bool initialized = false;

        private static int repeat_counter = 1;

        /// <summary> Initializes the DevConsole. Only the first execution will have an effect. </summary>
        #if UNITY_EDITOR
        [InitializeOnLoadMethod]
        #endif
        [RuntimeInitializeOnLoadMethod]
        public static void init() {
            if(initialized) return;
            if(AVR_Settings.get_bool("/settings/core/devConsole/relayLogMessages")) {
                Application.logMessageReceived += HandleLog;
                //Application.logMessageReceivedThreaded += HandleLog;
            }
            if (AVR_Settings.get_bool("/settings/core/devConsole/useConsoleCommandAttribute")) {
                register_all_attribute_commands();
            }
            initialized = true;
        }

        /// <summary> Executes a respective output based on logString, stackTrace and LogType. </summary>
        public static void HandleLog(string logString, string stackTrace, LogType type) {
            switch(type) {
                case LogType.Log : { cprint(logString, "Debug.Log"); break; }
                case LogType.Warning: { cwarn(logString, "Debug.Log"); break; }
                case LogType.Error: { cerror(logString, stackTrace); break; }
                case LogType.Exception: { cerror(logString, stackTrace); break; }
                default : { cprint(logString, "Debug.Log"); break; }
            }
        }

        /// <summary>
        /// Get the complete output of this console as text.
        /// </summary>
        public static string get_text() {
            return output_s + (repeat_counter > 1 ? " [x"+repeat_counter+"]" : "");
        }

        /// <summary> Clear console output entirely. </summary>
        public static void clear() {
            output_s = "";
        }

        /// <summary> Print with a caller-context variable. </summary>
        public static void cprint(string s, MonoBehaviour obj)
        {
            print(obj.name + "::" + obj.GetType().ToString() + ">> " + s);
        }

        /// <summary> Print with a caller-context variable. </summary>
        public static void cprint(string s, GameObject obj)
        {
            print(obj.name + ">> " + s);
        }

        /// <summary> Print with a caller-context variable. </summary>
        public static void cprint(string s, string obj)
        {
            print(obj + ">> " + s);
        }

        /// <summary> Print success message with a caller-context variable. </summary>
        public static void csuccess(string s, MonoBehaviour obj)
        {
            success(obj.name + "::" + obj.GetType().ToString() + ">> " + s);
        }

        /// <summary> Print success message with a caller-context variable. </summary>
        public static void csuccess(string s, GameObject obj) {
            success(obj.name + ">> " + s);
        }

        /// <summary> Print success message with a caller-context variable. </summary>
        public static void csuccess(string s, string obj)
        {
            success(obj + ">> " + s);
        }

        /// <summary> Print warning message with a caller-context variable. </summary>
        public static void cwarn(string s, MonoBehaviour obj)
        {
            warn(obj.name + "::" + obj.GetType().ToString() + ">> " + s);
        }

        /// <summary> Print warning message with a caller-context variable. </summary>
        public static void cwarn(string s, GameObject obj)
        {
            warn(obj.name + ">> " + s);
        }

        /// <summary> Print warning message with a caller-context variable. </summary>
        public static void cwarn(string s, string obj)
        {
            warn(obj + ">> " + s);
        }

        /// <summary> Print error message with a caller-context variable. </summary>
        public static void cerror(string s, MonoBehaviour obj)
        {
            error(obj.name + "::" + obj.GetType().ToString() + ">> " + s);
        }

        /// <summary> Print error message with a caller-context variable. </summary>
        public static void cerror(string s, GameObject obj)
        {
            error(obj.name + ">> " + s);
        }

        /// <summary> Print error message with a caller-context variable. </summary>
        public static void cerror(string s, string obj)
        {
            error(obj + ">> " + s);
        }

        public static void raw_print(string s) {
            // Avoid duplicate outputs
            if(output_history.Count>0 && s==output_history.Last()) {
                repeat_counter++;
                return;
            }
            else {
                if(repeat_counter>1) output_s += " [x"+repeat_counter+"]";
                repeat_counter = 1;
            }

            output_history.Add(s);
            output_s += s;
        }

        /// <summary>
        /// Print a given string on the console.
        /// </summary>
        public static void print(string s)
        {
            raw_print("\n# " + s);
        }

        /// <summary>
        /// Print a success message string on the console.
        /// </summary>
        public static void success(string s)
        {
            raw_print("\n# <color=green><b>" + s + "</b></color>");
        }

        /// <summary>
        /// Print a given error string on the console.
        /// </summary>
        public static void error(string s)
        {
            raw_print("\n! <color=red><b>ERR:</b> " + s + "</color>");
        }

        /// <summary>
        /// Print a given warning string on the console.
        /// </summary>
        public static void warn(string s)
        {
            raw_print("\n! <color=yellow><b>WRN:</b> " + s + "</color>");
        }

        private static void echo_command(string s)
        {
            raw_print("\n> <color=grey> " + s + "</color>");
        }

        /// <summary>
        /// Returns any command executed in the past.
        /// <param name="t"> Index of the past command with t=0 being the most recent one. </param>
        /// </summary>
        public static string history(int t) {
            if(command_history.Count<1) return "";
            t = Mathf.Clamp(t, 0, command_history.Count-1);
            return command_history[t];
        }

        /// <summary>
        /// Execute a given console command.
        /// </summary>
        public static void command(string s, bool addToHistory=true) {
            if(addToHistory) {
                command_history.Insert(0, s);
                while(command_history.Count>command_history_limit) command_history.RemoveAt(0);
                echo_command(s);
            }

            s = s.Trim();

            string[] c = s.Split(' ');

            // Find the "best fit" - meaning the command whose min_args best fit the amount of given args
            List<AVR_ConsoleCommand> cmds = commands.Where((cmd) => cmd.name == c[0]).ToList();
            AVR_ConsoleCommand cmd = cmds.Where((cmd) => cmd.min_args <= c.Length - 1).OrderBy((cmd) => -cmd.min_args).FirstOrDefault();

            if(cmd != null && c.Length > cmd.min_args) {
                cmd.func(c.Skip(1).ToArray());
            }
            else if(cmd == null && cmds.Count > 0) {
                error("Command "+c[0]+" requires more arguments.");
            }
            else
            {
                error("Could not recognize command \"" + c[0] + "\"");
            }
        }

        /// <summary> Represents a command for the AVR_Devconsole </summary>
        public class AVR_ConsoleCommand {

            /// <summary> 
            /// Represents a command for the AVR_Devconsole
            /// <param name="name"> Identifier-string for the command. This is the token a user should input to execute the command. Should contain no spaces. </param>
            /// <param name="func"> Action performed when the command is executed. </param>
            /// <param name="min_args"> Minimum amount of arguments the command requires. If less are provided an error is given to the user. </param>
            /// <param name="desc"> Brief description of how the command works/what it does. Is provided when used with the "help" command. </param>
            /// </summary>
            public AVR_ConsoleCommand(string name, System.Action<string[]> func, int min_args = 0, string desc = "No description given.") {
                this.name = name;
                this.func = func;
                this.min_args = min_args;
                this.desc = desc;
            }
            public string name;
            public System.Action<string[]> func;
            public int min_args;
            public string desc;
        }

        /// <summary>
        /// Registers a new command to the console.
        /// </summary>
        /// <param name="cmd"> AVR_ConsoleCommand object that represents the command. </param>
        public static void register_command(AVR_ConsoleCommand cmd) {
            commands.Add(cmd);
        }

        /// <summary>
        /// Registers a new command to the console.
        /// </summary>
        /// <param name="name"> Identifier-string for the command. This is the token a user should input to execute the command. Should contain no spaces. </param>
        /// <param name="func"> Action performed when the command is executed. </param>
        /// <param name="min_args"> Minimum amount of arguments the command requires. If less are provided an error is given to the user. </param>
        /// <param name="desc"> Brief description of how the command works/what it does. Is provided when used with the "help" command. </param>
        public static void register_command(string name, System.Action<string[]> func, int min_args=0, string desc = "No description given.") {
            commands.Add(new AVR_ConsoleCommand(name, func, min_args, desc));
        }


        /// <summary>
        /// Registers a new command to the console.
        /// </summary>
        /// <param name="name"> Identifier-string for the command. This is the token a user should input to execute the command. Should contain no spaces. </param>
        /// <param name="func"> Action performed when the command is executed. </param>
        /// <param name="desc"> Brief description of how the command works/what it does. Is provided when used with the "help" command. </param>
        public static void register_command(string name, System.Action<string[]> func, string desc) {
            commands.Add(new AVR_ConsoleCommand(name, func, 0, desc));
        }

        private static void register_all_attribute_commands() {
            // Find any and all methods marked as "ConsoleCommand" in all assemplies:
            var methodsMarkedAsCommand =
                from a in System.AppDomain.CurrentDomain.GetAssemblies().AsParallel()
                    from t in a.GetTypes()
                        from m in t.GetMethods(System.Reflection.BindingFlags.Static | (System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
                            let attributes = m.GetCustomAttributes(typeof(AVR.Core.Attributes.ConsoleCommand), true)
                            where attributes != null && attributes.Length > 0
                            select new { method = m, attributes = attributes.Cast<AVR.Core.Attributes.ConsoleCommand>() };

            // Register each as a command:
            foreach(var result in methodsMarkedAsCommand) {
                foreach(var att in result.attributes) {
                    try {
                        // Single string function   static void func(string arg)
                        if (result.method.GetParameters().Count() == 1 && result.method.GetParameters()[0].ParameterType == typeof(System.String))
                        {
                            System.Action<string> action = (System.Action<string>)System.Delegate.CreateDelegate(typeof(System.Action<string>), result.method);
                            register_command(att.getCommandString(), (args) => action(args[0]), att.getMinArgs(), att.getDescription());
                        }
                        // String-array function   static void func(string[] args)
                        else if(result.method.GetParameters().Count()>0) {
                            System.Action<string[]> action = (System.Action<string[]>)System.Delegate.CreateDelegate(typeof(System.Action<string[]>), result.method);
                            register_command(att.getCommandString(), action, att.getMinArgs(), att.getDescription());
                        }
                        // No-parameter function   static void func()
                        else {
                            System.Action action = (System.Action)System.Delegate.CreateDelegate(typeof(System.Action), result.method);
                            register_command(att.getCommandString(), (args) => action(), att.getMinArgs(), att.getDescription());
                        }
                    }
                    catch (System.Exception) {
                        error("Following method is marked as ConsoleCommand, but doesnt follow the necessary requirements: " + result.method + "\n It needs to be static and void. Parameters have to be either none, a single string or a single array of strings.");
                    }
                }
            }
        }

        [AVR.Core.Attributes.ConsoleCommand("help", 1, "Provides a description for a given console command. Example: help echo")]
        private static void help_command(string arg) {
            bool found = false;
            foreach (AVR_ConsoleCommand cmd in commands)
            {
                if(cmd.name == arg) {
                    print("COMMAND: " + cmd.name);
                    print("MIN_ARGS: " + cmd.min_args);
                    print("DESCRIPTION: " + cmd.desc);
                    print("");
                    found = true;
                }
            }
            if(!found) print("Command "+arg+" does not exist. Type \"commands\" to see a list of all available commands.");
        }

        [AVR.Core.Attributes.ConsoleCommand("commands", "Provides a list of all available console commands")]
        private static void print_all_available_commands() {
            print("COMMAND \t\t MIN ARGS");
            foreach(AVR_ConsoleCommand cmd in commands) {
                print(cmd.name + "\t\t" + cmd.min_args);
            }
        }

        [AVR.Core.Attributes.ConsoleCommand("sample", "Sample command for demonstration purposes")]
        private static void sample_command(string[] args) {
            Debug.Log("This is a sample command.");
        }
    }
}
