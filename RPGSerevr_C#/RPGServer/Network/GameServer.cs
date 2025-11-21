using System.Net;
using System.Net.Sockets;
using RPGServer.Data;
using RPGServer.Network;
using RPGServer.Service;
using RPGServer.Service.Interface;

namespace RPGServer;

public class GameServer
{
    private readonly ServerConfig serverConfig;
    private IUserService userService;
    private TcpListener tcpListener;

    private List<ClientSession> sessionList = new List<ClientSession>();
    private bool isRunning;
    private readonly object _lock = new object();

    public GameServer()
    {
        serverConfig = new ServerConfig();

        var userRepository = new UserRepository();
        userService = new UserServices(userRepository);
    }

    public void Start()
    {
        try
        {
            tcpListener = new TcpListener(IPAddress.Any, serverConfig.Port);
            tcpListener.Start();
            isRunning = true;

            // creat core thread
            Thread acceptThread = new Thread(AcceptListener);
            acceptThread.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine($"server start failed! {e}");
            throw;
        }
    }

    private void AcceptListener()
    {
        Console.WriteLine($"Server started on port {serverConfig.Port}");
        Console.WriteLine($"Max connections: {serverConfig.MaxSessionCount}");

        while (isRunning)
        {
            try
            {
                TcpClient client = tcpListener.AcceptTcpClient();

                lock (_lock)
                {
                    // 清理断开的 session
                    CleanDisconnectedSessions();

                    if (sessionList.Count >= serverConfig.MaxSessionCount)
                    {
                        Console.WriteLine("Reached max connections, refusing new connection");
                        client.Close();
                        continue;
                    }

                    var session = new ClientSession(client, userService, serverConfig.BufferSize);
                    sessionList.Add(session);

                    // start session thread
                    Thread sessionThread = new Thread(() =>
                    {
                        session.Start();
                        // Session 结束后从列表中移除
                        RemoveSession(session);
                    });
                    sessionThread.Start();

                    Console.WriteLine($"New Connection, Current connections: {sessionList.Count}");
                }
            }
            catch (Exception e)
            {
                if (isRunning) { Console.WriteLine($"Accept connection failed: {e.Message}"); }
            }
        }
    }

    private void CleanDisconnectedSessions()
    {
        // 注意：调用此方法时必须已持有 _lock
        sessionList.RemoveAll(s => !s.IsConnected());
    }

    private void RemoveSession(ClientSession session)
    {
        lock (_lock)
        {
            sessionList.Remove(session);
            Console.WriteLine($"Session removed, Current connections: {sessionList.Count}");
        }
    }

    public void Stop()
    {
        if (!isRunning) return;

        isRunning = false;
        tcpListener?.Stop();

        lock (_lock)
        {
            Console.WriteLine($"Closing {sessionList.Count} active sessions...");
            sessionList.Clear();
        }

        Console.WriteLine("Server stopped");
    }
}