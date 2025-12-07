namespace RPGServer.Utilities;

/// <summary>
/// [1bit保留][41bit时间戳][10bit机器ID][12bit序列号]
/// 雪花算法，可以在分布式的服务器架构中生成user的uid
/// </summary>
public static class SnowFlake
{
    public static readonly long Epoch = new DateTime(2020, 1, 1).Ticks / TimeSpan.TicksPerMillisecond;
    
    private static long machineId = 1;
    private static long sequence = 0;
    private static long lastTimeStamp = -1;
    private static object _lock = new object();

    public static long GenerateId()
    {
        lock (_lock)
        {
            var timeSpan = CurrentTimeMillis();
            if (timeSpan==lastTimeStamp)
            {
                sequence = (sequence + 1) & 0xFFF;
                if (sequence==0)
                {
                    // overflow
                    timeSpan = WaitNextTimeMillis();
                    
                }
            }
            else
            {
                sequence = 0;
            }

            lastTimeStamp = timeSpan;
            return ((timeSpan-Epoch) << 22) | (machineId << 12) | sequence;
        }
    }

    public static long CurrentTimeMillis()
    {
        return DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
    }

    public static long WaitNextTimeMillis()
    {
        long timeSpan = CurrentTimeMillis();
        while (timeSpan<=lastTimeStamp)
        {
            timeSpan = CurrentTimeMillis();
        }

        return timeSpan;
    }
}