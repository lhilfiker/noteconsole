namespace noteconsole
{
    public interface IPlugin
    {
        PluginInfo GetPluginInfo();
        List<ColorsGlobal> MainFunction(string buffer, int cursorX, int cursorY);
    }
}