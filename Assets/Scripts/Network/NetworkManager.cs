using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Network.Protocol.Response;
using UnityEngine;

namespace Network
{
    public class NetworkManager
    {
        private TcpClient tcpClient;
        private string ipAddress = "127.0.0.1";
        private int port = 5500;
        private CancellationTokenSource cancellationTokenSource;
        private NetworkStream stream;
        private bool isConnected;

        private static NetworkManager instance;
        private static readonly object _lock = new object();

        public Action<UserResponse> onRegisterResponse;
        public Action<UserResponse> onLoginResponse;

        public static NetworkManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null) { instance = new NetworkManager(); }

                    return instance;
                }
            }
        }

        public NetworkManager() { tcpClient = new TcpClient(); }


#region Connect & Disconnect

        public async Task<bool> ConnectAsync()
        {
            try
            {
                if (isConnected)
                {
                    Debug.Log("It's already connected");
                    isConnected = true;
                }

                await tcpClient.ConnectAsync(ipAddress, port);
                stream = tcpClient.GetStream();
                isConnected = true;
                cancellationTokenSource = new CancellationTokenSource();
                StartReceive(cancellationTokenSource.Token);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to connect:{e.Message}");
                throw;
            }
        }

        public async void SendAsync(Packet packet, CancellationToken ct = default)
        {
            if (isConnected == false || tcpClient == null) { Debug.LogError("No Network Connect!"); }

            byte[] bytes = packet.ToBytes();
            await stream.WriteAsync(bytes, 0, bytes.Length, ct);
            await stream.FlushAsync(ct);
        }

        public void DisConnect()
        {
            try
            {
                if (isConnected == false) { return; }

                cancellationTokenSource?.Cancel();
                stream?.Close();
                tcpClient.Close();
                isConnected = false;
                Debug.Log($"Server is disconnected now!");
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to disconnect:{e.Message}");
                throw;
            }
        }

#endregion


        private async void StartReceive(CancellationToken ct)
        {
            try
            {
                while (isConnected && ct.IsCancellationRequested == false)
                {
                    // packet length
                    byte[] packetBuffer = new byte[4];
                    var readExactly = await ReadExactly(packetBuffer, 0, 4, ct);
                    if (readExactly == false)
                    {
                        Debug.Log($"Connect closed while reading packet length");
                        break;
                    }

                    int packetLength = BitConverter.ToInt32(packetBuffer, 0);
                    if (packetLength == 0 || packetLength > 4096)
                    {
                        Debug.LogError($"Invalid packet length: {packetLength}");
                        break;
                    }

                    byte[] typeBuffer = new byte[1];
                    if (await ReadExactly(typeBuffer, 0, 1, ct) == false)
                    {
                        Debug.Log("Connect closed while reading packet type");
                        break;
                    }

                    PacketType packetType = (PacketType)typeBuffer[0];
                    int bodyLength = packetLength - 1;
                    byte[] bodyBuffer = new byte[bodyLength];
                    if (bodyLength > 0)
                    {
                        if (await ReadExactly(bodyBuffer, 0, bodyLength, ct) == false)
                        {
                            Debug.Log("Connect closed while reading packet body");
                            break;
                        }
                    }

                    byte[] fullPacket = new byte[packetLength];
                    fullPacket[0] = typeBuffer[0];
                    if (bodyLength > 0) { Array.Copy(bodyBuffer, 0, fullPacket, 1, bodyLength); }

                    HandleReceiveData(packetType, fullPacket);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task<bool> ReadExactly(byte[] buffer, int offset, int count, CancellationToken ct)
        {
            var totalRead = 0;
            while (totalRead < count)
            {
                var byteReadLength = await stream.ReadAsync(buffer, offset, count - totalRead, ct);
                if (byteReadLength == 0) { return false; }

                totalRead += byteReadLength;
            }

            return true;
        }

        private async void HandleReceiveData(PacketType type, byte[] fullPacket)
        {
            try
            {
                switch (type)
                {
                    case PacketType.Register:
                        var registerResponse = PhraseUserResponse(fullPacket);
                        // use this response
                        onRegisterResponse?.Invoke(registerResponse);
                        Debug.Log($"Register Response: {registerResponse.Code}-{registerResponse.Message}");
                        break;
                    case PacketType.Login:
                        var loginResponse = PhraseUserResponse(fullPacket);
                        ClientSession.Set(loginResponse.UserId,loginResponse.SessionId);
                        onLoginResponse?.Invoke(loginResponse);
                        Debug.Log($"Login Response: {loginResponse.Code}-{loginResponse.Message}");
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error handling packet: {e.Message}");
                throw;
            }
        }

        private UserResponse PhraseUserResponse(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                using (var reader = new BinaryReader(ms))
                {
                    reader.ReadByte(); // skip type
                    ResponseCode code = (ResponseCode)reader.ReadByte();
                    string message = BinarySerialization.ReadString(reader);
                    long userId = reader.ReadInt64();
                    string sessionId = BinarySerialization.ReadString(reader);
                    return new UserResponse()
                    {
                        Code = code,
                        Message = message,
                        UserId = userId,
                        SessionId = sessionId
                    };
                }
            }
        }
    }
}