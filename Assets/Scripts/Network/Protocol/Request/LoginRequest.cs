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
            BinarySerialization.WriteString(writer,username);
            BinarySerialization.WriteString(writer,password);
        }

        protected override void ReadBody(BinaryReader reader)
        {
            username = BinarySerialization.ReadString(reader);
            password = BinarySerialization.ReadString(reader);
        }
    }
}