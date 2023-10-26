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

            string filepath;
            
            while (true)
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

                ConsoleKeyInfo keyInfo;

                do
                {
                    keyInfo = Console.ReadKey();
                    Console.WriteLine();

                    if (keyInfo.Key == ConsoleKey.R)
                    {
                        Console.Clear();
                        Console.WriteLine("Kürzlich verwendete Notizen werden geladen...");
                        Console.Clear();

                    }
                    else if (keyInfo.Key == ConsoleKey.N)
                    {
                        Console.WriteLine("********************************");
                        Console.WriteLine("*   Erstelle eine neue Notiz   *");
                        Console.WriteLine("********************************");
                        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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
                        Thread.Sleep(1000);
                        filepath = filePath;
                        break;
                    }
                    else if (keyInfo.Key == ConsoleKey.S)
                    {
                        filepath = FilePicker();
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
                        Console.WriteLine("Ungültige Eingabe. Drücke 'R' oder 'N'.");
                    }
                } while (true);

                FileManager(filepath);
            }
        }

        static void Manager(string path)
        {
            
        }
        static void FileRender(string content, int startrender, int x, int y)
        {

        }
    }
}