using UnityEngine;
using Mirror;

public class LobbySceneInitializer : MonoBehaviour
{
    public Transform[] lobbySlots;

    void Awake()
    {
        var nm = (MyNetworkManager)NetworkManager.singleton;
        nm.lobbySlots = lobbySlots;
    }
}