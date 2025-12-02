using System.IO;

namespace Network.Protocol.Response
{
    public class UserResponse: Packet
    {
        public ResponseCode Code { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }

        protected override void WriteBody(BinaryWriter writer)
        {
        }

        protected override void ReadBody(BinaryReader reader) { throw new System.NotImplementedException(); }
    }
}