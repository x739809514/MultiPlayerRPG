using System.IO;

namespace Network.Protocol.Response
{
    public class UserResponse: Packet
    {
        public ResponseCode Code { get; set; }
        public string Message { get; set; }
        public long UserId { get; set; }
        public string SessionId { get; set; }

        protected override void WriteBody(BinaryWriter writer)
        {
            writer.Write((byte)Code);
            BinarySerialization.WriteString(writer,Message);
            writer.Write(UserId);
            BinarySerialization.WriteString(writer,SessionId);
        }

        protected override void ReadBody(BinaryReader reader)
        {
            Code = (ResponseCode)reader.ReadByte();
            Message = BinarySerialization.ReadString(reader);
            UserId = reader.ReadInt64();
            SessionId = BinarySerialization.ReadString(reader);
        }
    }
}