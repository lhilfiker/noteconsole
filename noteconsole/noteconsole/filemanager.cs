using System.Diagnostics;
using System.Text;
using TextCopy;

namespace noteconsole
{
    public class Formatted
    {
        public string? Text { get; set; }
        public ConsoleColor Color { get; set; }
        
        public ConsoleColor BackgroundColor { get; set; }
        public bool NewLine { get; set; }
    }

    internal partial class Program
    {
        private static List<string> _sidePanelContent = new()
        {
            "┌────────────────────────────────────┐",
            "|            noteconsole             |",
            "|────────────────────────────────────|",
            "| Hello!                             |",
            "|                                    |",
            "| Ctrl + S - Save                    |",
            "| Ctrl + P - Print                   |",
            "| Ctrl + E - Encrypt                 |",
            "| Esc - Escape                       |",
            "|────────────────────────────────────|",
            "| Edit Commands:                     |",
            "|   Alt + C - Copy                   |",
            "|   Alt + V - Paste                  |",
            "|   Alt + S - Selection Mode         |",
            "|                                    |",
            "|                                    |",
            "|                                    |",
            "|                                    |",
            "└────────────────────────────────────┘"
        };

        static bool _isSelection;
        static int _selectionStartX;
        static int _selectionStartY;
        static int _selectionEndX;
        static int _selectionEndY;
        public static string? Filecontent;

