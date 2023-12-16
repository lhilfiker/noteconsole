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

}