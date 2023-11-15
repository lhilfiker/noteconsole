using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// This is the initial implementation of the config file.
// For how to load/save to config see: https://github.com/RebelCoderJames/console-windows-file-explorer/blob/v3/Windows%20Terminal%20File%20Explorer/Windows%20Terminal%20File%20Explorer/config.cs
// Author: RebelCoderJames

namespace noteconsole
{
    internal partial class Program

    {
        // Default Configuration settings when loading them from the config File fails.

        // Define display colors for various UI elements
        public static List<ConsoleColor> config = new()
        {
            
        };
        public static List<ConsoleColor?> confignull = new()
        {
            
        };



        // End of configuration 

        public static string defaultconfig; // Default Configuration settings when loading them from the config File fails.
        public static void ChangeConfigValue(string key, string newValue)
        {
            string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WindowsTerminalNoteEditor", "config.conf");

            // Check if the file exists
            if (!File.Exists(configPath))
            {
                return;
            }

            // Read the file content
            string configData = File.ReadAllText(configPath);

            // Replace the specific key's value with the new value
            string pattern = $"{key} *= *[^\\n]*"; // This pattern will match "key = value" pattern in the config file
            string replacement = $"{key} = {newValue}";
            string updatedConfigData = Regex.Replace(configData, pattern, replacement);

            // If no replacement occurred (i.e., the key doesn't exist), append the new key-value pair to the end
            if (updatedConfigData == configData)
            {
                updatedConfigData += $"\n{key} = {newValue}";
            }


            // Write the updated data back to the file
            File.WriteAllText(configPath, updatedConfigData);
        }
        static void loadConfig()
        {
            string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WindowsTerminalNoteEditor", "config.conf");
            try
            {
                if (File.Exists(configPath)) //Load the config
                {
                    string configData = File.ReadAllText(configPath);
                    confignull.Clear();
                    
                    // Load the values from the config file

                    if (false) // Check if the config file is valid
                    {
                        throw new Exception();
                    }
                    // Clear the original config
                    config.Clear();

                    // Add every non-null item from configNull to config
                    foreach (var color in confignull)
                    {
                        if (color.HasValue)  // Check if the item is non-null
                        {
                            config.Add(color.Value);
                        }
                    }

                }
                else //create the config
                {
                    string defaultConfig = defaultconfig;
                    string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WindowsTerminalNoteEditor");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    File.WriteAllText(configPath, defaultConfig);
                }
            }
            catch // Set to default if an error occours when loading the config.
            {

                var defaultColors = new List<ConsoleColor?>
                {
                    ConsoleColor.Yellow,
                    ConsoleColor.Cyan,
                    ConsoleColor.DarkCyan,
                    ConsoleColor.Gray,
                    ConsoleColor.DarkGray,
                    ConsoleColor.Yellow,
                    ConsoleColor.Red,
                    ConsoleColor.Magenta,
                    ConsoleColor.DarkMagenta,
                    ConsoleColor.Green
                };

                var colorNames = new List<string>
                {
                    "infocolor",
                    "filecolor",
                    "foldercolor",
                    "inputcolor",
                    "recommcolor",
                    "highlightcolor",
                    "errorcolor",
                    "picturecolor",
                    "videocolor",
                    "executablecolor"
                };

                for (int i = 0; i < 9; i++)
                {
                    if (confignull[i] == null)
                    {
                        confignull[i] = (ConsoleColor)defaultColors[i];
                        ChangeConfigValue(colorNames[i], defaultColors[i]?.ToString());
                    }
                }

                // Clear the original config
                config.Clear();

                // Add every non-null item from configNull to config
                foreach (var color in confignull)
                {
                    if (color.HasValue)  // Check if the item is non-null
                    {
                        config.Add(color.Value);
                    }
                }

                
            }


        }

        public static ConsoleColor StringToConsoleColor(string colorName)
        {
            if (Enum.TryParse(colorName, true, out ConsoleColor result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException($"Invalid ConsoleColor: {colorName}");
            }
        }
    }
}
