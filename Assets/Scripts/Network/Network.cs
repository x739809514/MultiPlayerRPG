using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// 接收服务端的消息
/// </summary>
public class Network : MonoBehaviour
{
    public static Network Instance;

    [Header("Network Settings")] public string serverIp = "127.0.0.1";
    public int serverPort = 5500;
    public bool isConnected;

    public TcpClient playerSocket;
    public NetworkStream myStream;
    public StreamReader myReader;
    public StreamWriter myWriter;

    private byte[] asyncBuff;
    public bool shouldHandleData;

    private void Awake() { Instance = this; }

    private void Start()
    {
        ConnectGameServer();
    }

    private void ConnectGameServer()
    {
        if (playerSocket != null)
        {
            if (playerSocket.Connected || isConnected)
            {
                return;
            }
            playerSocket.Close();
            playerSocket = null;
        }

        playerSocket = new TcpClient();
        playerSocket.ReceiveBufferSize = 4096;
        playerSocket.SendBufferSize = 4096;
        playerSocket.NoDelay = false;
        
        Array.Resize(ref asyncBuff,8192);
        playerSocket.BeginConnect(serverIp, serverPort, new AsyncCallback(ConnectCallback), playerSocket);
    }

    private void ConnectCallback(IAsyncResult result)
    {
        if (playerSocket!=null)
        {
            playerSocket.EndConnect(result);
            if (playerSocket.Connected==false)
            {
                isConnected = false;
                return;
            }
            else
            {
                playerSocket.NoDelay = true;
                myStream = playerSocket.GetStream();
                myStream.BeginRead(asyncBuff, 0, 8192, OnReceive, null);
            }
        }
    }

    private void OnReceive(IAsyncResult tResult)
    {
        if (playerSocket != null)
        {
            if (playerSocket ==null)
            {
                return;
            }
            
            int byteArray = myStream.EndRead(tResult);
            byte[] myBytes = null;
            Array.Resize(ref myBytes, byteArray);
            Buffer.BlockCopy(asyncBuff,0,myBytes,0,byteArray);

            if (byteArray==0)
            {
                Debug.Log("You got disconnect from the server");
                playerSocket.Close();
                return;
            }
            
            // Handle Data
            if (playerSocket==null)
            {
                return;
            }

            myStream.BeginRead(asyncBuff, 0, 8192, OnReceive, null);
        }
    }
}