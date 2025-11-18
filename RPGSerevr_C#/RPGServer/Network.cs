using System.Net;
using System.Net.Sockets;
namespace RPGServer;

public class Network
{
    public TcpListener serverSocket;
    public static Network instance = new Network();
    public static Client[] clients = new Client[100];

    public void ServerStart()
    {
        for (int i = 1; i < 100; i++)
        {
            clients[i] = new Client();
        }
        
        serverSocket = new TcpListener(IPAddress.Any, 5500);
        serverSocket.Start();
        serverSocket.BeginAcceptTcpClient(OnClientConnect, null);
        Console.WriteLine("Server has successfully started");
    }

    private void OnClientConnect(IAsyncResult result)
    {
        TcpClient client = serverSocket.EndAcceptTcpClient(result);
        client.NoDelay = false;
        serverSocket.BeginAcceptTcpClient(OnClientConnect, null);

        for (int i = 1; i < 100; i++)
        {
            if (clients[i].socket==null)
            {
                clients[i].socket = client;
                clients[i].index = i;
                clients[i].ip = client.Client.RemoteEndPoint.ToString();
                clients[i].Start();
                Console.WriteLine("InComing Connect from" + clients[i].ip+"|| Index" + i);
                
                // Send Message
                
                // 这里需要return，否则别人没法连了
                return;
            }
        }
    }
}