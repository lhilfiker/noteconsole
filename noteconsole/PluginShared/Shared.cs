namespace PluginShared;

public class Shared
{
    public interface IPlugin
    {
        PluginInfo GetPluginInfo();
        List<ColorsGlobal> MainFunction(string buffer, int cursorX, int cursorY);
    }
    
    public class ColorsGlobal
    {
        public int line { get; set; }
        public int StartChar { get; set; } 
        public int EndChar { get; set; } 
        public ConsoleColor Color { get; set; }
        
        public ConsoleColor BackgroundColor { get; set; }
    }
    
    public class PluginInfo
    {
        public string Name { get; set; }
        public string FunctionName { get; set; }
        public List<string> FileExtensionDefault { get; set; }
        public string Version { get; set; }
    }
}