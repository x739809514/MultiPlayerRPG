namespace RPGServer.Protocol.Packs.Response;

public class UserResponse:Packet
{
    public ResponseCode Code { get; set; }
    public string Message { get; set; }
    public int UserId { get; set; }

    protected override void WriteBody(BinaryWriter writer)
    {
        writer.Write((byte)Code);
        BinarySerializer.WriteString(writer,Message);
        writer.Write(UserId);
    }

    protected override void ReadBody(BinaryReader reader)
    {
        Code = (ResponseCode)reader.ReadByte();
        Message = BinarySerializer.ReadString(reader);
        UserId = reader.ReadInt32();
    }
}