        public static void FileManager(string filePath)
        {
            Terminal.Clear();
            Console.WriteLine("Loading...");

            _isSelection = false;
            string? password = "";

            Filecontent = "";
            try
            {
                if (Path.GetExtension(filePath) == ".encrypted")
                {
                    byte[] decryptionBuffer = File.ReadAllBytes(filePath);
                    Terminal.Clear();
                    Console.Write("Please enter the password to decrypt the note: ");
                    password = Console.ReadLine();
                    Filecontent =
                        Encoding.UTF8.GetString(crypt.Decrypt(decryptionBuffer, password).GetAwaiter().GetResult());
                }
                else
                {
                    Filecontent = File.ReadAllText(filePath);
                }
            }
            catch
            {
                return;
            }

            int cursorX = 0;
            int cursorY = 0;
            bool isSidePanel = false;

            Console.CursorVisible = true;

            List<int> maxCharactersPerLine = new List<int>();
            // Precompute max characters for each line
            foreach (var line in Filecontent.Split('\n'))
            {
                maxCharactersPerLine.Add(line.Length + 1);
            }

            while (true)
            {
                int maxLine = maxCharactersPerLine.Count();
                int maxCharacter = maxCharactersPerLine[cursorY];
                int maxwidth = Console.WindowWidth;
                int maxheight = Console.WindowHeight;

                (List<Formatted> formatedtext, int startLine, int startChar) =
                    FileFormater(Filecontent, cursorX, cursorY, maxwidth, maxheight, isSidePanel);
                FileRender(formatedtext);

                int displayedCursorX = Math.Min(Math.Max(cursorX - startChar, 0), maxwidth - 1);
                int displayedCursorY = Math.Min(Math.Max(cursorY - startLine, 0), maxheight - 1);

                Console.SetCursorPosition(displayedCursorX, displayedCursorY);

                ConsoleKeyInfo pressedKey = Console.ReadKey();


                if (pressedKey.Modifiers.HasFlag(ConsoleModifiers.Alt))
                {
                    if (pressedKey.Key == ConsoleKey.V)
                    {
                        // Paste functionality
                        var clipboardText = ClipboardService.GetText();
                        if (!string.IsNullOrEmpty(clipboardText))
                        {
                            Terminal.Clear();
                            Console.WriteLine("Pasting...");
                            int pasteIndex = GetIndex(Filecontent, cursorX, cursorY);
                            Filecontent = Filecontent.Insert(pasteIndex, clipboardText);
                            cursorX += clipboardText.Length;
                            maxCharactersPerLine.Clear();
                            maxCharactersPerLine.Clear();
                            // Precompute max characters for each line
                            foreach (var line in Filecontent.Split('\n'))
                            {
                                maxCharactersPerLine.Add(line.Length + 1);
                            }
                        }
                    }

                    else if (pressedKey.Key == ConsoleKey.C)
                    {
                        if (cursorX > maxCharactersPerLine[cursorY])
                        {
                            cursorX = maxCharactersPerLine[cursorY];
                        }

                        ClipboardService.SetText(GetSelectedText(Filecontent));
                        _isSelection = false;
                    }

                    else if (!_isSelection)
                    {
                        _isSelection = true;
                        _selectionStartX = cursorX;
                        _selectionStartY = cursorY;
                        _selectionEndX = cursorX;
                        _selectionEndY = cursorY;
                    }
                    else
                    {
                        _isSelection = false;
                    }
                }
                else
                {
                    switch (pressedKey.Key)
                    {
                        case ConsoleKey.LeftArrow
                            when pressedKey.Modifiers.HasFlag(ConsoleModifiers.Control): // Move to start of line
                            if (!_isSelection) cursorX = 0;
                            else
                            {
                                cursorX = 0;
                                _selectionEndX = 0;
                            }

                            break;
                        case ConsoleKey.RightArrow
                            when pressedKey.Modifiers.HasFlag(ConsoleModifiers.Control): //Move to end of line
                            if (!_isSelection) cursorX = maxCharacter - 1;
                            else
                            {
                                cursorX = maxCharacter;
                                _selectionEndX = maxCharacter;
                            }

                            break;
                        case ConsoleKey.PageUp when pressedKey.Modifiers.HasFlag(ConsoleModifiers.Control):
                        case ConsoleKey.UpArrow
                            when pressedKey.Modifiers.HasFlag(ConsoleModifiers.Control): // Move to start of file
                            if (!_isSelection)
                            {
                                cursorY = 0;
                                cursorX = 0;
                            }
                            else
                            {
                                cursorY = 0;
                                cursorX = 0;
                                _selectionEndX = 0;
                                _selectionEndY = 0;
                            }

                            break;
                        case ConsoleKey.PageDown when pressedKey.Modifiers.HasFlag(ConsoleModifiers.Control):
                        case ConsoleKey.DownArrow
                            when pressedKey.Modifiers.HasFlag(ConsoleModifiers.Control): //Move to end of file
                            cursorY = maxLine - 1;
                            cursorX = maxCharacter;
                            if (_isSelection)
                            {
                                _selectionEndX = cursorX;
                                _selectionEndY = cursorY;
                            }

                            break;
                        case ConsoleKey.PageDown:
                        case ConsoleKey.DownArrow: //Movement
                            if (cursorY < maxLine - 1)
                            {
                                cursorY++;
                                cursorX = Math.Min(cursorX, maxCharactersPerLine[cursorY] - 1);
                                if (_isSelection)
                                {
                                    _selectionEndY = cursorY;
                                }
                            }
                            else if (cursorY == maxLine - 1 && formatedtext[cursorY - startLine].Text != "")
                            {
                                int enterIndex = GetIndex(Filecontent, maxCharacter, cursorY);
                                Filecontent = Filecontent.Insert(enterIndex, "\n");
                                cursorY++;
                                cursorX = 0;
                                maxCharactersPerLine.Insert(cursorY, GetMaxCharacter(Filecontent, cursorY));
                                maxCharactersPerLine[cursorY - 1] = GetMaxCharacter(Filecontent, cursorY - 1) + 1;
                            }

                            break;
                        case ConsoleKey.LeftArrow:
                            cursorX = Math.Max(0, cursorX - 1);
                            if (_isSelection)
                            {
                                _selectionEndX = cursorX;
                            }

                            break;
                        case ConsoleKey.RightArrow:
                            cursorX = Math.Min(cursorX + 1, maxCharacter - 1);
                            if (_isSelection)
                            {
                                _selectionEndX = cursorX;
                            }

                            break;
                        case ConsoleKey.PageUp:
                        case ConsoleKey.UpArrow:
                            if (cursorY > 0)
                            {
                                cursorY--;
                                cursorX = Math.Min(cursorX, maxCharactersPerLine[cursorY] - 1);
                                if (_isSelection)
                                {
                                    _selectionEndY = cursorY;
                                }
                            }

                            break;

                        case ConsoleKey.E when pressedKey.Modifiers.HasFlag(ConsoleModifiers.Control): //Encrypt
                            Terminal.Clear();
                            Console.WriteLine("Please enter a password to encrypt:");
                            try
                            {
                                password = Console.ReadLine();
                                byte[] toEncrypt = File.ReadAllBytes(filePath);
                                byte[] encryptedData = crypt.Encrypt(toEncrypt, password).GetAwaiter().GetResult();
                                string encryptedFilePath = filePath + ".encrypted";
                                Console.WriteLine("Encrypting file...");
                                File.WriteAllBytes(encryptedFilePath, encryptedData);
                                File.Delete(filePath);
                                // Change Last accessed item:
                                List<string> lastAccessed;
                                lastAccessed = GetValueForKey(cacheData, "last").Split("|:|").ToList();
                                lastAccessed[0] = encryptedFilePath;
                                string updatedLastAccessed = "";
                                foreach (string recentPaths in lastAccessed)
                                {
                                    updatedLastAccessed += recentPaths + "|:|";
                                }

                                ChangeCacheValue("last", updatedLastAccessed);
                                Console.WriteLine("Done. Press Any Key to exit.");
                                Console.ReadKey();
                            }
                            catch
                            {
                                Console.WriteLine("Error. Press Any Key to exit.");
                                Console.ReadKey();
                            }

                            break;
                        case ConsoleKey.S when pressedKey.Modifiers.HasFlag(ConsoleModifiers.Control): // Save
                            if (Path.GetExtension(filePath) == ".encrypted")
                            {
                                byte[] encryptionBuffer = Encoding.UTF8.GetBytes(Filecontent);
                                crypt.Encrypt(encryptionBuffer, password).GetAwaiter().GetResult();
                            }
                            else
                            {
                                File.WriteAllText(filePath, Filecontent);
                            }

                            break;
                        case ConsoleKey.Escape: // Go to welcome screen
                            Terminal.Clear();
                            if (Filecontent != File.ReadAllText(filePath))
                            {
                                Console.WriteLine("Save changes? (Y/N)");
                                ConsoleKeyInfo saveKey = Console.ReadKey();
                                if (saveKey.Key == ConsoleKey.Y)
                                {
                                    if (Path.GetExtension(filePath) == ".encrypted")
                                    {
                                        byte[] encryptionBuffer = Encoding.UTF8.GetBytes(Filecontent);
                                        crypt.Encrypt(encryptionBuffer, password).GetAwaiter().GetResult();
                                    }
                                    else
                                    {
                                        File.WriteAllText(filePath, Filecontent);
                                    }
                                }
                            }

                            return;
                        case ConsoleKey.Backspace: //Delete
                        case ConsoleKey.Delete:
                            if (_isSelection) // Check if selection is true
                            {
                                int startIndex = GetIndex(Filecontent, _selectionStartX, _selectionStartY);
                                int endIndex = GetIndex(Filecontent, _selectionEndX, _selectionEndY);

                                if (startIndex > endIndex) // If selecting behind the start change values
                                {
                                    (endIndex, startIndex) = (startIndex, endIndex);
                                    _selectionStartX = _selectionEndX;
                                    _selectionStartY = _selectionEndY;
                                }

                                Filecontent = Filecontent.Remove(startIndex, endIndex - startIndex);

                                cursorX = _selectionStartX;
                                cursorY = _selectionStartY;

                                // Precompute max characters for each line
                                maxCharactersPerLine.Clear();
                                foreach (var lineincontent in Filecontent.Split('\n'))
                                {
                                    maxCharactersPerLine.Add(lineincontent.Length + 1);
                                }

                                _isSelection = false;
                            }
                            else
                            {
                                // Existing logic
                                if (cursorX != 0)
                                {
                                    int deleteIndex = GetIndex(Filecontent, cursorX, cursorY);
                                    Filecontent = Filecontent.Remove(deleteIndex - 1, 1);
                                    cursorX--;
                                }
                                // Append the current line to the line above so there is only one line.
                                else if (cursorY != 0)
                                {
                                    int deleteIndex = GetIndex(Filecontent, cursorX, cursorY);
                                    string line2 = Filecontent.Substring(deleteIndex);
                                    Filecontent = Filecontent.Remove(deleteIndex);
                                    Filecontent = Filecontent.Insert(deleteIndex - 1, line2);
                                    cursorY--;
                                    cursorX = maxCharactersPerLine[cursorY];
                                    maxCharactersPerLine.RemoveAt(cursorY + 1);
                                    maxCharactersPerLine[cursorY] = GetMaxCharacter(Filecontent, cursorY) + 1;
                                }
                            }

                            break;

                        case ConsoleKey.Enter: // NewLine
                            if (_isSelection) // Check if selection is true
                            {
                                int startIndex = GetIndex(Filecontent, _selectionStartX, _selectionStartY);
                                int endIndex = GetIndex(Filecontent, _selectionEndX, _selectionEndY);

                                if (startIndex > endIndex) // If selecting behind the start change values
                                {
                                    (endIndex, startIndex) = (startIndex, endIndex);
                                    _selectionStartX = _selectionEndX;
                                    _selectionStartY = _selectionEndY;
                                }

                                Filecontent = Filecontent.Remove(startIndex, endIndex - startIndex);

                                cursorX = _selectionStartX;
                                cursorY = _selectionStartY;

                                // Precompute max characters for each line
                                maxCharactersPerLine.Clear();
                                foreach (var lineincontent in Filecontent.Split('\n'))
                                {
                                    maxCharactersPerLine.Add(lineincontent.Length + 1);
                                }

                                _isSelection = false;
                            }
                            else
                            {
                                int enterIndex = GetIndex(Filecontent, cursorX, cursorY);
                                Filecontent = Filecontent.Insert(enterIndex, "\n");
                                cursorY++;
                                cursorX = 0;
                                maxCharactersPerLine.Insert(cursorY, GetMaxCharacter(Filecontent, cursorY));
                                maxCharactersPerLine[cursorY - 1] = GetMaxCharacter(Filecontent, cursorY - 1) + 1;
                            }

                            break;

                        case ConsoleKey.Spacebar: // Space
                            if (_isSelection) // Check if selection is true
                            {
                                int startIndex = GetIndex(Filecontent, _selectionStartX, _selectionStartY);
                                int endIndex = GetIndex(Filecontent, _selectionEndX, _selectionEndY);

                                if (startIndex > endIndex) // If selecting behind the start change values
                                {
                                    (endIndex, startIndex) = (startIndex, endIndex);
                                    _selectionStartX = _selectionEndX;
                                    _selectionStartY = _selectionEndY;
                                }

                                Filecontent = Filecontent.Remove(startIndex, endIndex - startIndex);

                                cursorX = _selectionStartX;
                                cursorY = _selectionStartY;

                                // Precompute max characters for each line
                                maxCharactersPerLine.Clear();
                                foreach (var lineincontent in Filecontent.Split('\n'))
                                {
                                    maxCharactersPerLine.Add(lineincontent.Length + 1);
                                }

                                _isSelection = false;
                            }
                            else
                            {
                                int spaceIndex = GetIndex(Filecontent, cursorX, cursorY);
                                Filecontent = Filecontent.Insert(spaceIndex, " ");
                                maxCharactersPerLine[cursorY] = GetMaxCharacter(Filecontent, cursorY) + 1;
                                cursorX++;
                            }

                            break;
                        case ConsoleKey.Tab:
                            if (_isSelection) // Check if selection is true
                            {
                                int startIndex = GetIndex(Filecontent, _selectionStartX, _selectionStartY);
                                int endIndex = GetIndex(Filecontent, _selectionEndX, _selectionEndY);

                                if (startIndex > endIndex) // If selecting behind the start change values
                                {
                                    (endIndex, startIndex) = (startIndex, endIndex);
                                    _selectionStartX = _selectionEndX;
                                    _selectionStartY = _selectionEndY;
                                }

                                Filecontent = Filecontent.Remove(startIndex, endIndex - startIndex);

                                cursorX = _selectionStartX;
                                cursorY = _selectionStartY;

                                // Precompute max characters for each line
                                maxCharactersPerLine.Clear();
                                foreach (var lineincontent in Filecontent.Split('\n'))
                                {
                                    maxCharactersPerLine.Add(lineincontent.Length + 1);
                                }

                                _isSelection = false;
                            }
                            else
                            {
                                int spaceIndex = GetIndex(Filecontent, cursorX, cursorY);
                                Filecontent = Filecontent.Insert(spaceIndex, "    ");
                                maxCharactersPerLine[cursorY] = GetMaxCharacter(Filecontent, cursorY);
                                cursorX = cursorX + 4;
                            }

                            break;
                        case ConsoleKey.Home:
                        case ConsoleKey.End:
                        case ConsoleKey.Insert:
                        case ConsoleKey.F1:
                        case ConsoleKey.F2:
                        case ConsoleKey.F3:
                        case ConsoleKey.F4:
                        case ConsoleKey.F5:
                        case ConsoleKey.F6:
                        case ConsoleKey.F7:
                        case ConsoleKey.F8:
                        case ConsoleKey.F9:
                        case ConsoleKey.F10:
                        case ConsoleKey.F11:
                        case ConsoleKey.F12:
                        case ConsoleKey.Pause:
                            // Do nothing for these special keys to prevent unwanted behavior
                            break;

                        case ConsoleKey.P when pressedKey.Modifiers.HasFlag(ConsoleModifiers.Control):
                        case ConsoleKey.PrintScreen:
                            // Here comes the print logic
                            File.WriteAllText(path, Filecontent);

                            // Use a system command to print the file
                            ProcessStartInfo psi = new ProcessStartInfo()
                            {
                                FileName = "print",
                                Verb = "PrintTo",
                                Arguments = path,
                                CreateNoWindow = true,
                                UseShellExecute = true,
                            };

                            Process.Start(psi);
                            Console.WriteLine("File has been sent to the printer.");
                            break;

                        case ConsoleKey.N when pressedKey.Modifiers.HasFlag(ConsoleModifiers.Control):
                            isSidePanel = !isSidePanel;
                            break;
                        default: // Default case is a character key
                            char keyChar = pressedKey.KeyChar;

                            if (_isSelection) // Check if selection is true
                            {
                                int startIndex = GetIndex(Filecontent, _selectionStartX, _selectionStartY);
                                int endIndex = GetIndex(Filecontent, _selectionEndX, _selectionEndY);

                                if (startIndex > endIndex) // If selecting behind the start change values
                                {
                                    (endIndex, startIndex) = (startIndex, endIndex);
                                    _selectionStartX = _selectionEndX;
                                    _selectionStartY = _selectionEndY;
                                }

                                Filecontent = Filecontent.Remove(startIndex, endIndex - startIndex);

                                Filecontent = Filecontent.Insert(startIndex, keyChar.ToString());

                                cursorX = _selectionStartX + 1;
                                cursorY = _selectionStartY;

                                // Precompute max characters for each line
                                maxCharactersPerLine.Clear();
                                foreach (var lineincontent in Filecontent.Split('\n'))
                                {
                                    maxCharactersPerLine.Add(lineincontent.Length + 1);
                                }

                                _isSelection = false;
                            }
                            else
                            {
                                int charIndex = GetIndex(Filecontent, cursorX, cursorY);
                                Filecontent = Filecontent.Insert(charIndex, keyChar.ToString());
                                cursorX++;
                                maxCharactersPerLine[cursorY] = GetMaxCharacter(Filecontent, cursorY);
                            }

                            break;
                    }

                    //Make sure cursor is within boundaries
                    if (cursorX > maxCharactersPerLine[cursorY])
                    {
                        cursorX = maxCharactersPerLine[cursorY];
                    }
                }
            }
        }

