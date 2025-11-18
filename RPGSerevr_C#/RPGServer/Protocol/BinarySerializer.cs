using System.Text;

namespace RPGServer.Protocol;

public static class BinarySerializer
{
    public static void WriteString(BinaryWriter writer, string value)
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

    public static void WriteDateTime(BinaryWriter writer, DateTime dateTime)
    {
        writer.Write(dateTime.ToBinary());
    }

    public static void ReadDateTime(BinaryReader reader)
    {
        long binary = reader.ReadInt64();
    }
}