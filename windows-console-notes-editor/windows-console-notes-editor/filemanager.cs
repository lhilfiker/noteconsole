using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace windows_console_notes_editor
{
    internal partial class Program
    {
        public static void FileManager(string filepath)
        {
            string filecontent = File.ReadAllText(filepath);
            int cursorx = 0;
            int cursory = 0;

            Console.CursorVisible = true;

            List<int> maxCharactersPerLine = new List<int>();
            Console.Clear();
            Console.WriteLine("Loading...");
            // Precompute max characters for each line
            foreach (var line in filecontent.Split('\n'))
            {
                maxCharactersPerLine.Add(line.Length);
            }

            while (true)
            {
                int maxline = maxCharactersPerLine.Count();
                int maxcharacter = maxCharactersPerLine[cursory];
                int maxwidth = Console.WindowWidth;
                int maxheight = Console.WindowHeight;

                (List<string> formatedtext, int startLine, int startChar) = FileFormater(filecontent, cursorx, cursory, maxwidth, maxheight);
                FileRender(formatedtext);

                int displayedCursorX = Math.Min(Math.Max(cursorx - startChar, 0), maxwidth - 1);
                int displayedCursorY = Math.Min(Math.Max(cursory - startLine, 0), maxheight - 1);

                Console.SetCursorPosition(displayedCursorX, displayedCursorY);

                ConsoleKeyInfo pressedKey = Console.ReadKey();


                switch (pressedKey.Key)
                {
                    case ConsoleKey.DownArrow:
                        if (cursory < maxline - 1)
                        {
                            cursory++;
                            cursorx = Math.Min(cursorx, maxcharacter);
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        cursorx = Math.Max(0, cursorx - 1);
                        break;
                    case ConsoleKey.RightArrow:
                        cursorx = Math.Min(cursorx + 1, maxcharacter - 1);
                        break;

                    case ConsoleKey.UpArrow:
                        if (cursory > 0)
                        {
                            cursory--;
                            cursorx = Math.Min(cursorx, maxcharacter);
                        }
                        break;
                    case ConsoleKey.S when pressedKey.Modifiers.HasFlag(ConsoleModifiers.Control):
                        File.WriteAllText(filepath, filecontent);
                        break;
                    case ConsoleKey.Escape:
                        Console.Clear();
                        if (filecontent != File.ReadAllText(filepath))
                        {
                            Console.WriteLine("SSave changes? (Y/N)");
                            ConsoleKeyInfo saveKey = Console.ReadKey();
                            if (saveKey.Key == ConsoleKey.Y)
                            {
                                File.WriteAllText(filepath, filecontent);
                            }
                        }
                        return;
                    case ConsoleKey.Delete:
                        //If cursor is not == 0, then remove the character before it and move the cursor back

                        if (cursorx != 0)
                        {
                            int deleteIndex = GetIndex(filecontent, cursorx, cursory);
                            filecontent = filecontent.Remove(deleteIndex - 1, 1);
                            cursorx--;
                        }
                        break;

                    case ConsoleKey.Enter:
                        int enterIndex = GetIndex(filecontent, cursorx, cursory);
                        string line = filecontent.Substring(enterIndex);
                        filecontent = filecontent.Insert(enterIndex, "\n");
                        cursory++;
                        maxCharactersPerLine.Insert(cursory, GetMaxCharacter(filecontent, cursory));
                        maxCharactersPerLine[cursory -1] = GetMaxCharacter(filecontent, cursory - 1);
                        break;

                    case ConsoleKey.Spacebar:
                        int spaceIndex = GetIndex(filecontent, cursorx, cursory);
                        filecontent = filecontent.Insert(spaceIndex, " ");
                        cursorx++;
                        break;
                    default:
                        char keyChar = pressedKey.KeyChar;
                        int charIndex = GetIndex(filecontent, cursorx, cursory);
                        filecontent = filecontent.Insert(charIndex, keyChar.ToString());
                        cursorx++;
                        maxCharactersPerLine[cursory] = GetMaxCharacter(filecontent, cursory);
                        break;
                }
                //Make sure cursor is within boundaries
                if (cursorx > maxCharactersPerLine[cursory])
                {
                    cursorx = maxCharactersPerLine[cursory];
                }
            }
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
                index += lines[i].Length + 1;  // +1 for the '\n' character
            }
            index += cursorx;

            return index;
        }

        static (List<string>, int, int) FileFormater(string filecontent, int x, int y, int maxwidth, int maxheight)
        {
            List<string> lines = filecontent.Split('\n').ToList();
            List<string> formattedLines = new List<string>();

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
                startLine = Math.Max(0, lines.Count - maxheight);
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

                formattedLines.Add(line.Substring(startChar, Math.Min(maxwidth, line.Length - startChar)));
            }

            return (formattedLines, startLine, startChar);
        }


        static void FileRender(List<string> text)
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            foreach (string line in text)
            {
                Console.WriteLine(line);
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

        static int GetMaxLine(string filecontent)
        {
            try
            {
                string[] lines = filecontent.Split('\n');
                int numberOfLines = lines.Length;
                return numberOfLines;
            }
            catch
            {
                return 0;
            }
        }
    }
}
