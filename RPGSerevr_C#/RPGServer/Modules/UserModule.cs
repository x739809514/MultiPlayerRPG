namespace RPGServer.Modules;

public class UserModule
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string PwdHash { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime LastLogin { get; set; }
}