namespace RPGServer.Modules;

public class UserModule
{
    public int Id { get; set; }
    public int UserName { get; set; }
    public int Password { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime LastLogin { get; set; }
}