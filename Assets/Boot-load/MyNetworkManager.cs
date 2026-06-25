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

        if (GameOrchestrator.Instance != null)
        {
            GameOrchestrator.Instance.addPlayer(player.GetComponent<PlayerStats>());
        }
        else
        {
            Debug.LogError("GameOrchestrator.Instance è null");
        }

    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);

        Debug.Log("Client connected → loading GameScene");

        ServerChangeScene("GameScene");
    }

    public override void OnServerChangeScene(string sceneName)
    {
        base.OnServerChangeScene(sceneName);
        
        if (sceneName == "GameScene")
        {
            if (GameOrchestrator.Instance != null)
            {
                GameOrchestrator.Instance.StartGame();
            }
        }
    }

    public override Transform GetStartPosition()
    {
        return base.GetStartPosition();
    }
}