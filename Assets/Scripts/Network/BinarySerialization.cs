using System.IO;
using System.Text;

namespace Network
{
    public static class BinarySerialization
    {
        public static void WriteString(BinaryWriter writer,string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                writer.Write((ushort)0);
                return;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(value);
            writer.Write((ushort)bytes.Length);
            writer.Write(bytes);
        }

        public static string ReadString(BinaryReader reader)
        {
            ushort length = reader.ReadUInt16();
            if (length==0)
            {
                return string.Empty;
            }

            byte[] bytes = reader.ReadBytes(length);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}