using System;
using System.Threading.Tasks;
using Network;
using Network.Protocol.Request;

public class HeartBeat
{
    public void StartHeartBeat()
    {
        Task.Run(StartBeat);
    }

    private async void StartBeat()
    {
        while (ClientSession.IsLogin)
        {
            var heartBeatPack = new HeartBeatRequest();
            heartBeatPack.Uid = ClientSession.Uid;
            heartBeatPack.SessionId = ClientSession.SessionId;
            heartBeatPack.timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            NetworkManager.Instance.SendAsync(heartBeatPack);

            await Task.Delay(5000);
        }
    }
}