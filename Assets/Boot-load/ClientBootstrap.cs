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
            NetworkManager.singleton.networkAddress = "127.0.0.1";
            NetworkManager.singleton.StartClient();
        }
    }
}