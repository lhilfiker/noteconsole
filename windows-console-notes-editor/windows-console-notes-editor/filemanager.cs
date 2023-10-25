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

            while (true)
            {
                int maxline = GetMaxLine(filecontent);
                int maxcharacter = GetMaxCharacter(filecontent, cursory);
                int maxwidth = Console.WindowWidth;
                int maxheight = Console.WindowHeight;

                List<string> formatedtext = new();
                formatedtext = FileFormater(filecontent, cursorx, cursory, maxwidth, maxheight);
                FileRender(formatedtext);

                ConsoleKeyInfo pressedKey = Console.ReadKey();

                int currentLineLength;

                switch (pressedKey.Key)
                {
                    case ConsoleKey.DownArrow:
                        if (cursory < maxline)
                        {
                            cursory++;
                            currentLineLength = GetMaxCharacter(filecontent, cursory);
                            cursorx = Math.Min(cursorx, currentLineLength);
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        cursorx = Math.Max(0, cursorx - 1);
                        break;
                    case ConsoleKey.RightArrow:
                        currentLineLength = GetMaxCharacter(filecontent, cursory);
                        cursorx = Math.Min(cursorx + 1, currentLineLength);
                        break;
                    case ConsoleKey.UpArrow:
                        if (cursory > 0)
                        {
                            cursory--;
                            currentLineLength = GetMaxCharacter(filecontent, cursory);
                            cursorx = Math.Min(cursorx, currentLineLength);
                        }
                        break;
                }


            }
        }




        static List<string> FileFormater(string filecontent, int x, int y, int maxwidth, int maxheight)
        {
            List<string> lines = filecontent.Split('\n').ToList();
            List<string> formattedLines = new List<string>();

            // Ensure y is wihin boundaries
            y = Math.Min(y, lines.Count - 1);

            // Calculate start line fr vertical scrollng
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

            int endLine = Math.Min(lines.Count, startLine + maxheight -1);

            for (int i = startLine; i < endLine; i++)
            {
                string line = lines[i];

                // Calculate start character for horizontal scrolling
                int startChar;
                if (line.Length > maxwidth)
                {
                    if (i == y)
                    {
                        startChar = Math.Max(0, x - (maxwidth / 2));
                        startChar = Math.Min(startChar, line.Length - maxwidth);
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

            return formattedLines;
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
