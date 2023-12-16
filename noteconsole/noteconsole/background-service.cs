using System.Reflection;
using PluginShared;

namespace noteconsole
{

    internal partial class Program
    {
        public static List<Shared.ColorsGlobal> GlobalColorList = new();
        public static void StartBackgroundServices()
        {
            // Load the Plugins
            List<Shared.PluginInfo> pluginInfoList;
            List<dynamic> pluginInstances;
            (pluginInfoList, pluginInstances) = LoadDisplayPlugins();
            
            List<Shared.ColorsGlobal> ColorsListBuffer = new();
            string buffer = Filecontent;
            string extensionBuffer = Path.GetExtension(filepath);
            dynamic currentPlugin = null;
            
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
                        Console.WriteLine($"Plugin used successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error using plugin: {ex.Message}");
                    }
                }


                GlobalColorList.Clear();
                GlobalColorList.AddRange(ColorsListBuffer);

                while (Filecontent != buffer)
                {
                }

                buffer = Filecontent;}
            }
        
        private static dynamic FindPluginForExtension(List<Shared.PluginInfo> pluginInfoList, List<dynamic> pluginInstances, string extension)
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
        
        private static (List<Shared.PluginInfo>, List<dynamic>) LoadDisplayPlugins()
        {
            string pluginPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "noteconsole", "plugins");
            var pluginInfoList = new List<Shared.PluginInfo>();
            var pluginInstances = new List<dynamic>();

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
                        if (TypeImplementsIPluginStructure(type) && !type.IsInterface && !type.IsAbstract)
                        {
                            try
                            {
                                dynamic pluginInstance = Activator.CreateInstance(type);
                                Shared.PluginInfo info = pluginInstance.GetPluginInfo();
                                pluginInfoList.Add(info);
                                pluginInstances.Add(pluginInstance);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Failed to load plugin {type.FullName}: {ex.Message}");
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return (new List<Shared.PluginInfo>(), new List<dynamic>());
            }

            return (pluginInfoList, pluginInstances);
        }

        private static bool TypeImplementsIPluginStructure(Type type)
        {
            var getPluginInfoMethod = type.GetMethod("GetPluginInfo");
            var mainFunctionMethod = type.GetMethod("MainFunction");

            return getPluginInfoMethod != null && mainFunctionMethod != null;
        }
    }
    

}    