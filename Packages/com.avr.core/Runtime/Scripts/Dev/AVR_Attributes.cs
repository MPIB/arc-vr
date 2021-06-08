using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Contains C#-Attributes for various uses.
/// </summary>
namespace AVR.Core.Attributes {

    /// <summary>
    /// This attribute can be used to easily make a function into a command runnable through the AVR_DevConsole.
    /// This can only be used on *static* void-functions where the parameters are eithe none, a single string or an array of strings.
    /// Usage examples:
    ///
    /// This will add "run_command" as a command that outputs a hello world message. The second parameter in the constructor provides a description, which the user can see by using the "help" command. Providing a description is not mandatory but recommended.
    /// \code
    /// [AVR.Core.Attributes.ConsoleCommand("run_command", "Prints a hello world")]
    /// static void run_command() {
    ///     print("Hello World");
    /// }
    /// \endcode
    ///
    /// This examples creates a simple "echo" command, which outputs the first argument given to the command. The second parameter (1), defines the minimum amount of arguments a command requires. Providing less will output a warning message to the user.
    /// \code
    /// [AVR.Core.Attributes.ConsoleCommand("echo_command", 1, "Replies with the same string")]
    /// static void echo_command(string arg) {
    ///     print(arg);
    /// }
    /// \endcode
    ///
    /// This example echoes the second parameter passed. (So the command "echo_second_word hello world" will output "world").
    /// \code
    /// [AVR.Core.Attributes.ConsoleCommand("echo_second_word", 2)]
    /// static void echo_second_word(string[] args) {
    ///     print(args[1]);
    /// }
    /// \endcode
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class ConsoleCommand : Attribute {
        private string command_string;
        private string desc;
        private int min_args;

        /// <summary>
        /// Defines a method as a command vor the AVR_DevConsole.
        /// <param name="command_string"> String which represents the command. This is the same string a user inputs to run it. </param>
        /// <param name="min_args"> Minimum amount of arguments a user must provide for the command to run. </param>
        /// <param name="desc"> Brief description that explains what the command does and how it is used. </param>
        /// </summary>
        public ConsoleCommand(string command_string, int min_args = 0, string description = "No description given.") {
            this.command_string = command_string;
            this.min_args = min_args;
            this.desc = description;
        }

        /// <summary>
        /// Defines a method as a command vor the AVR_DevConsole.
        /// <param name="command_string"> String which represents the command. This is the same string a user inputs to run it. </param>
        /// <param name="desc"> Brief description that explains what the command does and how it is used. </param>
        /// </summary>
        public ConsoleCommand(string command_string, string description)
        {
            this.command_string = command_string;
            this.min_args = 0;
            this.desc = description;
        }

        public string getCommandString() => command_string;

        public int getMinArgs() => min_args;

        public string getDescription() => desc;
    }


    /// <summary>
    /// Sets the documentation html file inside of Packages/com.avr.core/Documentation/html of a given class. When the user clicks on the documentation button in the inspector the respective page will be opened.
    /// Example:
    /// \code
    /// [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___controller.html")]
    /// public class AVR_Controller : AVR_GenericXRDevice
    /// { ... }
    /// \endcode
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class DocumentationUrl : Attribute {
        string url;

        /// <summary>
        /// Defines the html file used for documentation for an AVR_Component.
        /// <param name="url"> File relative to Packages/com.avr.core/Documentation/html that is the documentation for this class. </param>
        /// </summary>
        public DocumentationUrl(string url) {
            this.url = url;
        }

        public string getDocumentationUrl() {
            return url;
        }
    }


