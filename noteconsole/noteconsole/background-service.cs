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