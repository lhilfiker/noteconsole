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
                string[] content = {
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

                do
                {
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
                            break;
                        }
                        catch
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Clear();

                            Console.WriteLine("             _                                 _      ");
                            Console.WriteLine("            | |                               | |     ");
                            Console.WriteLine(" _ __   ___ | |_ ___  ___ ___  _ __  ___  ___ | | ___ ");
                            Console.WriteLine("| '_ \\ / _ \\| __/ _ \\/ __/ _ \\| '_ \\/ __|/ _ \\| |/ _ \\");
                            Console.WriteLine("| | | | (_) | ||  __/ (_| (_) | | | \\__ \\ (_) | |  __/");
                            Console.WriteLine("|_| |_|\\___/ \\__\\___|\\___\\___/|_| |_|___/\\___/|_|\\___|");
                            Console.WriteLine("                                                      ");
                            Console.WriteLine("********************************");
                            Console.WriteLine("*      Willkommen zurück!      *");
                            Console.WriteLine("********************************");
                            Console.WriteLine("Drücke 'R' für kürzlich verwendete Notizen.");
                            Console.WriteLine("Drücke 'S' um eine Datei aus deinem Computer auszuwählen");
                            Console.WriteLine("Drücke 'N' für eine neue Notiz.");
                        }
                    }
                    else if (keyInfo.Key == ConsoleKey.N)
                    {
                        Console.WriteLine("********************************");
                        Console.WriteLine("*   Erstelle eine neue Notiz   *");
                        Console.WriteLine("********************************");
                        string documentsPath = FilePicker(true);
                        if (documentsPath == "") break;
                        Console.WriteLine($"A new file will be created in {documentsPath}");
                        Console.Write("Gib den Namen der neuen Notiz ein: ");
                        string fileName = Console.ReadLine();

                        string filePath = Path.Combine(documentsPath, fileName);

                        if (File.Exists(filePath))
                        {
                            Console.WriteLine($"Die Datei '{fileName}' existiert bereits.");
                        }
                        else
                        {
                            try
                            {
                                File.Create(filePath).Close();
                                Console.WriteLine($"Die Datei '{fileName}' wurde erfolgreich erstellt.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Fehler beim Erstellen der Datei: {ex.Message}");
                            }
                        }
                        filepath = filePath;
                        break;
                    }
                    else if (keyInfo.Key == ConsoleKey.S)
                    {
                        filepath = FilePicker(false);
                        if (filepath == "") break;
                        Console.Clear();
                        Console.WriteLine($"{filepath} is opening...");
                        //Save it in lastaccessed
                        if (GetValueForKey(cacheData, "last") != null)
                        {
                            ChangeCacheValue("last", (GetValueForKey(cacheData, "last") + filepath + "|:|"));
                        }
                        else
                        {
                            AddToChache($"last = {filepath}|:|");
                        }
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Ungültige Eingabe. Drücke 'R', 'S' oder 'N'.");
                    }
                } while (true);
                if (filepath == null)
                {
                    continue;
                }
                FileManager(filepath);
            }
        }

        static string DisplayRecentFiles()
        {
            Console.Clear();
            Console.WriteLine("Kürzlich verwendete Notizen:");

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
                    return recentFiles[selectedIndex];
                }
                else if (key == ConsoleKey.Escape)
                {
                    return null;
                }
            }
        }
    }
}