using System;
using System.Threading;

namespace RPGServer
{
    class Program
    {
        private static Thread threadConsole;
        private static bool consoleRunning;

        static void Main(string[] args)
        {
            threadConsole = new Thread(ConsoleThread);
            threadConsole.Start();
            
        }

        private static void ConsoleThread()
        {
            string line;
            consoleRunning = true;
            while (consoleRunning)
            {
                line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    consoleRunning = false;
                    return;
                }
            }
        }
    }
}

