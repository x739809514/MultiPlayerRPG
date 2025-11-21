namespace RPGServer.Protocol.Packs;

public class UserRequest: Packet
{
    public string UserName { get; set; }
    public string Password { get; set; }

    protected override void WriteBody(BinaryWriter writer)
    {
        BinarySerializer.WriteString(writer,UserName);
        BinarySerializer.WriteString(writer,Password);
    }

    protected override void ReadBody(BinaryReader reader)
    {
        UserName = BinarySerializer.ReadString(reader);
        Password = BinarySerializer.ReadString(reader);
    }
}