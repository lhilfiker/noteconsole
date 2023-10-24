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

                List<string> formatedtext = FileFormater(filecontent, cursorx, cursory, maxwidth, maxheight);
                FileRender(formatedtext);

                ConsoleKeyInfo pressedKey = Console.ReadKey();

                switch (pressedKey.Key)
                {
                    case ConsoleKey.DownArrow:
                        if (cursory < maxline - 1)
                        {
                            cursory++;
                            cursorx = Math.Min(cursorx, GetMaxCharacter(filecontent, cursory) - 1);
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (cursorx > 0) cursorx--;
                        break;
                    case ConsoleKey.RightArrow:
                        if (cursorx < maxcharacter - 1) cursorx++;
                        break;
                    case ConsoleKey.UpArrow:
                        if (cursory > 0) cursory--;
                        break;
                }
            }
        }




        static List<string> FileFormater(string filecontent, int x, int y, int maxwidth, int maxheight)
        {
            List<string> lines = filecontent.Split('\n').ToList();
            List<string> formattedLines = new List<string>();

            int startLine = Math.Max(0, y - (maxheight / 2));

            for (int i = startLine; i < lines.Count && i < startLine + maxheight; i++)
            {
                string line = lines[i];
                int start = 0;
                if (line.Length > maxwidth)
                {
                    if (i == y)
                    {
                        start = Math.Max(0, x - (maxwidth / 2));
                        start = Math.Min(start, line.Length - maxwidth);  // Ensure start doesn't go too far right
                    }
                }
                int lengthToTake = Math.Min(maxwidth, line.Length - start);  // Calculate the correct length to take
                formattedLines.Add(line.Substring(start, lengthToTake));

            }

            return formattedLines;
        }


        static void FileRender(List<string> text)
        {
            Console.SetCursorPosition(0, 0);
            foreach (string line in text)
            {
                Console.WriteLine(line.PadRight(Console.WindowWidth, ' '));
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