    /// <summary>
    /// Allows for simple hiding of properties in the UnityEditor depending on certain conditions.
    /// For instance, in the following example the "type" field will only be displayed in the inspector if the "tracking" field is set to true:
    /// \code
    /// public bool tracking = false;
    ///
    /// [AVR.Core.Attributes.ConditionalHideInInspector("tracking", true)]
    /// public TrackingType type;
    /// \endcode
    /// <param name="hideConditionPropertyName"> The first parameter is always the name of the field that represents the condition. The name is case-sensitive. </param>
    /// <param name="invertCondition"> Will invert the given condition. Hence why in the example above, tracking = false hides the type field. </param>
    /// <param name="compareValue"> Optionally, one can compare the given field with some given value. The type of comparison is determined by: </param>
    /// <param name="ctype"> Type of comparison performed if a compareValue is provided. Examples: </param>
    /// \code
    /// // Hides the variable if "counter" is larger than 3
    /// [AVR.Core.Attributes.ConditionalHideInInspector("counter", 3, AVR.Core.Attributes.ConditionalHideInInspector.compareType.BIGGER)]
    ///
    /// // Hides the variable if "type" is equal to TypeEnum.TYPE2. Since "EQUAL" is the default comparison mode, the parameter is optional.
    /// [AVR.Core.Attributes.ConditionalHideInInspector("type", (int)TypeEnum.TYPE2)]
    /// \endcode
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class ConditionalHideInInspector: PropertyAttribute
    {
        public enum compareType { EQUAL, BIGGER, SMALLER, BIGGEREQUAL, SMALLEREQUAL, UNEQUAL };

        public compareType ctype { get; private set; }
        public string hideConditionPropertyName { get; private set; }
        public bool invertCondition { get; private set; }
        public float compareValue { get; private set; }

        /// <summary>
        /// Hides a field in the Inspector if a given condition is true.
        /// <param name="hideConditionPropertyName"> The first parameter is always the name of the field that represents the condition. The name is case-sensitive. </param>
        /// <param name="invertCondition"> Will invert the given condition. Hence why in the example above, tracking = false hides the type field. </param>
        /// </summary>
        public ConditionalHideInInspector(string hideConditionPropertyName, bool invertCondition = false)
        {
            this.hideConditionPropertyName = hideConditionPropertyName;
            this.invertCondition = invertCondition;
            this.ctype = compareType.BIGGEREQUAL;
            compareValue = 1.0f;
        }

        /// <summary>
        /// Hides a field in the Inspector if a given condition is true.
        /// <param name="hideConditionPropertyName"> The first parameter is always the name of the field that represents the condition. The name is case-sensitive. </param>
        /// <param name="invertCondition"> Will invert the given condition. Hence why in the example above, tracking = false hides the type field. </param>
        /// <param name="compareValue"> Optionally, one can compare the given field with some given value. The type of comparison is determined by: </param>
        /// <param name="ctype"> Type of comparison performed if a compareValue is provided. Examples: </param>
        /// </summary>
        public ConditionalHideInInspector(string hideConditionPropertyName, float compareValue, compareType ctype = compareType.EQUAL, bool invertCondition = false)
        {
            this.hideConditionPropertyName = hideConditionPropertyName;
            this.invertCondition = invertCondition;
            this.compareValue = compareValue;
            this.ctype = ctype;
        }

        /// <summary>
        /// Hides a field in the Inspector if a given condition is true.
        /// <param name="hideConditionPropertyName"> The first parameter is always the name of the field that represents the condition. The name is case-sensitive. </param>
        /// <param name="invertCondition"> Will invert the given condition. Hence why in the example above, tracking = false hides the type field. </param>
        /// <param name="compareValue"> Optionally, one can compare the given field with some given value. The type of comparison is determined by: </param>
        /// <param name="ctype"> Type of comparison performed if a compareValue is provided. Examples: </param>
        /// </summary>
        public ConditionalHideInInspector(string hideConditionPropertyName, int compareValue, compareType ctype = compareType.EQUAL, bool invertCondition = false)
        {
            this.hideConditionPropertyName = hideConditionPropertyName;
            this.invertCondition = invertCondition;
            this.compareValue = compareValue;
            this.ctype = ctype;
        }
    }

    /// <summary>
    /// Makes a field read-only in the inspector
    /// </summary>
    public class ReadOnly : PropertyAttribute
    {

    }
}
