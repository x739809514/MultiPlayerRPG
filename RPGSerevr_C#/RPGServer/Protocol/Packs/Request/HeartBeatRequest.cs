namespace RPGServer.Protocol.Packs;

public class HeartBeatRequest: Packet
{
    public long Uid { get; set; }
    public string SessionId { get; set; }
    public long timeStamp { get; set; }
    
    protected override void WriteBody(BinaryWriter writer)
    {
        writer.Write(Uid);
        BinarySerializer.WriteString(writer,SessionId);
        writer.Write(timeStamp);
    }

    protected override void ReadBody(BinaryReader reader)
    {
        Uid = reader.ReadInt64();
        SessionId = BinarySerializer.ReadString(reader);
        timeStamp = reader.ReadInt64();
    }
}