using System;
using Network;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameManager: MonoBehaviour
    {
        private async void Start()
        {
            var isConnect = await NetworkManager.Instance.ConnectAsync();
            if (isConnect)
            {
                Debug.Log("Network Connect Success!");
            }
        }

        private void OnDestroy()
        {
            NetworkManager.Instance.DisConnect();
        }
    }
}