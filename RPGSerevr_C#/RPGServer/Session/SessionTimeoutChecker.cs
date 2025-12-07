namespace RPGServer.Session;

public static class SessionTimeoutChecker
{
    private static CancellationTokenSource cts = new CancellationTokenSource();

    public static void Start()
    {
        Task.Run(CheckIfTimeout);
    }

    private static async void CheckIfTimeout()
    {
        while (cts.IsCancellationRequested==false)
        {
            var allSession = SessionManager.GetAllSession();
            foreach (var session in allSession)
            {
                if (session.expireTime<DateTime.Now)
                {
                    Console.WriteLine($"Session {session.sessionId} is expired");
                    SessionManager.Remove(session.uid);
                }
            }

            await Task.Delay(5000);
        }
    }

    public static void Stop()
    {
        cts.Cancel();
    }
}