using UnityEngine;
using Mirror;

public class GameBootstrap : MonoBehaviour
{
    void Start()
    {
        var flow = SceneFlowManager.Instance;

        NetworkManager.singleton.networkAddress = flow.pendingIp;

        var transport = NetworkManager.singleton.GetComponent<TelepathyTransport>();
        transport.port = (ushort)flow.pendingPort;

        NetworkManager.singleton.StartClient();
    }
}