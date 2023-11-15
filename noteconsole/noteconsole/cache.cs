using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace noteconsole
{
    internal partial class Program
    {
        static string cacheData;
        static string cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WindowsTerminalNoteEditor", "chache");
        static void LoadCache()
        {
            string chachePath = cachePath;
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
            string configPath = cachePath;

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
            File.WriteAllText(configPath, configData);
        }

        public static void AddToChache(string data)
        {
            cacheData = cacheData + "\n" + data;
            string chachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WindowsTerminalNoteEditor", "chache");
            File.WriteAllText(chachePath, cacheData);
        }

        static string GetValueForKey(string content, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(content))
                {
                    return null;
                }
                string[] lines = content.Split('\n');
                foreach (var line in lines)
                {
                    if (line.StartsWith(key))
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            return parts[1].Trim();
                        }
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
