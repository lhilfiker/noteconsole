## Plugin Developer Documentation

### Overview
Plugins are designed to handle the display of colors. Each plugin is a .dll file that can be added to the application's plugins folder.

### Plugin DLL Requirements
Each plugin DLL should implement two public functions:

1. **Plugin Info Function**: Provides metadata about the plugin.
2. **Main Function**: The core functionality of the plugin.

#### 1. Plugin Info Function
This function is called when the plugin is loaded and should return an object with the following properties:
- **Name**: Name of the plugin.
- **Function Name**: Name of the Main Function.
- **FileExtensionDefault**: List of file extensions associated with this plugin by default.
- **Version**: Version of the console notes application this plugin was developed for.

#### 2. Main Function
This function is invoked to modify the color display settings of the notes.

**Input Parameters**:
- `fileContent` (string): The current content of the file.
- `cursorX` (int): The X-coordinate of the cursor.
- `cursorY` (int): The Y-coordinate of the cursor.

**Output**:
- `List<ColorsGlobal>`: A list representing color formatting settings for various parts of the text.

### `ColorsGlobal` Class
The `ColorsGlobal` class defines color formatting for specific text segments:
```
public class ColorsGlobal
{
    public int Line { get; set; }
    public int StartChar { get; set; } 
    public int EndChar { get; set; } 
    public ConsoleColor Color { get; set; }
    public ConsoleColor BackgroundColor { get; set; }
}
```

### Example Plugin Code

```
using System;
using System.Collections.Generic;

public class MyColorPlugin : IPlugin
{
    public PluginInfo GetPluginInfo()
    {
        return new PluginInfo
        {
            Name = "MyColorPlugin",
            FunctionName = nameof(MainFunction),
            FileExtensionDefault = new List<string> { ".txt" },
            Version = "1.0"
        };
    }

    public List<ColorsGlobal> MainFunction(string fileContent, int cursorX, int cursorY)
    {
        List<ColorsGlobal> colorSettings = new List<ColorsGlobal>();
        // Implement the logic to determine color formatting based on fileContent, cursorX, and cursorY
        
        // Example: Change the color of a specific line
        colorSettings.Add(new ColorsGlobal 
        {
            Line = 1, 
            StartChar = 0, 
            EndChar = 10, 
            Color = ConsoleColor.Red, 
            BackgroundColor = ConsoleColor.Black
        });

        return colorSettings;
    }
}
```

### Building the Plugin DLL

1. **Create a Class Library Project**: In your IDE (e.g., Visual Studio), start a new Class Library project targeting .NET 6.0.

2. **Implement the IPlugin Interface**: Your plugin class should implement the `IPlugin` interface as illustrated.

3. **Build the Project**: Compile your project to generate the DLL.

4. **Deploy the DLL**: Place the DLL in the application's plugins folder.

### Notes
- Ensure the plugin DLLs are compatible with the .NET version of your console notes application.
- Incorporate error handling and logging within your plugin for easier debugging and to enhance stability.


This updated documentation reflects the specific requirements of your console notes application, focusing on the color display functionality through plugins.