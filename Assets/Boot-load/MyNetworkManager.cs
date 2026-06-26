using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            Debug.LogError("Errore: GameOrchestrator non trovato! L'hai messo nella scena?");
        }
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        
        if (SceneManager.GetActiveScene().name != "GameScene")
        {
            Debug.Log("Client connected → loading GameScene");
            ServerChangeScene("GameScene");
        }
    }
}