        static string GetSelectedText(string? filecontent)
        {
            // Determine the "true" start and end of the selection
            int trueStartX = Math.Min(_selectionStartX, _selectionEndX);
            int trueEndX = Math.Max(_selectionStartX, _selectionEndX);
            int trueStartY = Math.Min(_selectionStartY, _selectionEndY);
            int trueEndY = Math.Max(_selectionStartY, _selectionEndY);

            StringBuilder selectedText = new StringBuilder();

            string[]? lines = filecontent?.Split('\n');

            for (int i = trueStartY; i <= trueEndY; i++)
            {
                if (lines != null && i >= lines.Length)
                    break;

                // If it's the start and end line of the selection
                if (i == trueStartY && i == trueEndY)
                {
                    if (lines != null && lines[i].Length >= trueEndX + 1)
                    {
                        selectedText.Append(lines[i].Substring(trueStartX, trueEndX - trueStartX + 1));
                    }
                    else if (lines != null && lines[i].Length > trueStartX)
                    {
                        selectedText.Append(lines[i].Substring(trueStartX));
                    }
                }
                // If it's the start line of the selection
                else if (i == trueStartY)
                {
                    if (lines != null && lines[i].Length > trueStartX)
                    {
                        selectedText.Append(lines[i].Substring(trueStartX));
                    }
                    else
                    {
                        selectedText.Append(lines?[i]);
                    }
                }
                // If it's the end line of the selection
                else if (i == trueEndY)
                {
                    if (lines != null && lines[i].Length >= trueEndX + 1)
                    {
                        selectedText.Append(lines[i].Substring(0, trueEndX + 1));
                    }
                    else
                    {
                        selectedText.Append(lines?[i]);
                    }
                }
                // Any line in between
                else
                {
                    selectedText.Append(lines?[i]);
                }

                // Add a newline character for every line except the last one
                if (i != trueEndY)
                {
                    selectedText.Append('\n');
                }
            }

            Terminal.Clear();
            Console.WriteLine($"{selectedText} will be copied. \nPress Enter to continue");
            Console.ReadLine();

            return selectedText.ToString();
        }


