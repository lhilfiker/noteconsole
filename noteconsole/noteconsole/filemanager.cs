﻿using System.Diagnostics;
using System.Text;
using TextCopy;

namespace noteconsole
{
    public class formated
    {
        public string Text { get; set; }
        public ConsoleColor Color { get; set; }
        public bool NewLine { get; set; }
    }

    internal partial class Program
    {
        static List<string> sidePanelContent = new()
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

        static bool isSelection = false;
        static int selectionStartX;
        static int selectionStartY;
        static int selectionEndX;
        static int selectionEndY;
        public static string filecontent;

        public static void FileManager(string filepath)
        {
            Terminal.Clear();
            Console.WriteLine("Loading...");

            string password = "";

            filecontent = "";
            try
            {
                if (Path.GetExtension(filepath) == ".encrypted")
                {
                    byte[] decryptionBuffer = File.ReadAllBytes(filepath);
                    Terminal.Clear();
                    Console.Write("Please enter the password to decrypt the note: ");
                    password = Console.ReadLine();
                    filecontent =
                        Encoding.UTF8.GetString(crypt.Decrypt(decryptionBuffer, password).GetAwaiter().GetResult());
                }
                else
                {
                    filecontent = File.ReadAllText(filepath);
                }
            }
            catch
            {
                return;
            }

            int cursorx = 0;
            int cursory = 0;
            bool isSidePanel = false;

            Console.CursorVisible = true;

            List<int> maxCharactersPerLine = new List<int>();
            // Precompute max characters for each line
            foreach (var line in filecontent.Split('\n'))
            {
                maxCharactersPerLine.Add(line.Length + 1);
            }

            while (true)
            {
                int maxline = maxCharactersPerLine.Count();
                int maxcharacter = maxCharactersPerLine[cursory];
                int maxwidth = Console.WindowWidth;
                int maxheight = Console.WindowHeight;

                (List<formated> formatedtext, int startLine, int startChar) =
                    FileFormater(filecontent, cursorx, cursory, maxwidth, maxheight, isSidePanel);
                FileRender(formatedtext);

                int displayedCursorX = Math.Min(Math.Max(cursorx - startChar, 0), maxwidth - 1);
                int displayedCursorY = Math.Min(Math.Max(cursory - startLine, 0), maxheight - 1);

                Console.SetCursorPosition(displayedCursorX, displayedCursorY);

                ConsoleKeyInfo pressedKey = Console.ReadKey();


                if (pressedKey.Modifiers.HasFlag(ConsoleModifiers.Alt))
                {
                    if (pressedKey.Key == ConsoleKey.V)
                    {
                        // Paste functionality
                        var clipboardText = TextCopy.ClipboardService.GetText();
                        if (!string.IsNullOrEmpty(clipboardText))
                        {
                            Terminal.Clear();
                            Console.WriteLine("Pasting...");
                            int pasteIndex = GetIndex(filecontent, cursorx, cursory);
                            filecontent = filecontent.Insert(pasteIndex, clipboardText);
                            cursorx += clipboardText.Length;
                            maxCharactersPerLine.Clear();
                            maxCharactersPerLine.Clear();
                            // Precompute max characters for each line
                            foreach (var line in filecontent.Split('\n'))
                            {
                                maxCharactersPerLine.Add(line.Length + 1);
                            }
                        }
                    }

                    else if (pressedKey.Key == ConsoleKey.C)
                    {
                        if (cursorx > maxCharactersPerLine[cursory])
                        {
                            cursorx = maxCharactersPerLine[cursory];
                        }

                        ClipboardService.SetText(GetSelectedText(filecontent));
                        isSelection = false;
                    }

                    else if (!isSelection)
                    {
                        isSelection = true;
                        selectionStartX = cursorx;
                        selectionStartY = cursory;
                        selectionEndX = cursorx;
                        selectionEndY = cursory;
                    }
                    else
                    {
                        isSelection = false;
                    }
                }
                else
                {
                    switch (pressedKey.Key)
                    {
                        case ConsoleKey.LeftArrow
                            when pressedKey.Modifiers.HasFlag(ConsoleModifiers.Control): // Move to start of line
                            if (!isSelection) cursorx = 0;
                            else
                            {
                                cursorx = 0;
                                selectionEndX = 0;
                            }

                            break;
                        case ConsoleKey.RightArrow
                            when pressedKey.Modifiers.HasFlag(ConsoleModifiers.Control): //Move to end of line
                            if (!isSelection) cursorx = maxcharacter;
                            else
                            {
                                cursorx = maxcharacter;
                                selectionEndX = maxcharacter;
                            }

                            break;
                        case ConsoleKey.PageUp when pressedKey.Modifiers.HasFlag(ConsoleModifiers.Control):
                        case ConsoleKey.UpArrow
                            when pressedKey.Modifiers.HasFlag(ConsoleModifiers.Control): // Move to start of file
                            if (!isSelection)
                            {
                                cursory = 0;
                                cursorx = 0;
                            }
                            else
                            {
                                cursory = 0;
                                cursorx = 0;
                                selectionEndX = 0;
                                selectionEndY = 0;
                            }

                            break;
                        case ConsoleKey.PageDown when pressedKey.Modifiers.HasFlag(ConsoleModifiers.Control):
                        case ConsoleKey.DownArrow
                            when pressedKey.Modifiers.HasFlag(ConsoleModifiers.Control): //Move to end of file
                            cursory = maxline - 1;
                            cursorx = maxcharacter;
                            if (isSelection)
                            {
                                selectionEndX = cursorx;
                                selectionEndY = cursory;
                            }

                            break;
                        case ConsoleKey.PageDown:
                        case ConsoleKey.DownArrow: //Movement
                            if (cursory < maxline - 1)
                            {
                                cursory++;
                                cursorx = Math.Min(cursorx, maxcharacter);
                                if (isSelection)
                                {
                                    selectionEndY = cursory;
                                }
                            }
                            else if (cursory == maxline - 1 && formatedtext[cursory - startLine].Text != "")
                            {
                                int enterIndex = GetIndex(filecontent, maxcharacter, cursory);
                                string line = filecontent.Substring(enterIndex);
                                filecontent = filecontent.Insert(enterIndex, "\n");
                                cursory++;
                                cursorx = 0;
                                maxCharactersPerLine.Insert(cursory, GetMaxCharacter(filecontent, cursory));
                                maxCharactersPerLine[cursory - 1] = GetMaxCharacter(filecontent, cursory - 1);
                            }

                            break;
                        case ConsoleKey.LeftArrow:
                            cursorx = Math.Max(0, cursorx - 1);
                            if (isSelection)
                            {
                                selectionEndX = cursorx;
                            }

                            break;
                        case ConsoleKey.RightArrow:
                            cursorx = Math.Min(cursorx + 1, maxcharacter - 1);
                            if (isSelection)
                            {
                                selectionEndX = cursorx;
                            }

                            break;
                        case ConsoleKey.PageUp:
                        case ConsoleKey.UpArrow:
                            if (cursory > 0)
                            {
                                cursory--;
                                cursorx = Math.Min(cursorx, maxcharacter);
                                if (isSelection)
                                {
                                    selectionEndY = cursory;
                                }
                            }

                            break;

                        case ConsoleKey.E when pressedKey.Modifiers.HasFlag(ConsoleModifiers.Control): //Encrypt
                            Terminal.Clear();
                            Console.WriteLine("Please enter a password to encrypt:");
                            try
                            {
                                password = Console.ReadLine();
                                byte[] toencrypt = File.ReadAllBytes(filepath);
                                byte[] encrypteddata = crypt.Encrypt(toencrypt, password).GetAwaiter().GetResult();
                                string encryptedFilePath = filepath + ".encrypted";
                                Console.WriteLine("Encrypting file...");
                                File.WriteAllBytes(encryptedFilePath, encrypteddata);
                                File.Delete(filepath);
                                // Change Last accessed item:
                                List<string> lastaccessed = new();
                                lastaccessed = GetValueForKey(cacheData, "last").Split("|:|").ToList();
                                lastaccessed[0] = encryptedFilePath;
                                string updatedLastAccessed = "";
                                foreach (string recentpaths in lastaccessed)
                                {
                                    updatedLastAccessed += recentpaths + "|:|";
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
                            if (Path.GetExtension(filepath) == ".encrypted")
                            {
                                byte[] encryptionBuffer = Encoding.UTF8.GetBytes(filecontent);
                                byte[] encryptedData =
                                    crypt.Encrypt(encryptionBuffer, password).GetAwaiter().GetResult();
                            }
                            else
                            {
                                File.WriteAllText(filepath, filecontent);
                            }

                            break;
                        case ConsoleKey.Escape: // Go to welcome screen
                            Terminal.Clear();
                            if (filecontent != File.ReadAllText(filepath))
                            {
                                Console.WriteLine("Save changes? (Y/N)");
                                ConsoleKeyInfo saveKey = Console.ReadKey();
                                if (saveKey.Key == ConsoleKey.Y)
                                {
                                    if (Path.GetExtension(filepath) == ".encrypted")
                                    {
                                        byte[] encryptionBuffer = Encoding.UTF8.GetBytes(filecontent);
                                        byte[] encryptedData =
                                            crypt.Encrypt(encryptionBuffer, password).GetAwaiter().GetResult();
                                    }
                                    else
                                    {
                                        File.WriteAllText(filepath, filecontent);
                                    }
                                }
                            }

                            return;
                        case ConsoleKey.Backspace: //Delete
                        case ConsoleKey.Delete:
                            if (isSelection) // Check if selection is true
                            {
                                int startIndex = GetIndex(filecontent, selectionStartX, selectionStartY);
                                int endIndex = GetIndex(filecontent, selectionEndX, selectionEndY);

                                if (startIndex > endIndex) // If selecting behind the start change values
                                {
                                    int buffer = endIndex;
                                    endIndex = startIndex;
                                    startIndex = buffer;
                                    selectionStartX = selectionEndX;
                                    selectionStartY = selectionEndY;
                                }

                                filecontent = filecontent.Remove(startIndex, endIndex - startIndex);

                                cursorx = selectionStartX;
                                cursory = selectionStartY;

                                // Precompute max characters for each line
                                maxCharactersPerLine.Clear();
                                foreach (var lineincontent in filecontent.Split('\n'))
                                {
                                    maxCharactersPerLine.Add(lineincontent.Length + 1);
                                }

                                isSelection = false;
                            }
                            else
                            {
                                // Existing logic
                                if (cursorx != 0)
                                {
                                    int deleteIndex = GetIndex(filecontent, cursorx, cursory);
                                    filecontent = filecontent.Remove(deleteIndex - 1, 1);
                                    cursorx--;
                                }
                                // Append the current line to the line above so there is only one line.
                                else if (cursory != 0)
                                {
                                    int deleteIndex = GetIndex(filecontent, cursorx, cursory);
                                    string line2 = filecontent.Substring(deleteIndex);
                                    filecontent = filecontent.Remove(deleteIndex);
                                    filecontent = filecontent.Insert(deleteIndex - 1, line2);
                                    cursory--;
                                    cursorx = maxCharactersPerLine[cursory];
                                    maxCharactersPerLine.RemoveAt(cursory + 1);
                                    maxCharactersPerLine[cursory] = GetMaxCharacter(filecontent, cursory);
                                }
                            }

                            break;

                        case ConsoleKey.Enter: // NewLine
                            if (isSelection) // Check if selection is true
                            {
                                int startIndex = GetIndex(filecontent, selectionStartX, selectionStartY);
                                int endIndex = GetIndex(filecontent, selectionEndX, selectionEndY);

                                if (startIndex > endIndex) // If selecting behind the start change values
                                {
                                    int buffer = endIndex;
                                    endIndex = startIndex;
                                    startIndex = buffer;
                                    selectionStartX = selectionEndX;
                                    selectionStartY = selectionEndY;
                                }

                                filecontent = filecontent.Remove(startIndex, endIndex - startIndex);

                                cursorx = selectionStartX;
                                cursory = selectionStartY;

                                // Precompute max characters for each line
                                maxCharactersPerLine.Clear();
                                foreach (var lineincontent in filecontent.Split('\n'))
                                {
                                    maxCharactersPerLine.Add(lineincontent.Length + 1);
                                }

                                isSelection = false;
                            }
                            else
                            {
                                int enterIndex = GetIndex(filecontent, cursorx, cursory);
                                string line = filecontent.Substring(enterIndex);
                                filecontent = filecontent.Insert(enterIndex, "\n");
                                cursory++;
                                cursorx = 0;
                                maxCharactersPerLine.Insert(cursory, GetMaxCharacter(filecontent, cursory));
                                maxCharactersPerLine[cursory - 1] = GetMaxCharacter(filecontent, cursory - 1);
                            }

                            break;

                        case ConsoleKey.Spacebar: // Space
                            if (isSelection) // Check if selection is true
                            {
                                int startIndex = GetIndex(filecontent, selectionStartX, selectionStartY);
                                int endIndex = GetIndex(filecontent, selectionEndX, selectionEndY);

                                if (startIndex > endIndex) // If selecting behind the start change values
                                {
                                    int buffer = endIndex;
                                    endIndex = startIndex;
                                    startIndex = buffer;
                                    selectionStartX = selectionEndX;
                                    selectionStartY = selectionEndY;
                                }

                                filecontent = filecontent.Remove(startIndex, endIndex - startIndex);

                                cursorx = selectionStartX;
                                cursory = selectionStartY;

                                // Precompute max characters for each line
                                maxCharactersPerLine.Clear();
                                foreach (var lineincontent in filecontent.Split('\n'))
                                {
                                    maxCharactersPerLine.Add(lineincontent.Length + 1);
                                }

                                isSelection = false;
                            }
                            else
                            {
                                int spaceIndex = GetIndex(filecontent, cursorx, cursory);
                                filecontent = filecontent.Insert(spaceIndex, " ");
                                maxCharactersPerLine[cursory] = GetMaxCharacter(filecontent, cursory);
                                cursorx++;
                            }

                            break;
                        case ConsoleKey.Tab:
                            if (isSelection) // Check if selection is true
                            {
                                int startIndex = GetIndex(filecontent, selectionStartX, selectionStartY);
                                int endIndex = GetIndex(filecontent, selectionEndX, selectionEndY);

                                if (startIndex > endIndex) // If selecting behind the start change values
                                {
                                    int buffer = endIndex;
                                    endIndex = startIndex;
                                    startIndex = buffer;
                                    selectionStartX = selectionEndX;
                                    selectionStartY = selectionEndY;
                                }

                                filecontent = filecontent.Remove(startIndex, endIndex - startIndex);

                                cursorx = selectionStartX;
                                cursory = selectionStartY;

                                // Precompute max characters for each line
                                maxCharactersPerLine.Clear();
                                foreach (var lineincontent in filecontent.Split('\n'))
                                {
                                    maxCharactersPerLine.Add(lineincontent.Length + 1);
                                }

                                isSelection = false;
                            }
                            else
                            {
                                int spaceIndex = GetIndex(filecontent, cursorx, cursory);
                                filecontent = filecontent.Insert(spaceIndex, "    ");
                                maxCharactersPerLine[cursory] = GetMaxCharacter(filecontent, cursory);
                                cursorx = cursorx + 4;
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
                            string path = "print_file.txt";
                            File.WriteAllText(path, filecontent);

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

                            if (isSelection) // Check if selection is true
                            {
                                int startIndex = GetIndex(filecontent, selectionStartX, selectionStartY);
                                int endIndex = GetIndex(filecontent, selectionEndX, selectionEndY);

                                if (startIndex > endIndex) // If selecting behind the start change values
                                {
                                    int buffer = endIndex;
                                    endIndex = startIndex;
                                    startIndex = buffer;
                                    selectionStartX = selectionEndX;
                                    selectionStartY = selectionEndY;
                                }

                                filecontent = filecontent.Remove(startIndex, endIndex - startIndex);

                                filecontent = filecontent.Insert(startIndex, keyChar.ToString());

                                cursorx = selectionStartX + 1;
                                cursory = selectionStartY;

                                // Precompute max characters for each line
                                maxCharactersPerLine.Clear();
                                foreach (var lineincontent in filecontent.Split('\n'))
                                {
                                    maxCharactersPerLine.Add(lineincontent.Length + 1);
                                }

                                isSelection = false;
                            }
                            else
                            {
                                int charIndex = GetIndex(filecontent, cursorx, cursory);
                                filecontent = filecontent.Insert(charIndex, keyChar.ToString());
                                cursorx++;
                                maxCharactersPerLine[cursory] = GetMaxCharacter(filecontent, cursory);
                            }

                            break;
                    }

                    //Make sure cursor is within boundaries
                    if (cursorx > maxCharactersPerLine[cursory])
                    {
                        cursorx = maxCharactersPerLine[cursory];
                    }
                }
            }
        }

        static string GetSelectedText(string filecontent)
        {
            // Determine the "true" start and end of the selection
            int trueStartX = Math.Min(selectionStartX, selectionEndX);
            int trueEndX = Math.Max(selectionStartX, selectionEndX);
            int trueStartY = Math.Min(selectionStartY, selectionEndY);
            int trueEndY = Math.Max(selectionStartY, selectionEndY);

            StringBuilder selectedText = new StringBuilder();

            string[] lines = filecontent.Split('\n');

            for (int i = trueStartY; i <= trueEndY; i++)
            {
                if (i >= lines.Length)
                    break;

                // If it's the start and end line of the selection
                if (i == trueStartY && i == trueEndY)
                {
                    if (lines[i].Length >= trueEndX + 1)
                    {
                        selectedText.Append(lines[i].Substring(trueStartX, trueEndX - trueStartX + 1));
                    }
                    else if (lines[i].Length > trueStartX)
                    {
                        selectedText.Append(lines[i].Substring(trueStartX));
                    }
                }
                // If it's the start line of the selection
                else if (i == trueStartY)
                {
                    if (lines[i].Length > trueStartX)
                    {
                        selectedText.Append(lines[i].Substring(trueStartX));
                    }
                    else
                    {
                        selectedText.Append(lines[i]);
                    }
                }
                // If it's the end line of the selection
                else if (i == trueEndY)
                {
                    if (lines[i].Length >= trueEndX + 1)
                    {
                        selectedText.Append(lines[i].Substring(0, trueEndX + 1));
                    }
                    else
                    {
                        selectedText.Append(lines[i]);
                    }
                }
                // Any line in between
                else
                {
                    selectedText.Append(lines[i]);
                }

                // Add a newline character for every line except the last one
                if (i != trueEndY)
                {
                    selectedText.Append('\n');
                }
            }

            Terminal.Clear();
            Console.WriteLine($"{selectedText.ToString()} will be copied. \nPress Enter to continue");
            Console.ReadLine();

            return selectedText.ToString();
        }


        static int GetIndex(string filecontent, int cursorx, int cursory)
        {
            string[] lines = filecontent.Split('\n');

            // Ensure cursory is within the number of lines
            if (cursory >= lines.Length)
            {
                cursory = lines.Length - 1;
            }

            // Ensure cursorx is within the length of the line at cursory
            if (cursory >= 0 && cursorx > lines[cursory].Length)
            {
                cursorx = lines[cursory].Length;
            }

            int index = 0;
            for (int i = 0; i < cursory; i++)
            {
                index += lines[i].Length + 1; // +1 for the '\n' character
            }

            index += cursorx;

            return index;
        }

        static (List<formated>, int, int) FileFormater(string filecontent, int x, int y, int maxwidth, int maxheight,
            bool isSidePanel)
        {
            List<string> lines = filecontent.Split('\n').ToList();
            List<string> formattedLines = new List<string>();
            List<formated> formattedWithColor = new();

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

            for (int i = startLine; i < endLine; i++)
            {
                string line = lines[i];
                // Start line for horizontal scrolling
                if (line.Length > maxwidth)
                {
                    if (i == y)
                    {
                        startChar = Math.Max(0, Math.Min(x - (maxwidth), line.Length - maxwidth));
                    }
                    else
                    {
                        startChar = 0;
                    }
                }
                else
                {
                    startChar = 0;
                }

                List<formated> formattedLine = ProcessLineForColor(line, startChar, i, maxwidth);
                formattedWithColor.AddRange(formattedLine);
            }

            while (formattedWithColor.Count < maxheight - 2)
            {
                formattedWithColor.Add(new formated { Text = "", Color = ConsoleColor.White, NewLine = true });
            }

            // If Sidepanl is activated replace right side with the panel
            if (isSidePanel)
            {
                for (int i = 0; i < formattedLines.Count; i++)
                {
                    if (i >= sidePanelContent.Count) // Check if were beyond the length of sidepanel
                    {
                        break;
                    }

                    int totalContentLength = formattedLines[i].Length + sidePanelContent[i].Length;
                    int paddingLength = maxwidth - totalContentLength; // Calculate the number of spaces required

                    if (paddingLength < 0) // If content is longer than maxwidth
                    {
                        formattedLines[i] = formattedLines[i].Substring(0, maxwidth - sidePanelContent[i].Length);
                        paddingLength = 0;
                    }

                    formattedLines[i] = formattedLines[i] + new string(' ', paddingLength) + sidePanelContent[i];
                }
            }

            return (formattedWithColor, startLine, startChar);
        }


        static void FileRender(List<formated> text)
        {
            foreach (var obj in text) // TODO: Optimze
            {
                if (obj.NewLine)
                {
                    Console.ForegroundColor = obj.Color;
                    Console.WriteLine(obj.Text);
                }
                else
                {
                    Console.ForegroundColor = obj.Color;
                    Console.Write(obj.Text);
                }
            }

            if (isSelection)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.Write("[Selection Mode]");
                Console.ResetColor();
            }
        }


        static int GetMaxCharacter(string filecontent, int line)
        {
            try
            {
                string[] lines = filecontent.Split('\n');
                int numberOfCharacters = lines[line].Count();
                return numberOfCharacters;
            }
            catch
            {
                return 0;
            }
        }

        private static List<formated> ProcessLineForColor(string line, int startChar, int i, int maxwidth)
        {
            List<formated> formattedLine = new List<formated>();
            List<ColorsGlobal> ColorsForThisLine = new();
            foreach (var obj in GlobalColorList)
            {
                if (obj.line == i)
                {
                    ColorsForThisLine.Add(obj);
                }
            }

            string lineWithoutColor = line.Substring(startChar, Math.Min(maxwidth, line.Length - startChar));
    
            for (int j = startChar; j < lineWithoutColor.Length; )
            {
                var colorItem = ColorsForThisLine.FirstOrDefault(item => item.StartChar == j);
                if (colorItem != null)
                {
                    int length = Math.Min(colorItem.EndChar - j, lineWithoutColor.Length - j);
                    formattedLine.Add(new formated
                    {
                        Text = line.Substring(j, length),
                        Color = colorItem.Color,
                        NewLine = (j + length) >= lineWithoutColor.Length
                    });
                    j += length;
                }
                else
                {
                    var nextColorItems = ColorsForThisLine.Where(item => item.StartChar > j);
                    int nextColorStart;

                    if (nextColorItems.Any())
                    {
                        nextColorStart = nextColorItems.Min(item => item.StartChar) - j;
                    }
                    else
                    {
                        // If there are no more color items, use the rest of the line
                        nextColorStart = lineWithoutColor.Length - j;
                    }

                    int length = Math.Min(nextColorStart, lineWithoutColor.Length - j);
                    formattedLine.Add(new formated
                    {
                        Text = line.Substring(j, length),
                        // Default color or some logic to determine the color
                        Color = ConsoleColor.White, 
                        NewLine = (j + length) >= lineWithoutColor.Length
                    });
                    j += length;
                }

            }

            return formattedLine;
        }


    }
}