using System;
using System.Collections.Generic;
using PluginShared;

namespace ExamplePlugin 
{
    public class MyColorPlugin : Shared.IPlugin
    {
        public Shared.PluginInfo GetPluginInfo()
        {
            return new Shared.PluginInfo
            {
                Name = "MyColorPlugin",
                FunctionName = nameof(MainFunction),
                FileExtensionDefault = new List<string> { ".txt" },
                Version = "1.0"
            };
        }

        public List<Shared.ColorsGlobal> MainFunction(string buffer, int cursorX, int cursorY)
        {
            List<Shared.ColorsGlobal> colorSettings = new List<Shared.ColorsGlobal>();

            // Create a Random object
            Random rnd = new Random();

            // Generate a random ConsoleColor, excluding Black as it's often used for background
            ConsoleColor randomColor = (ConsoleColor)rnd.Next(1, 16); // Enum values of ConsoleColor range from 0 to 15

            // Highlight characters 1 to 10 of the first line in a random color
            colorSettings.Add(new Shared.ColorsGlobal
            {
                line = 1, // Assuming lines are 1-indexed
                StartChar = 1,
                EndChar = 10,
                Color = randomColor,
                BackgroundColor = ConsoleColor.Black
            });

            return colorSettings;
        }
    }
}