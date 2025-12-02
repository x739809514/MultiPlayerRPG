using System.IO;

namespace Network.Protocol.Request
{
    public class LoginRequest: Packet
    {
        public string username;
        public string password;

        public LoginRequest()
        {
            PacketType = PacketType.Login;
        }
        protected override void WriteBody(BinaryWriter writer)
        {
            writer.Write((byte)PacketType);
            BinarySerialization.WriteString(writer,username);
            BinarySerialization.WriteString(writer,password);
        }

        protected override void ReadBody(BinaryReader reader)
        {
            PacketType = (PacketType)reader.ReadByte();
            username = BinarySerialization.ReadString(reader);
            password = BinarySerialization.ReadString(reader);
        }
    }
}