        static int GetIndex(string? filecontent, int cursorx, int cursory)
        {
            string[]? lines = filecontent?.Split('\n');

            // Ensure cursory is within the number of lines
            if (lines != null && cursory >= lines.Length)
            {
                cursory = lines.Length - 1;
            }

            // Ensure cursorx is within the length of the line at cursory
            if (lines != null && cursory >= 0 && cursorx > lines[cursory].Length)
            {
                cursorx = lines[cursory].Length;
            }

            int index = 0;
            for (int i = 0; i < cursory; i++)
            {
                if (lines != null) index += lines[i].Length + 1; // +1 for the '\n' character
            }

            index += cursorx;

            return index;
        }

        static (List<Formatted>, int, int) FileFormater(string? filecontent, int x, int y, int maxwidth, int maxheight,
            bool isSidePanel)
        {
            List<string>? lines = filecontent?.Split('\n').ToList();
            List<Formatted> formattedWithColor = new();

            // Ensure y is within boundaries
            y = Math.Min(y, lines.Count - 1);

            // Calculate start line for vertical scrolling
            int midPoint = maxheight / 2;
            int startLine;
            if (y < midPoint)
            {
                startLine = 0;
            }
            else if (y >= lines.Count - midPoint)
            {
                startLine = Math.Max(0, lines.Count - maxheight + 1);
            }
            else
            {
                startLine = y - midPoint;
            }

            int endLine = Math.Min(lines.Count, startLine + maxheight - 1);
            int startChar = 0;
            List<ColorsGlobal> globalColorListBuffer = GlobalColorList.ToList();
            List<Formatted> formattedLine = new();

            for (int i = startLine; i < endLine; i++)
            {
                string line = lines[i];
                // Start line for horizontal scrolling
                if (line.Length > maxwidth - 1)
                {
                    startChar = i == y ? Math.Max(0, Math.Min(x - (maxwidth), line.Length - maxwidth) + 1) : 0;
                }
                else
                {
                    startChar = 0;
                }

                formattedLine = ProcessLineForColor(line, startChar, i, maxwidth, globalColorListBuffer);
                formattedWithColor.AddRange(formattedLine);
            }

            while (formattedWithColor.Count < maxheight - 2)
            {
                formattedWithColor.Add(new Formatted { Text = "", Color = ConsoleColor.White, NewLine = true });
            }

            // If Sidepanl is activated replace right side with the panel
            if (isSidePanel)
            {
                for (int i = 0; i < formattedWithColor.Count; i++)
                {
                    if (i >= _sidePanelContent.Count) // Check if were beyond the length of sidepanel
                    {
                        break;
                    }

                    var substring = formattedWithColor[i].Text;
                    if (substring != null)
                    {
                        int totalContentLength = substring.Length + _sidePanelContent[i].Length;
                        int paddingLength = maxwidth - totalContentLength; // Calculate the number of spaces required

                        if (paddingLength < 0) // If content is longer than maxwidth
                        {
                            formattedWithColor[i].Text = substring.Substring(0, maxwidth - _sidePanelContent[i].Length);
                            formattedWithColor[i].Color = ConsoleColor.White;
                            paddingLength = 0;
                        }

                        formattedWithColor[i].Text = substring + new string(' ', paddingLength) + _sidePanelContent[i];
                    }

                    formattedWithColor[i].Color = ConsoleColor.White;
                }
            }

            return (formattedWithColor, startLine, startChar);
        }


