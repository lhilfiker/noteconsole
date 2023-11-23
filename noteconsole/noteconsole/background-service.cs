namespace noteconsole
{
    internal partial class Program
    {
        public static List<ColorsGlobal> GlobalColorList = new();
        public static void StartBackgroundServices()
        {
            while (true)
            {
                string[] lines = Filecontent.Split('\n');
                List<ColorsGlobal> ColorsListBuffer = new();

                GlobalColorList.Add(new ColorsGlobal{line = 0, StartChar = 0, EndChar = 5, Color = ConsoleColor.Blue});

                Thread.Sleep(100); // TODO: Better mechanism
            }
        }
        
        
    }

    public class ColorsGlobal
    {
        public int line { get; set; }
        public int StartChar { get; set; } 
        public int EndChar { get; set; } 
        public ConsoleColor Color { get; set; }
    }

}    