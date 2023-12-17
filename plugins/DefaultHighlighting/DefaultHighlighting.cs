using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PluginShared;

namespace DefaultHighlighting;

public class DefaultHighlighting
{
    public class MyColorPlugin : Shared.IPlugin
    {
        public Shared.PluginInfo GetPluginInfo()
        {
            return new Shared.PluginInfo
            {
                Name = "DefaultHighlighting",
                FunctionName = nameof(MainFunction),
                FileExtensionDefault = new List<string> { ".txt", ".md" },
                Version = 040
            };
        }

        public List<Shared.ColorsGlobal> MainFunction(string buffer, int cursorX, int cursorY)
        {
            List<Shared.ColorsGlobal> colorSettings = new List<Shared.ColorsGlobal>();

            var lines = buffer.Split(new[] { '\n' }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();

                // Headings
                if (line.StartsWith("#"))
                {
                    colorSettings.Add(CreateColorSetting(i, 0, line.Length, ConsoleColor.Cyan, ConsoleColor.Black));
                }

                // Hyperlinks
                if (line.Contains("[") && line.Contains("]"))
                {
                    colorSettings.Add(CreateColorSetting(i, line.IndexOf("["), line.IndexOf("]") + 1, ConsoleColor.Blue,
                        ConsoleColor.Black));
                }

                // Quoted Text
                if (line.StartsWith(">"))
                {
                    colorSettings.Add(CreateColorSetting(i, 0, line.Length, ConsoleColor.DarkGray, ConsoleColor.Black));
                }

                // Bullet Points
                if (line.StartsWith("*") || line.StartsWith("-") || line.StartsWith("+"))
                {
                    colorSettings.Add(CreateColorSetting(i, 0, line.Length, ConsoleColor.Green, ConsoleColor.Black));
                }

                // Dates
                var dateMatch = Regex.Match(line, @"\b\d{4}-\d{2}-\d{2}\b");
                if (dateMatch.Success)
                {
                    colorSettings.Add(CreateColorSetting(i, dateMatch.Index, dateMatch.Index + dateMatch.Length,
                        ConsoleColor.DarkYellow, ConsoleColor.Black));
                }

                // Strong Text
                var strongTextMatch = Regex.Match(line, @"\*\*(.+?)\*\*|__(.+?)__");
                if (strongTextMatch.Success)
                {
                    colorSettings.Add(CreateColorSetting(i, strongTextMatch.Index, strongTextMatch.Index + strongTextMatch.Length, ConsoleColor.DarkRed, ConsoleColor.Black));
                }

                // Emphasized Text
                var emphasizedMatch = Regex.Match(line, @"(?<!\*)\*(?!\*)(.+?)(?<!\*)\*(?!\*)|_(.+?)_");
                if (emphasizedMatch.Success)
                {
                    colorSettings.Add(CreateColorSetting(i, emphasizedMatch.Index, emphasizedMatch.Index + emphasizedMatch.Length, ConsoleColor.Magenta, ConsoleColor.Black));
                }

                // Task Lists
                var taskListMatch = Regex.Match(line, @"\[.\]");
                if (taskListMatch.Success)
                {
                    colorSettings.Add(CreateColorSetting(i, taskListMatch.Index,
                        taskListMatch.Index + taskListMatch.Length, ConsoleColor.DarkGreen, ConsoleColor.Black));
                }
            }

            return colorSettings;
        }

        private Shared.ColorsGlobal CreateColorSetting(int line, int startChar, int endChar, ConsoleColor color,
            ConsoleColor backgroundColor)
        {
            return new Shared.ColorsGlobal
            {
                line = line,
                StartChar = startChar,
                EndChar = endChar,
                Color = color,
                BackgroundColor = backgroundColor
            };
        }
    }
}