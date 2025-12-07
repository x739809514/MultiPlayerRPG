using System.Net.Sockets;

namespace RPGServer.Session;

public static class SessionManager
{
    private const int timeOutSeconds = 25;
    private static Dictionary<long, Session> sessionDic = new Dictionary<long, Session>();
    private static object _lock = new object();

    public static void CreateSession(long uid, string sessionId, TcpClient client)
    {
        lock (_lock)
        {
            var session = new Session();
            session.uid = uid;
            session.sessionId = sessionId;
            session.client = client;
            var expireTime = DateTime.UtcNow.AddSeconds(timeOutSeconds);
            session.expireTime = expireTime;
            sessionDic.Add(uid,session);
        }
    }

    public static bool Validation(long uid, string sessionId)
    {
        lock (_lock)
        {
            if (sessionDic.TryGetValue(uid,out var session)==false)
            {
                return false;
            }

            if (session.sessionId!=sessionId)
            {
                return false;
            }

            session.expireTime = DateTime.UtcNow.AddSeconds(timeOutSeconds);
            return true; 
        }
    }

    public static void Remove(long uid)
    {
        lock (_lock)
        {
            if (sessionDic.TryGetValue(uid,out var s))
            {
                s.client.Close();
                sessionDic.Remove(uid);
            }
        }
    }

    public static IEnumerable<Session> GetAllSession()
    {
        lock (_lock)
        {
            return sessionDic.Values.ToArray();
        }
    }
}