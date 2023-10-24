using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace windows_console_notes_editor
{
    internal partial class Program
    {
        static string cacheData;
        public static void LoadCache()
        {
            string chachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WindowsTerminalNoteEditor", "chache");
            if (File.Exists(chachePath)) //Load the chache
            {
                cacheData = File.ReadAllText(chachePath);

            }
            else //create the cache file
            {
                string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WindowsTerminalNoteEditor");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                File.WriteAllText(chachePath, "");
            }
        }
        static void ChangeCacheValue(string key, string newValue)
        {
            string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WindowsTerminalNoteEditor", "cache");

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
            configData = Regex.Replace(configData, pattern, replacement);

            // Write the updated data back to the file
            File.WriteAllText(configPath, cacheData);
        }

        public static void AddToChache(string data)
        {
            cacheData = cacheData + "\n" + data;
            string chachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WindowsTerminalNoteEditor", "chache");
            File.WriteAllText(chachePath, cacheData);
        }
    }
}
