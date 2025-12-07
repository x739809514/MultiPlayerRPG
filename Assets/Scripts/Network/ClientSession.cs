namespace Network
{
    public static class ClientSession
    {
        public static long Uid { get; private set; }
        public static string SessionId { get; private set; }

        public static void Set(long uid, string sessionId)
        {
            Uid = uid;
            SessionId = sessionId;
        }

        public static bool IsLogin => Uid > 0 && !string.IsNullOrEmpty(SessionId);
    }
}