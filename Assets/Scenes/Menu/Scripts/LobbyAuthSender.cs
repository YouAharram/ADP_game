using Mirror;
using UnityEngine;

public class LobbyAuthSender : MonoBehaviour
{
    void Start()
    {
        NetworkClient.OnConnectedEvent += SendAuth;
    }

    void SendAuth()
    {
        NetworkClient.OnConnectedEvent -= SendAuth;

        NetworkClient.Send(new AuthMessage
        {
            playerName = SceneFlowManager.Instance.playerName
        });
    }
}