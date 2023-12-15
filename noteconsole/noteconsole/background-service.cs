using System.Reflection;

namespace noteconsole
{
    public class PluginInfo
    {
        public string Name { get; set; }
        public string FunctionName { get; set; }
        public List<string> FileExtensionDefault { get; set; }
        public string Version { get; set; }
    }

    internal partial class Program
    {
        public static List<ColorsGlobal> GlobalColorList = new();
        public static void StartBackgroundServices()
        {
            // Load the Plugins
            List<PluginInfo> pluginInfoList;
            List<IPlugin> pluginInstances;
            (pluginInfoList, pluginInstances) = LoadDisplayPlugins();
            
            List<ColorsGlobal> ColorsListBuffer = new();
            string buffer = Filecontent;
            string extensionBuffer = Path.GetExtension(filepath);
            IPlugin currentPlugin = null;
            
            while (true)
            {
                ColorsListBuffer.Clear();
                // Check if extension changed and if so load a new plugin
                string currentExtension = Path.GetExtension(filepath);
                if (currentExtension != extensionBuffer)
                {
                    extensionBuffer = currentExtension;
                    currentPlugin = FindPluginForExtension(pluginInfoList, pluginInstances, currentExtension);
                }
                
                if (currentPlugin != null)
                {
                    try
                    {
                        ColorsListBuffer = currentPlugin.MainFunction(buffer, cursorX, cursorY);
                    }
                    catch
                    {
                    }
                }

                GlobalColorList.Clear();
                GlobalColorList.AddRange(ColorsListBuffer);

                while (Filecontent != buffer)
                {
                }

                buffer = Filecontent;}
            }
        
        private static IPlugin FindPluginForExtension(List<PluginInfo> pluginInfoList, List<IPlugin> pluginInstances, string extension)
        {
            for (int i = 0; i < pluginInfoList.Count; i++)
            {
                if (pluginInfoList[i].FileExtensionDefault.Contains(extension))
                {
                    return pluginInstances[i];
                }
            }
            return null;
        }
        
        private static (List<PluginInfo>, List<IPlugin>) LoadDisplayPlugins()
        {
            string pluginPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "noteconsole", "plugins");
            List<PluginInfo> pluginInfoList = new List<PluginInfo>();
            List<IPlugin> pluginInstances = new List<IPlugin>();

            try
            {
                if (!Directory.Exists(pluginPath))
                {
                    Directory.CreateDirectory(pluginPath);
                }

                foreach (string file in Directory.GetFiles(pluginPath, "*.dll"))
                {
                    Assembly pluginAssembly = Assembly.LoadFrom(file);
                    foreach (Type type in pluginAssembly.GetTypes())
                    {
                        if (typeof(IPlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                        {
                            IPlugin pluginInstance = (IPlugin)Activator.CreateInstance(type);
                            PluginInfo info = pluginInstance.GetPluginInfo();
                            pluginInfoList.Add(info);
                            pluginInstances.Add(pluginInstance);
                        }
                    }
                }
            }
            catch
            {
                return (new List<PluginInfo>(), new List<IPlugin>());
            }

            return (pluginInfoList, pluginInstances);
        }

    }

    public class ColorsGlobal
    {
        public int line { get; set; }
        public int StartChar { get; set; } 
        public int EndChar { get; set; } 
        public ConsoleColor Color { get; set; }
        
        public ConsoleColor BackgroundColor { get; set; }
    }

}    