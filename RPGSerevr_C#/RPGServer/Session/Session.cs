using System.Data;
using System.Net.Sockets;

namespace RPGServer.Session;

public class Session
{
    public long uid;
    public string sessionId;
    public TcpClient client;
    public DateTime expireTime;
}