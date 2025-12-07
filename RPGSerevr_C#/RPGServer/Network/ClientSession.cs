using System.Net.Sockets;
using System.Text;
using RPGServer.Protocol;
using RPGServer.Protocol.Packs;
using RPGServer.Protocol.Packs.Response;
using RPGServer.Service.Interface;
using RPGServer.Session;

namespace RPGServer.Network;

public class ClientSession
{
    private readonly TcpClient tcpClient;
    private readonly IUserService userService;
    private readonly NetworkStream stream;
    private readonly byte[] buffer;
    private bool isRunning;

    public string sessionId { get; }
    public long UserId { get; private set; }

    public ClientSession(TcpClient client, IUserService service, int bufferSize)
    {
        tcpClient = client;
        stream = tcpClient.GetStream();
        userService = service;
        buffer = new byte[bufferSize];
        sessionId = Guid.NewGuid().ToString();
        isRunning = true;
    }

    public void Start()
    {
        Console.WriteLine($"$Session {sessionId} Start!");

        try
        {
            while (isRunning && tcpClient.Connected)
            {
                // 先length
                byte[] lengthBuffer = new byte[4];
                if (ReadExactly(lengthBuffer,0,4)==false)
                {
                    Console.WriteLine($"Session {sessionId} disconnected while reading length");
                    break;
                }
                // authorize packet length is valid
                int packetLength = BitConverter.ToInt32(lengthBuffer, 0);
                if (packetLength<=0 || packetLength>buffer.Length)
                {
                    Console.WriteLine($"Session {sessionId} invalid Packet length: {packetLength}");
                    break;
                }
                // read packet type
                byte[] typeBuffer = new byte[1];
                if (ReadExactly(typeBuffer,0,1)==false)
                {
                    Console.WriteLine($"Session {sessionId} disconnected while reading type");
                    break;
                }

                // read body
                PacketType packetType = (PacketType)typeBuffer[0];
                int bodyLength = packetLength - 1;
                byte[] bodyBuffer = new byte[bodyLength];
                if (bodyLength>0)
                {
                    if (ReadExactly(bodyBuffer,0,bodyLength)==false)
                    {
                        Console.WriteLine($"Session {sessionId} disconnected while reading body");
                        break;
                    }
                }
                
                // re-organize entire packet
                byte[] fullPacket = new byte[packetLength];
                fullPacket[0] = typeBuffer[0];
                if (bodyLength>0)
                {
                    Array.Copy(bodyBuffer,0,fullPacket,1,bodyLength);
                }
                
                HandlePacket(packetType,fullPacket);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Session {sessionId} error: {e.Message}");
            Console.ForegroundColor = ConsoleColor.Red;
        }
        finally
        {
            Close();
        }
    }

    public bool IsConnected()
    {
        return isRunning && tcpClient.Connected;
    }

    private void Close()
    {
        if (!isRunning) return;

        isRunning = false;
        stream?.Close();
        tcpClient?.Close();
        Console.WriteLine($"Session {sessionId} closed");
    }


    private void HandlePacket(PacketType type, byte[] data)
    {
        try
        {
            switch (type)
            {
                case PacketType.Register:
                    HandleRegister(data);
                    break;
                case PacketType.Login:
                    HandleLogin(data);
                    break;
                case PacketType.HeartBeat:
                    HandleHeartBeat(data);
                    break;
                default:
                    Console.WriteLine($"Session {sessionId} unknown packet type: {type}");
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Session {sessionId} handle packet error: {e.Message}");
        }
    }

    private void HandleRegister(byte[] data)
    {
        try
        {
            // 反序列化请求包
            var request = Packet.FromBytes<Protocol.Packs.UserRequest>(data);
            Console.WriteLine($"Session {sessionId} register request: {request.UserName}");

            // 调用服务处理注册
            var response = userService.Register(request.UserName, request.Password);
            response.PacketType = PacketType.Register;

            // 发送响应
            SendPacket(response);

            Console.WriteLine($"Session {sessionId} register response: {response.Code}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Session {sessionId} register error: {e.Message}");
            SendErrorResponse(PacketType.Register, "注册处理失败");
        }
    }

    private void HandleLogin(byte[] data)
    {
        try
        {
            // 反序列化请求包
            var request = Packet.FromBytes<Protocol.Packs.UserRequest>(data);
            Console.WriteLine($"Session {sessionId} login request: {request.UserName}");

            // 调用服务处理登录
            var response = userService.Login(request.UserName, request.Password);
            response.PacketType = PacketType.Login;

            // 登录成功时保存用户ID
            if (response.Code == Protocol.ResponseCode.Success)
            {
                UserId = response.UserId;
                var sessionId = Guid.NewGuid().ToString("N");
                SessionManager.CreateSession(UserId,sessionId,tcpClient);
                response.SessionId = sessionId;
                Console.WriteLine($"Session {sessionId} user {UserId} logged in");
            }

            // 发送响应
            SendPacket(response);

            Console.WriteLine($"Session {sessionId} login response: {response.Code}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Session {sessionId} login error: {e.Message}");
            SendErrorResponse(PacketType.Login, "登录处理失败");
        }
    }

    private void HandleHeartBeat(byte[] data)
    {
        var request = Packet.FromBytes<HeartBeatRequest>(data);
        var isOk = userService.Validation(request.Uid, request.SessionId);
        if (isOk)
        {
            Console.WriteLine($"Session {sessionId} 's heart is beating ...");
        }
        else
        {
            Console.WriteLine($"Session {sessionId} 's SessionId or Uid is not valid");
            userService.CloseSession(request.Uid);
        }
    }

    private void SendPacket(Packet packet)
    {
        try
        {
            byte[] data = packet.ToBytes();
            stream.Write(data, 0, data.Length);
            stream.Flush();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Session {sessionId} send packet error: {e.Message}");
        }
    }

    private void SendErrorResponse(PacketType type, string message)
    {
        var response = new UserResponse
        {
            PacketType = type,
            Code = Protocol.ResponseCode.ServerError,
            Message = message,
            UserId = 0
        };
        SendPacket(response);
    }

    // read byte from stream, avoid 粘包
    private bool ReadExactly(byte[] buffer, int offset, int count)
    {
        int totalRead = 0;
        while (totalRead < count)
        {
            int byteRead = stream.Read(buffer, offset + totalRead, count - totalRead);
            if (byteRead==0)
            {
                return false;
            }

            totalRead += byteRead;
        }

        return true;
    }
    
}