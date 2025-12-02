using System.IO;

namespace Network.Protocol.Request
{
    public class RegisterRequest: Packet
    {
        public string userName;
        public string password;

        public RegisterRequest()
        {
            PacketType = PacketType.Register;
        }

        protected override void WriteBody(BinaryWriter writer)
        {
            BinarySerialization.WriteString(writer,userName);
            BinarySerialization.WriteString(writer,password);
        }

        protected override void ReadBody(BinaryReader reader)
        {
            userName = BinarySerialization.ReadString(reader);
            password = BinarySerialization.ReadString(reader);
        }
    }
}