using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.XPath;
using System.Xml;
using System.Linq.Expressions;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace windows_console_notes_editor
{
    internal partial class Program
    {
        static string FilePicker()
        {
            return Picker();
        }

        static string path;
        static int currentSelection = 0;
        static int startrender = 0;
        static List<string> data = new();
        static List<DriveInfo> removableDrives = new();

        static string Picker()
        {

            try
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                // Start the Function to get the max window size
                Thread resizen = new(GetMaxWindowSize);
                resizen.Start();
                // Other variables
                ConsoleKeyInfo pressedKey;
                // Inital fetching Path Data
                FetchFolderData();
                FilePickRender();
                while (!File.Exists(path))
                {
                    pressedKey = Console.ReadKey(intercept: true);
                    switch (pressedKey.Key)
                    {
                        case ConsoleKey.UpArrow when pressedKey.Modifiers == ConsoleModifiers.Control:
                            currentSelection = 0;
                            startrender = 0;
                            break;
                        case ConsoleKey.DownArrow when pressedKey.Modifiers == ConsoleModifiers.Control:
                            currentSelection = data.Count - 2;
                            startrender = currentSelection + 6 - maxheightConsole;
                            if (startrender < 0)
                            {
                                startrender = 0;
                            }
                            break;
                        case var _ when (pressedKey.Key == ConsoleKey.UpArrow || pressedKey.Key == ConsoleKey.PageUp):
                            if (currentSelection != 0)
                            {
                                currentSelection--;
                            }
                            if (startrender == currentSelection && startrender != 0)
                            {
                                startrender--;
                            }
                            Thread.Sleep(20);
                            break;
                        case var _ when (pressedKey.Key == ConsoleKey.DownArrow || pressedKey.Key == ConsoleKey.PageDown):
                            if (currentSelection + 2 < data.Count)
                            {
                                currentSelection++;
                            }
                            if (currentSelection + 2 - startrender >= maxheightConsole - 3)
                            {
                                startrender++;
                            }
                            Thread.Sleep(20);
                            break;
                        case var _ when (pressedKey.Key == ConsoleKey.RightArrow):
                            if (path == "")
                            {
                                if (data[currentSelection] != "")
                                {
                                    path = removableDrives[currentSelection].Name;
                                    currentSelection = 0;
                                    startrender = 0;
                                }
                            }
                            else
                            {
                                path = Path.Combine(path, data[currentSelection]);
                            }


                            if (Directory.Exists(path))
                            {
                                startrender = 0;
                                currentSelection = 0;
                                FetchFolderData();
                            }
                            else
                            {
                                return path;
                            }

                            break;
                        case var _ when (pressedKey.Key == ConsoleKey.LeftArrow):
                            path = GetParentDirectory(path);
                            startrender = 0;
                            currentSelection = 0;
                            FetchFolderData();

                            break;
                        case var _ when (pressedKey.Key == ConsoleKey.Enter):
                            if (path == "")
                            {
                                if (data[currentSelection] != "")
                                {
                                    path = removableDrives[currentSelection].Name;
                                    currentSelection = 0;
                                    startrender = 0;
                                }
                            }
                            else
                            {
                                path = Path.Combine(path, data[currentSelection]);
                            }


                            if (Directory.Exists(path))
                            {
                                startrender = 0;
                                currentSelection = 0;
                                FetchFolderData();
                            }
                            else
                            {
                                return path;
                            }
                            break;
                        case var _ when (pressedKey.Key == ConsoleKey.R):
                            FetchFolderData();
                            FilePickRender();
                            break;
                        case var _ when (pressedKey.Key == ConsoleKey.Home):
                            path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                            startrender = 0;
                            currentSelection = 0;
                            FetchFolderData();
                            break;

                    }
                    FilePickRender();
                }
                return path;
            }
            catch (Exception e)
            {
                
                Console.WriteLine($"An unknown error occurred: \n{e.Message}\nStack-Trace:\n {e.StackTrace}");
                return null;
                
            }
            return null;
        }

        static int maxheightConsole;
        static int maxwidthConsole;

        // Gets the maximum window size
        static void GetMaxWindowSize()
        {
            int buffer = Console.WindowHeight;
            maxheightConsole = buffer;
            maxwidthConsole = Console.WindowWidth;
            while (true)
            {
                if (buffer != Console.WindowHeight)
                {
                    buffer = Console.WindowHeight;
                    maxheightConsole = buffer;
                    maxwidthConsole = Console.WindowWidth;
                    FilePickRender();
                }
                System.Threading.Thread.Sleep(500);
            }
        }

        static void FilePickRender()
        {
            List<string> buffer = new();

            try
            {
                //Header
                if (path == "")
                {
                    var combinedText = $"{Path.Combine(path, data[currentSelection])}";
                    if (combinedText.Length > maxwidthConsole)
                    {
                        // Truncate the path to fit the console width with selectionText
                        var availableSpace = maxwidthConsole - combinedText.Length - 5; // 5 for "[tab] " space and to avoid clutter
                        combinedText = $"{combinedText.Substring(0, Math.Max(0, availableSpace))}...";
                    }

                    buffer.Add(combinedText);
                }
                else
                {
                    var combinedText = $"{Path.Combine(path, data[currentSelection])}";

                    if (combinedText.Length > maxwidthConsole)
                    {
                        // Truncate the path to fit the console width with selectionText
                        var availableSpace = maxwidthConsole - combinedText.Length - 5; // 5 for "[tab] " space and to avoid clutter
                        combinedText = $"{combinedText.Substring(0, Math.Max(0, availableSpace))}...";
                    }

                    buffer.Add(combinedText);
                }

                int linesLeft = maxheightConsole - 4;
                //Files/Folders
                for (int i = -1; i < linesLeft - 1; i++)
                {
                    int index = i + startrender;
                    if (index >= 0 && index < data.Count)
                    {
                        string displayText = data[index];
                        if (index == currentSelection)
                        {
                            buffer.Add("--> " + displayText);
                        }
                        else
                        {
                            buffer.Add("    " + displayText);
                        }
                    }
                }

                //Scroll Indication
                if (data.Count() - startrender > maxheightConsole - 3 && startrender == 0)
                {
                    buffer.Add(">>>");
                }
                else if (data.Count() - startrender > maxheightConsole - 3 && startrender != 0)
                {
                    buffer.Add(">><");
                }
                else if (startrender != 0)
                {
                    buffer.Add("<<<");
                }

                // Display the entire buffer at once
                Console.Clear();
                foreach (var entry in buffer)
                {
                    string adjustedText = AdjustTextToFit(entry, maxwidthConsole);
                    Console.WriteLine(adjustedText);
                }
            }
            catch (Exception er)
            {
                Console.Clear();
                Console.WriteLine("Error Encountered\r\nYou may not have the necessary permissions for this folder or file. Please:\r\n\r\n    Check your permissions.\r\n    Consider running the program as an administrator.\r\n\r\nIf you believe this is a different issue, press [e] to retrieve the error code.");
                if (Console.ReadKey().Key == ConsoleKey.E)
                {
                    Console.Clear();
                    Console.WriteLine($"Here is your error code. If this isn't a permission error, please submit a bug report at https://github.com/RebelCoderJames/console-windows-notes-editor/issues.\nThank you.\n\n{er}");
                    Thread.Sleep(100000);
                }
                path = GetParentDirectory(path);
                FetchFolderData();
                FilePickRender();
            }
        }

        static string AdjustTextToFit(string? text, int maxWidth)
        {
            if (text.Length <= maxWidth)
            {
                return text;
            }

            // Truncate the text with "..." to indicate it's been shortened
            return text.Substring(0, maxWidth - 3) + "...";
        }

        static string GetParentDirectory(string path)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);

                // Check if the path is a root directory of a drive
                if (directoryInfo.FullName.TrimEnd('\\') == directoryInfo.Root.FullName.TrimEnd('\\'))
                {
                    return "";
                }

                if (directoryInfo.Parent != null)
                {
                    return directoryInfo.Parent.FullName;
                }

                // Handle other cases, e.g., return the original path or an empty string
                return path;
            }
            catch
            {
                return path;
            }
            return path;
        }

        public static void FetchFolderData()
        {
            data.Clear();
            try
            {
                if (path == "")
                {
                    removableDrives.Clear();
                    removableDrives = DriveInfo.GetDrives()
                        .Where(drive => drive.DriveType == DriveType.Removable || drive.DriveType == DriveType.Fixed)
                        .ToList();
                    data.Clear();
                    foreach (DriveInfo device in removableDrives)
                    {
                        try
                        {
                            data.Add(device.Name + "  " + device.VolumeLabel);
                        }
                        catch (IOException)
                        {
                        }
                    }
                    data.Add("");
                    data.Add("");
                }
                else
                {
                    foreach (var directory in Directory.GetDirectories(path))
                    {
                        data.Add(Path.GetFileName(directory));
                    }

                    foreach (var file in Directory.GetFiles(path))
                    {
                        data.Add(Path.GetFileName(file));
                    }
                    data.Add("");
                    data.Add("");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }
        }

    }
}
