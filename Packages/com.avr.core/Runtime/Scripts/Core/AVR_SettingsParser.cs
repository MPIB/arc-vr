using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

namespace AVR.Core {
    /// <summary>
    /// Parser used to parse setting-files.
    /// </summary>
    public class AVR_SettingsParser
    {
        /// <summary>
        /// Automatically parse all settingsfiles found that have the *.avr suffix. Duplicate settings are overwritten in alphabetical order of the filename.
        /// </summary>
        /// <param name="exclude_overridesettings"> If set to true, any file with the name "~overridesettings.avr" will be excluded. </param>
        /// <returns> Dictionary of all settings. </returns>
        public static Dictionary<string, string> AutoParseSettingFiles(bool exclude_overridesettings=false) {
            string[] files = System.IO.Directory.GetFiles(Application.dataPath+"/..", "*.avr", SearchOption.AllDirectories);
            System.Array.Sort(files, (a, b) => string.Compare(b, a));

            Dictionary<string, string> entries = new Dictionary<string, string>();
            foreach(string file in files) {
                if(exclude_overridesettings && file.Contains("~overridesettings.avr")) continue;
                entries = ParseSettings(file, entries);
            }
            return entries;
        }

        /// <summary>
        /// Parse a single given settings-file.
        /// </summary>
        /// <param name="filepath"> Path of the file in question </param>
        /// <param name="entries"> pre-existing entries to add to. Duplicates will be overwritten. </param>
        /// <returns> Dictionary containing all settings from entries as well as the given file. </returns>
        public static Dictionary<string, string> ParseSettings(string filepath, Dictionary<string, string> entries = null) {
            AVR.Core.AVR_DevConsole.print("Parsing settingsfile: "+filepath);
            string content = "";
            try {
                content = File.ReadAllText(filepath);
            } catch(System.Exception) {
                AVR.Core.AVR_DevConsole.error("Could not open settings file: " + filepath);
                return entries;
            }

            //First we remove all the comments (From hashtag to newline. TODO: Does this even work on Linux/MAC?
            content = Regex.Replace(content, "#.*(\\r\\n|\\n|\\r)", "");

            //This regex splits the input at whitespaces, only if they are followed by an even number of quotation marks. See: https://stackoverflow.com/questions/25477562/split-java-string-by-space-and-not-by-double-quotation-that-includes-space
            List<string> tokens = new List<string>(Regex.Split(content, "(?<!\\G\\S{0,99999}[\"'].{0,99999})\\s|(?<=\\G\\S{0,99999}\".{0,99999}\"\\S{0,99999})\\s|(?<=\\G\\S{0,99999}'.{0,99999}'\\S{0,99999})\\s"));

            // Clean up: (Delete empty tokens)
            for(int i=0; i<tokens.Count; i++) {
                tokens[i] = tokens[i].Trim().Trim('\"');
                if(tokens[i].Length<1) {
                    tokens.RemoveAt(i);
                    i--;
                }
            }

            // Parse tokens
            if(entries==null) entries = new Dictionary<string, string>();
            parseDepth("", 0, tokens, entries);

            return entries;
        }

        // Recusrive parsing function.
        private static int parseDepth(string context, int i, List<string> content, Dictionary<string, string> entries) {
            while (i < content.Count)
            {
                if (content[i] == "}") return i + 1;
                // For custom tokens
                //else if (content[i].equals("$embededCSV") && content[i + 1].equals("="))
                //{
                //    parseEmbededCSV(context, content[i + 2]);
                //    i += 3;
                //}
                //else if (content[i].equals("$embededJSON") && content[i + 1].equals("="))
                //{
                //    parseEmbededJSON(context, content[i + 2]);
                //    i += 3;
                //}
                else if (is_identifier(content[i]) && content[i + 1]=="=" && content[i + 2]=="{")
                {
                    i = parseDepth(context + "/" + content[i], i + 3, content, entries);
                }
                else if (is_identifier(content[i]) && content[i + 1]=="=")
                { //We dont care if content[i+2] is an identifier, because quotation-strings are also values.
                    entries[context + "/" + content[i]] = content[i + 2];
                    i += 3;
                }
                else
                {
                    AVR.Core.AVR_DevConsole.cerror("Incorrect Token Syntax at (index "+i+"): " + content[i] + " " + content[i + 1] + " " + content[i + 2], "SettingsParser");
                    return int.MaxValue;
                }
            }
            return i;
        }

        // Determines if a given token is a valid identifier.
        private static bool is_identifier(string str)
        {
            return Regex.IsMatch(str, "\\w+");
        }
    }
}
