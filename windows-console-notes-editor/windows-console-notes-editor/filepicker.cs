namespace windows_console_notes_editor
{
    internal partial class Program
    {
        static string path;
        static bool stopResizen = false;
        static int currentSelection = 0;
        static int startrender = 0;
        static List<string> data = new();
        static List<DriveInfo> removableDrives = new();

        static int maxheightConsole;
        static int maxwidthConsole;

        static string FilePicker(bool folderselection)
        {
            return Picker(folderselection);
        }

        static string Picker(bool folderselection)
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
                FetchFolderData(folderselection);
                FilePickRender(folderselection);
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
                        case var _
                            when (pressedKey.Key == ConsoleKey.DownArrow || pressedKey.Key == ConsoleKey.PageDown):
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
                                FetchFolderData(folderselection);
                            }
                            else
                            {
                                if (IsFileAllowedToOpen(path) && !folderselection)
                                {
                                    stopResizen = true;
                                    return path;
                                }
                                else
                                {
                                    path = GetParentDirectory(path);
                                }
                            }

                            break;
                        case var _ when (pressedKey.Key == ConsoleKey.LeftArrow):
                            path = GetParentDirectory(path);
                            startrender = 0;
                            currentSelection = 0;
                            FetchFolderData(folderselection);

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
                                FetchFolderData(folderselection);
                            }
                            else
                            {
                                if (IsFileAllowedToOpen(path) && !folderselection)
                                {
                                    stopResizen = true;
                                    return path;
                                }
                                else
                                {
                                    path = GetParentDirectory(path);
                                }
                            }

                            break;
                        case var _ when (pressedKey.Key == ConsoleKey.R):
                            FetchFolderData(folderselection);
                            FilePickRender(folderselection);
                            break;
                        case var _ when (pressedKey.Key == ConsoleKey.Home):
                            path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                            startrender = 0;
                            currentSelection = 0;
                            FetchFolderData(folderselection);
                            break;
                        case var _ when (pressedKey.Key == ConsoleKey.N):
                            if (folderselection)
                            {
                                stopResizen = true;
                                return path;
                            }

                            break;
                        case var _ when (pressedKey.Key == ConsoleKey.Escape):
                            return "";
                        case var _ when (pressedKey.Key == ConsoleKey.C && folderselection):
                            Console.Clear();
                            Console.Write("Please enter a name for the new folder: ");
                            string folderName = Console.ReadLine();
                            if (!Directory.Exists(Path.Combine(path, folderName)))
                            {
                                try
                                {
                                    Directory.CreateDirectory(Path.Combine(path, folderName));
                                    path = Path.Combine(path, folderName);
                                    FetchFolderData(true);
                                }
                                catch
                                {
                                }
                            }

                            break;
                    }

                    FilePickRender(folderselection);
                }

                stopResizen = true;
                return path;
            }
            catch (Exception e)
            {
                Console.WriteLine($"An unknown error occurred: \n{e.Message}\nStack-Trace:\n {e.StackTrace}");
                return null;
            }

            stopResizen = true;
            return null;
        }

        static bool IsFileAllowedToOpen(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            if (extension == ".txt" || extension == ".md" || extension == ".html" || extension == ".css" ||
                extension == ".json" || extension == ".xml" || extension == ".csv" || extension == ".log" ||
                extension == ".sql" || extension == ".yml" || extension == ".yaml" || extension == ".conf" ||
                extension == ".cfg" || extension == ".ini" || extension == ".properties" || extension == ".bat" ||
                extension == ".sh" || extension == ".php" || extension == ".js" || extension == ".py" ||
                extension == ".pl") return true;
            return false;
        }

        // Gets the maximum window size
        static void GetMaxWindowSize()
        {
            int buffer = Console.WindowHeight;
            maxheightConsole = buffer;
            maxwidthConsole = Console.WindowWidth;
            while (!stopResizen)
            {
                if (buffer != Console.WindowHeight)
                {
                    buffer = Console.WindowHeight;
                    maxheightConsole = buffer;
                    maxwidthConsole = Console.WindowWidth;
                    FilePickRender(false);
                }

                System.Threading.Thread.Sleep(500);
            }
        }


        static void FilePickRender(bool folderselection)
        {
            List<(string Text, ConsoleColor Color)> buffer = new();

            try
            {
                //Header
                var combinedText = $"{Path.Combine(path, data[currentSelection])}";

                if (combinedText.Length > maxwidthConsole)
                {
                    // Truncate the path to fit the console width with selectionText
                    var availableSpace = maxwidthConsole - combinedText.Length - 5; // 5 for space and to avoid clutter
                    combinedText = $"{combinedText.Substring(0, Math.Max(0, availableSpace))}...";
                }

                buffer.Add((combinedText, ConsoleColor.Yellow));

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
                            buffer.Add(("--> " + displayText, ConsoleColor.Yellow));
                        }
                        else
                        {
                            if (File.Exists(Path.Combine(path, data[index])))
                            {
                                buffer.Add(("    " + displayText, ConsoleColor.Cyan));
                            }
                            else
                            {
                                buffer.Add(("    " + displayText, ConsoleColor.DarkCyan));
                            }
                        }
                    }
                }

                //Scroll Indication
                ConsoleColor scrollColor = ConsoleColor.Yellow;
                if (data.Count() - startrender > maxheightConsole - 3 && startrender == 0)
                {
                    buffer.Add((">>>", scrollColor));
                }
                else if (data.Count() - startrender > maxheightConsole - 3 && startrender != 0)
                {
                    buffer.Add((">><", scrollColor));
                }
                else if (startrender != 0)
                {
                    buffer.Add(("<<<", scrollColor));
                }

                //Information when selecting folder
                if (folderselection)
                {
                    buffer.Add(("Press 'N' to create the file in this folder. 'C' to create a new folder.",
                        ConsoleColor.White));
                }

                // Display the entire buffer at once
                Console.Clear();
                foreach (var (text, color) in buffer)
                {
                    string adjustedText = AdjustTextToFit(text, maxwidthConsole);
                    Console.ForegroundColor = color;
                    Console.WriteLine(adjustedText);
                }

                Console.ResetColor();
            }
            catch (Exception er)
            {
                Console.Clear();
                Console.WriteLine(
                    "Error Encountered\r\nYou may not have the necessary permissions for this folder or file. Please:\r\n\r\n    Check your permissions.\r\n    Consider running the program as an administrator.\r\n\r\nIf you believe this is a different issue, press [e] to retrieve the error code.");
                if (Console.ReadKey().Key == ConsoleKey.E)
                {
                    Console.Clear();
                    Console.WriteLine(
                        $"Here is your error code. If this isn't a permission error, please submit a bug report at https://github.com/RebelCoderJames/console-windows-notes-editor/issues.\nThank you.\n\n{er}");
                    Thread.Sleep(100000);
                }

                path = GetParentDirectory(path);
                FetchFolderData(folderselection);
                FilePickRender(folderselection);
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

        public static void FetchFolderData(bool folderpicker)
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
                        string fileName = Path.GetFileName(file);
                        if (IsFileAllowedToOpen(Path.GetExtension(file)))
                        {
                            data.Add(fileName);
                        }
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