# Plugin Developer Documentation

A plugin is just a .dll file added into the applications folder.

What does the .dll need:

Two public Functions:

1. Plugin Info Function
2. The Main Function

1. The Public Function will be called whenever the plugin gets loaded. It needs to return an object. That object needs to return the following:
   - PluginInfo Object:
     - Name: The plugin's name
     - Function Name: The name of the Main Function (2.)
     - List<string> FileExtensionDefault: All the file extensions this plugin needs to be opened with by default
     - Version: The NoteConsole version this plugin was developed for. (Note: It will work on newer or older versions that haven't changed anything about how plugins work, which could break it. It gets automatically disabled if such changes are detected.)

   Heres an example code:
   ```
   TODO
   ```

2. The Main function:
   Input:
   - file content
   - cursor x
   - cursor y
   - plugin-specific setting (string)

   Output:
   - GlobalColorsList
   - plugin-specific setting (string)
