using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] private GameOrchestrator gameOrchestrator;
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("Spawning player");

        Transform startPos = GetStartPosition();

        GameObject player = Instantiate(playerPrefab);

        if (startPos != null)
            player.transform.position = startPos.position;

        NetworkServer.AddPlayerForConnection(conn, player);

        gameOrchestrator = GetComponent<GameOrchestrator>();

        gameOrchestrator.addPlayer(player.GetComponent<PlayerStats>());

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
            if (gameOrchestrator == null)
            {
                gameOrchestrator = GetComponent<GameOrchestrator>();
            }
            gameOrchestrator.StartGame();
        }
    }

    public override Transform GetStartPosition()
    {
        return base.GetStartPosition();
    }
}