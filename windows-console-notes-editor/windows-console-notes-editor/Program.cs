namespace windows_console_notes_editor
{
    internal partial class Program
    {
        static void Main(string[] args)
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
                }
                else if (keyInfo.Key == ConsoleKey.N)
                {
                    Console.Clear();
                    Console.WriteLine("Neue Notiz wird erstellt...");
                }
                else if (keyInfo.Key == ConsoleKey.S)
                {
                    FilePicker();
                }
                else
                {
                    Console.WriteLine("Ungültige Eingabe. Drücke 'R' oder 'N'.");
                }
            } while (keyInfo.Key != ConsoleKey.R && keyInfo.Key != ConsoleKey.N);
        }

        static void Manager(string path)
        {

        }
        static void FileRender(string content, int startrender, int x, int y)
        {

        }
    }
}