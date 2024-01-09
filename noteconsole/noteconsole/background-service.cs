using System.Reflection;
using PluginShared;
using System.IO;
using System.Threading;

namespace noteconsole
{

    internal partial class Program
    {
        public static List<Shared.ColorsGlobal> GlobalColorList = new();
        
        private static AutoResetEvent fileContentUpdatedEvent = new AutoResetEvent(false);

        
        public static void StartBackgroundServices()
        {
            // Load the Plugins
            List<Shared.PluginInfo> pluginInfoList;
            List<dynamic> pluginInstances;
            (pluginInfoList, pluginInstances) = LoadDisplayPlugins();
            
            List<Shared.ColorsGlobal> ColorsListBuffer = new();
            string? buffer = Filecontent;
            string extensionBuffer = Path.GetExtension(filepath);
            dynamic currentPlugin = null;
            
            while (true)
            {
                if (global.plugins == true)
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
                        catch (Exception ex)
                        {
                        }
                    }


                    GlobalColorList.Clear();
                    GlobalColorList.AddRange(ColorsListBuffer);

                    fileContentUpdatedEvent.WaitOne();

                    buffer = Filecontent;
                }
                else
                {
                    GlobalColorList.Clear();
                    Thread.Sleep(1000); // If plugins are disabled.
                }
                
            }
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
                                if (info.Version >= global.latestSupportedPluginVersion && info.Version <= global.version)
                                {
                                    pluginInfoList.Add(info);
                                    pluginInstances.Add(pluginInstance);    
                                }
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