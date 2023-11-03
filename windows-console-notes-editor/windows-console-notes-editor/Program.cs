namespace windows_console_notes_editor
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

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();

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
                for (int i = 0; i < content.Length; i++)
                {
                    string line = content[i];
                    int horizontalPadding = (consoleWidth - line.Length) / 2;
                    Console.SetCursorPosition(horizontalPadding, verticalCenter + i);
                    Console.WriteLine(line);
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
                    if (documentsPath == "") break;
                    Console.Clear();
                    Console.WriteLine($"A new file will be created in {documentsPath}");
                    Console.Write("Gib den Namen der neuen Notiz ein: ");
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
                        Console.WriteLine($"Die Datei '{fileName}' existiert bereits.");
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
                            Console.WriteLine($"Fehler beim Erstellen der Datei: {ex.Message}");
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
                    Console.Clear();
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

                FileManager(filepath);
            }
        }

        static string DisplayRecentFiles()
        {
            Console.Clear();
            Console.WriteLine("Kürzlich verwendete Notizen:");
            LoadCache();

            string recentFilesData = GetValueForKey(cacheData, "last");
            List<string> recentFiles = recentFilesData?.Split("|:|").ToList() ?? new List<string>();

            if (recentFiles.Count == 0)
            {
                Console.WriteLine("Keine kürzlich verwendeten Notizen gefunden.");
                Console.WriteLine("Drücke Enter, um zur vorherigen Ansicht zurückzukehren.");
                while (true)
                {
                    var key = Console.ReadKey().Key;
                    if (key == ConsoleKey.Enter)
                        return null;
                }
            }

            int selectedIndex = 0;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Kürzlich verwendete Notizen:");

                for (int i = 0; i < recentFiles.Count; i++)
                {
                    if (i == selectedIndex)
                        Console.Write(">> ");
                    Console.WriteLine($"{i + 1}. {recentFiles[i]}");
                }

                Console.WriteLine("Drücke 'UpArrow' für vorherige Datei, 'DownArrow' für nächste Datei,");
                Console.WriteLine("'Enter', um auszuwählen, oder 'Esc', um zur vorherigen Ansicht zurückzukehren.");

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