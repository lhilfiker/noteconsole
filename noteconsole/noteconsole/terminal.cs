using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace noteconsole
{
    internal class Terminal
    {
        public static void Clear()
        {
            // Check if it is running windows
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.Clear();
                return;
            }
        
            try
            {
                Console.Clear(); 
                Console.Write("\x1b[3J");
            }
            catch
            {
                Console.Clear();
            }
        }

    }
}