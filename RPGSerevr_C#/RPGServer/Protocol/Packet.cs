using Microsoft.VisualBasic;

namespace RPGServer.Protocol;

// {"Type":"Login","Username":"player","Password":"123456"}
// ```
//
// **二进制格式：** ~25字节
//     ```
// [长度:4字节][类型:1字节][用户名长度:2字节][用户名数据][密码长度:2字节][密码数据]
// ```
//
// ## 📦 **协议结构设计**
// ```
// 完整数据包结构：
//     ┌─────────────┬──────────┬─────────────┐
//     │ 包长度(4B)  │ 类型(1B) │   包体数据   │
//     └─────────────┴──────────┴─────────────┘
//
// 请求包体结构：
//     ┌────────────┬────────┬────────────┬────────┐
//     │ 用户名长度 │ 用户名 │ 密码长度   │  密码  │
//     │   (2B)     │ (变长) │   (2B)     │ (变长) │
//     └────────────┴────────┴────────────┴────────┘
//
// 响应包体结构：
//     ┌─────────┬────────────┬────────┬─────────┐
//     │ 响应码  │ 消息长度   │  消息  │ 用户ID  │
//     │  (1B)   │   (2B)     │ (变长) │  (4B)   │
//     └─────────┴────────────┴────────┴─────────┘
public abstract class Packet
{
    public const int HEADER_SIZE = 5;
    public PacketType packetType { get; set; }

    public byte[] ToBytes()
    {
        using var ms = new MemoryStream();
        using (var writer = new BinaryWriter(ms))
        {
            // write type here firstly
            writer.Write((byte)packetType);
            WriteBody(writer);

            byte[] body = ms.ToArray();
            
            //create complete packet
            using (var finalMs = new MemoryStream())
            {
                using (var finalWriter = new BinaryWriter(finalMs))
                {
                    finalWriter.Write(body.Length);
                    finalWriter.Write(body);
                    return finalMs.ToArray();
                }
            }
        }
    }

    protected abstract void WriteBody(BinaryWriter writer);

    /// <summary>
    /// 反序列化纯包体
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T FromBytes<T>(byte[] data) where T : Packet, new()
    {
        using (var ms = new MemoryStream(data))
        {
            using (var reader = new BinaryReader(ms))
            {
                var packet = new T();
                packet.packetType = (PacketType)reader.ReadByte();
                packet.ReadBody(reader);
                return packet;
            }
        }
    }

    protected abstract void ReadBody(BinaryReader reader);
}