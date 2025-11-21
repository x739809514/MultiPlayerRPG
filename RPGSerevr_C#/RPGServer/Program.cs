using System;
using System.Threading;

namespace RPGServer
{
    class Program
    {
        private static GameServer gameServer;

        static void Main(string[] args)
        {
            Console.WriteLine("=== RPG Server ===");
            Console.WriteLine("Starting server...");

            gameServer = new GameServer();

            // 设置 Ctrl+C 处理
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                Console.WriteLine("\nShutting down server...");
                gameServer.Stop();
                Environment.Exit(0);
            };

            // 在新线程启动服务器
            Thread serverThread = new Thread(gameServer.Start);
            serverThread.Start();

            // 主线程保持运行，等待用户输入
            Console.WriteLine("Press 'q' to quit");
            while (true)
            {
                var key = Console.ReadLine();
                if (key?.ToLower() == "q")
                {
                    Console.WriteLine("Shutting down server...");
                    gameServer.Stop();
                    break;
                }
            }
        }
    }
}

