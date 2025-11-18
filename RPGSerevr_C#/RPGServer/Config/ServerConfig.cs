
/// <summary>
/// server config
/// </summary>
public class ServerConfig
{
    public int MaxSessionCount { get; set; } = 100;
    public int BufferSize { get; set; } = 4096;
    public int Port { get; set; } = 5500;
    public string DataBasePath { get; set; } = "user.db";
}