using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("Spawning player");

        Transform startPos = GetStartPosition();

        GameObject player = Instantiate(playerPrefab);

        if (startPos != null)
            player.transform.position = startPos.position;

        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);

        Debug.Log("Client connected → loading GameScene");

        ServerChangeScene("GameScene");
    }

    public override Transform GetStartPosition()
    {
        return base.GetStartPosition();
    }
}