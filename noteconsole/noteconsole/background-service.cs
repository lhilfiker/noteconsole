namespace noteconsole
{
    internal partial class Program
    {
        public static List<ColorsGlobal> GlobalColorList = new();
        public static void StartBackgroundServices()
        {
            List<ColorsGlobal> ColorsListBuffer = new();
            string buffer = Filecontent;
            while (true)
            {
                ColorsListBuffer.Clear();
                
                GlobalColorList.Clear();
                GlobalColorList = ColorsListBuffer.ToList();

                while (Filecontent != buffer)
                {
                }

                buffer = Filecontent;}
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