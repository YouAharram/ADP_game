using Mirror;
using UnityEngine;

public class ServerBootstrap : MonoBehaviour
{
    void Start()
    {
        if (!Application.isBatchMode)
            return;

        if (!NetworkServer.active)
        {
            Debug.Log("START SERVER");
            NetworkManager.singleton.StartServer();
        }
    }
}