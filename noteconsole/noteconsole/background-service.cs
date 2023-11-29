namespace noteconsole
{
    internal partial class Program
    {
        public static List<ColorsGlobal> GlobalColorList = new();
        public static void StartBackgroundServices()
        {
            List<ColorsGlobal> ColorsListBuffer = new();
            while (true)
            {
                ColorsListBuffer.Clear();
                ColorsListBuffer.Add(new ColorsGlobal{line = 0, StartChar = 0, EndChar = 5, Color = ConsoleColor.Blue, BackgroundColor = ConsoleColor.Yellow});
                ColorsListBuffer.Add(new ColorsGlobal{line = 1, StartChar = 0, EndChar = 10, Color = ConsoleColor.Green});
                ColorsListBuffer.Add(new ColorsGlobal{line = 1, StartChar = 11, EndChar = 20, Color = ConsoleColor.Red});
                ColorsListBuffer.Add(new ColorsGlobal{line = 2, StartChar = 0, EndChar = 50, Color = ConsoleColor.DarkCyan});

                GlobalColorList.Clear();
                GlobalColorList = ColorsListBuffer.ToList();
                
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
        
        public ConsoleColor BackgroundColor { get; set; }
    }

}    