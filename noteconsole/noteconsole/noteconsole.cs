namespace noteconsole
{
    internal partial class Program
    {
        static void Main(string[] args)
        {
            //Loading Cache
            LoadCache();
            List<string> lastaccessed = new();
            if (GetValueForKey(cacheData, "last") != null)
            {
                lastaccessed = GetValueForKey(cacheData, "last").Split("|:|").ToList();
            }
            else
            {
                lastaccessed.Add("Nothing in here. Sorry");
            }

            string filepath = "";
            
            // TODO: Start the Background Service:
            Thread BackgroundServices = new(StartBackgroundServices);
            BackgroundServices.Start();
            
            // Check for filepath argument
            if (args.Length > 0 && File.Exists(Path.Combine(Directory.GetCurrentDirectory(), args[0])))
            {
                filepath = Path.Combine(Directory.GetCurrentDirectory(), args[0]);
            }
            else if (args.Length > 0 && File.Exists(args[0]))
            {
                filepath = args[0];
            }
            if (filepath != "") FileManager(filepath);

            Console.CursorVisible = false;

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Terminal.Clear();

                int consoleWidth = Console.WindowWidth;
                int consoleHeight = Console.WindowHeight;

                // Define the content
                string[] content =
                {
                    "             _                                 _      ",
                    "            | |                               | |     ",
                    " _ __   ___ | |_ ___  ___ ___  _ __  ___  ___ | | ___ ",
                    "| '_ \\ / _ \\| __/ _ \\/ __/ _ \\| '_ \\/ __|/ _ \\| |/ _ \\",
                    "| | | | (_) | ||  __/ (_| (_) | | | \\__ \\ (_) | |  __/",
                    "|_| |_|\\___/ \\__\\___|\\___\\___/|_| |_|___/\\___/|_|\\___|",
                    "",
                    "********************************",
                    "**        Welcome back!       **",
                    "********************************",
                    "",
                    "Press 'R' for recently used notes",
                    "Press 'S' to pick a file from your PC",
                    "Press 'N' to create a new note"
                };

                // Calculate vertical center
                int verticalCenter = (consoleHeight - content.Length) / 2;

                // Print content centered horizontally and vertically
                try
                {
                    for (int i = 0; i < content.Length; i++)
                    {
                        string line = content[i];
                        int horizontalPadding = (consoleWidth - line.Length) / 2;
                        Console.SetCursorPosition(horizontalPadding, verticalCenter + i);
                        Console.WriteLine(line);
                    }
                }
                catch
                {
                    Console.WriteLine("Noteconsole. N to create a new file. S to select a file. R to open recent files. Q to quit. NOTE: Window is to small to render welcome screen.");
                }

                ConsoleKeyInfo keyInfo;

                keyInfo = Console.ReadKey();
                Console.WriteLine();

                if (keyInfo.Key == ConsoleKey.R)
                {
                    try
                    {
                        filepath = DisplayRecentFiles();
                        if (filepath == null || filepath == "")
                        {
                            throw new Exception();
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.N)
                {
                    string documentsPath = FilePicker(true);
                    if (documentsPath == "") continue;
                    Terminal.Clear();
                    Console.WriteLine($"A new file will be created in {documentsPath}");
                    Console.Write("Enter the name of the new note: ");
                    string fileName = Console.ReadLine();

                    //Check if an extension got added.
                    string fileExtension = Path.GetExtension(fileName);
                    if (fileExtension == ".txt" || fileExtension == ".md" || fileExtension == ".html" ||
                        fileExtension == ".css" ||
                        fileExtension == ".json" || fileExtension == ".xml" || fileExtension == ".csv" ||
                        fileExtension == ".log" ||
                        fileExtension == ".sql" || fileExtension == ".yml" || fileExtension == ".yaml" ||
                        fileExtension == ".conf" || fileExtension == ".cfg" || fileExtension == ".ini" ||
                        fileExtension == ".properties" || fileExtension == ".bat" || fileExtension == ".sh" ||
                        fileExtension == ".php" || fileExtension == ".js" || fileExtension == ".py" ||
                        fileExtension == ".pl") ;
                    else
                    {
                        fileName += ".txt"; // Add .txt if non provided
                    }

                    string filePath = Path.Combine(documentsPath, fileName);

                    if (File.Exists(filePath))
                    {
                        Console.WriteLine($"The file '{fileName}' already exists.");
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        try
                        {
                            File.Create(filePath).Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error creating the file: {ex.Message}");
                            Thread.Sleep(1000);
                        }
                    }

                    filepath = filePath;
                    //Save it in lastaccessed
                    if (GetValueForKey(cacheData, "last") != null)
                    {
                        List<string> recentFiles = GetValueForKey(cacheData, "last")?.Split("|:|").ToList() ??
                                                   new List<string>();
                        if (recentFiles.Contains(filepath)) // If the path is in the cache already move it to the top
                        {
                            recentFiles.RemoveAll(item => item == filepath);
                            recentFiles.Insert(0, filepath);
                            string updatedLastAccessed = "";
                            foreach (string path in recentFiles)
                            {
                                updatedLastAccessed += path + "|:|";
                            }

                            ChangeCacheValue("last", updatedLastAccessed);
                        }
                        else
                        {
                            ChangeCacheValue("last", filepath + "|:|" + (GetValueForKey(cacheData, "last")));
                        }
                    }
                    else
                    {
                        AddToChache($"last = {filepath}|:|");
                    }
                }
                else if (keyInfo.Key == ConsoleKey.S)
                {
                    filepath = FilePicker(false);
                    if (filepath == "") continue;
                    Terminal.Clear();
                    Console.WriteLine($"{filepath} is opening...");
                    if (filepath == null || filepath == "")
                    {
                        continue;
                    }

                    //Save it in lastaccessed
                    if (GetValueForKey(cacheData, "last") != null)
                    {
                        List<string> recentFiles = GetValueForKey(cacheData, "last")?.Split("|:|").ToList() ??
                                                   new List<string>();
                        if (recentFiles.Contains(filepath)) // If the path is in the cache already move it to the top
                        {
                            recentFiles.RemoveAll(item => item == filepath);
                            recentFiles.Insert(0, filepath);
                            string updatedLastAccessed = "";
                            foreach (string path in recentFiles)
                            {
                                updatedLastAccessed += path + "|:|";
                            }

                            ChangeCacheValue("last", updatedLastAccessed);
                        }
                        else
                        {
                            ChangeCacheValue("last", filepath + "|:|" + (GetValueForKey(cacheData, "last")));
                        }
                    }
                    else
                    {
                        AddToChache($"last = {filepath}|:|");
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Q)
                {
                    Terminal.Clear();
                    Console.ResetColor();
                    Console.WriteLine("Bye");
                    break;
                }
                else
                {
                    filepath = "";
                }

                if (filepath != "") FileManager(filepath);
            }
            Environment.ExitCode = 0; // Exit application
            Environment.Exit(Environment.ExitCode);
        }

        static string DisplayRecentFiles()
        {
            Terminal.Clear();
            Console.WriteLine("Recently used notes:");
            LoadCache();

            string recentFilesData = GetValueForKey(cacheData, "last");
            List<string> recentFiles = recentFilesData?.Split("|:|").ToList() ?? new List<string>();

            //Remove Empty Values
            recentFiles.RemoveAll(item => item == "");

            if (recentFiles.Count == 0)
            {
                Console.WriteLine("No recently used notes found.");
                Console.WriteLine("Press Enter to return to the previous view.");
                while (true)
                {
                    var key = Console.ReadKey().Key;
                    if (key == ConsoleKey.Enter || key == ConsoleKey.Escape)
                        return null;
                }
            }

            int selectedIndex = 0;
            while (true)
            {
                Terminal.Clear();
                Console.WriteLine("Recently used notes:");

                for (int i = 0; i < recentFiles.Count; i++)
                {
                    if (i == selectedIndex)
                        Console.Write(">> ");
                    Console.WriteLine($"{i + 1}. {recentFiles[i]}");
                }

                Console.WriteLine(
                    "Press 'UpArrow' for previous file, 'DownArrow' for next file, \n'Enter' to select, or 'Esc' to return to the previous view.");

                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.UpArrow)
                {
                    selectedIndex = Math.Max(0, selectedIndex - 1);
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    selectedIndex = Math.Min(recentFiles.Count - 1, selectedIndex + 1);
                }
                else if (key == ConsoleKey.Enter)
                {
                    string selectedFile = recentFiles[selectedIndex];

                    // Move the selected file to the top
                    recentFiles.Remove(selectedFile);
                    recentFiles.Insert(0, selectedFile);

                    string updatedLastAccessed = string.Join("|:|", recentFiles);
                    ChangeCacheValue("last", updatedLastAccessed);

                    return selectedFile;
                }
                else if (key == ConsoleKey.Escape)
                {
                    return null;
                }
            }
        }
    }
}