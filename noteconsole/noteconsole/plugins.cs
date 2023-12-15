namespace noteconsole
{
    public interface IPlugin
    {
        PluginInfo GetPluginInfo();
        List<ColorsGlobal> MainFunction();
    }


}