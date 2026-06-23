using Mirror;
using UnityEngine;

public class ClientBootstrap : MonoBehaviour
{
    void Start()
    {
        if (Application.isBatchMode)
            return;

        if (!NetworkClient.isConnected)
        {
            Debug.Log("START CLIENT");
            NetworkManager.singleton.networkAddress = "192.168.1.3";
            NetworkManager.singleton.StartClient();
        }
    }
}