using System;
using System.Collections.Generic;
using PluginShared;

// Make sure this namespace matches with where you've defined the IPlugin interface
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
            // Here, implement your logic to analyze the buffer and determine color settings
            List<Shared.ColorsGlobal> colorSettings = new List<Shared.ColorsGlobal>();

            // Example: Change the color of a specific line
            colorSettings.Add(new Shared.ColorsGlobal 
            {
                line = 1,
                StartChar = 0,
                EndChar = 10,
                Color = ConsoleColor.Red,
                BackgroundColor = ConsoleColor.Black
            });

            return colorSettings;
        }
    }

    // Assuming ColorsGlobal and PluginInfo classes are defined in your main application
}