        static void FileRender(List<Formatted> text)
        {
            Terminal.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            for (int i = 0; i < text.Count; i++)
            {
                Console.ForegroundColor = text[i].Color;
                if (text[i].BackgroundColor != null) Console.BackgroundColor = text[i].BackgroundColor;
                if (text[i].NewLine) Console.WriteLine(text[i].Text);
                else Console.Write(text[i].Text);
            }

            if (_isSelection)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.Write("[Selection Mode]");
                Console.ResetColor();
            }
        }


        static int GetMaxCharacter(string? filecontent, int line)
        {
            try
            {
                string[]? lines = filecontent?.Split('\n');
                int numberOfCharacters = lines[line].Count();
                return numberOfCharacters + 1;
            }
            catch
            {
                return 0;
            }
        }

        private static List<Formatted> ProcessLineForColor(string line, int startChar, int i, int maxwidth, List<ColorsGlobal> globalColorListBuffer)
        {
            List<Formatted> formattedLine = new List<Formatted>();
            List<ColorsGlobal> colorsForThisLine = new();

            if (line == "")
            {
                formattedLine.Add(new Formatted { Text = line, Color = ConsoleColor.White, BackgroundColor = ConsoleColor.Black, NewLine = true });
                return formattedLine;
            }

            colorsForThisLine.AddRange(globalColorListBuffer.Where(obj => obj.line == i));

            string lineWithoutColor = line.Substring(startChar, Math.Min(maxwidth, line.Length - startChar));

            for (int j = 0; j < lineWithoutColor.Length;)
            {
                int originalIndex = j + startChar; // Index in the original line
                var colorItem = colorsForThisLine.FirstOrDefault(item =>
                    originalIndex >= item.StartChar && originalIndex < item.EndChar);

                int segmentLength;
                if (colorItem != null)
                {
                    // Calculate the length of the colored segment
                    int segmentEnd = Math.Min(colorItem.EndChar - startChar, lineWithoutColor.Length);
                    segmentLength = segmentEnd - j;
                    formattedLine.Add(new Formatted
                    {
                        Text = lineWithoutColor.Substring(j, segmentLength), Color = colorItem.Color, BackgroundColor = colorItem.BackgroundColor == null ? ConsoleColor.Black : colorItem.BackgroundColor, NewLine = false
                    });
                }
                else
                {
                    // Find the next color change or the end of the line
                    int nextColorStart = colorsForThisLine.Where(item => item.StartChar > originalIndex)
                        .Select(item => item.StartChar - startChar)
                        .DefaultIfEmpty(lineWithoutColor.Length)
                        .Min();
                    segmentLength = nextColorStart - j;
                    formattedLine.Add(new Formatted
                    {
                        Text = lineWithoutColor.Substring(j, segmentLength), Color = ConsoleColor.White, NewLine = false
                    });
                }

                j += segmentLength;
            }

            if (formattedLine.Any())
            {
                formattedLine.Last().NewLine = true;
            }

            return formattedLine;
        }
    }
}