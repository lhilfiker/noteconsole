using System.Runtime.InteropServices;
namespace noteconsole
{
    internal class global
    {
        public static int version = 040; // 040 = v.0.4.0
        public static int latestSupportedPluginVersion = 040;
        public static bool os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool plugins = true;
    }
}