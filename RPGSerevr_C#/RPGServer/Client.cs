using System.Net.Sockets;

namespace RPGServer;

public class Client
{
    public int index;
    public string ip;
    public TcpClient socket;
    public NetworkStream myStream;
    private byte[] readBuff;

    public void Start()
    {
        socket.SendBufferSize = 4096;
        socket.ReceiveBufferSize = 4096;
        myStream = socket.GetStream();
        Array.Resize(ref readBuff,socket.ReceiveBufferSize);
        myStream.BeginRead(readBuff, 0, socket.ReceiveBufferSize, OnReceiveData, null);
    }

    private void OnReceiveData(IAsyncResult tResult)
    {
        try
        {
            int readBytes = myStream.EndRead(tResult);
            if (socket==null)
            {
                return;
            }

            if (readBytes<=0)
            {
                CloseConnection();
                return;
            }

            byte[] newBytes = null;
            Array.Resize(ref newBytes, readBytes);
            Buffer.BlockCopy(readBuff,0,newBytes,0,readBytes);
            
            // HandleData
            if (socket == null)
            {
                return;
            }

            myStream.BeginRead(readBuff, 0, socket.ReceiveBufferSize, OnReceiveData, null);
            
        }
        catch (Exception e)
        {
            CloseConnection();
            throw;
        }
    }

    private void CloseConnection()
    {
        socket.Close();
        socket = null;
        Console.WriteLine("Player Disconnect:"+ip);
    }
}