using System;
using System.IO;

namespace Network.Protocol.Request
{
    public class HeartBeatRequest: Packet
    {
        public long Uid { get; set; }
        public string SessionId { get; set; }
        public long timeStamp { get; set; }

        public HeartBeatRequest()
        {
            PacketType = PacketType.HeartBeat;
        }

        protected override void WriteBody(BinaryWriter writer)
        {
            writer.Write(Uid);
            BinarySerialization.WriteString(writer,SessionId);
            writer.Write(timeStamp);
        }

        protected override void ReadBody(BinaryReader reader)
        {
            Uid = reader.ReadInt64();
            SessionId = BinarySerialization.ReadString(reader);
            timeStamp = reader.ReadInt64();
        }
    }
}