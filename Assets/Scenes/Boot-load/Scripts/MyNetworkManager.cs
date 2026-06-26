using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{
    public enum GameState
    {
        Bootstrap,
        Lobby,
        Playing
    }

    public GameState state = GameState.Bootstrap;
    public GameObject lobbyPlayerPrefab;

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<StartPartyMessage>(OnPartyRequest);
        NetworkServer.RegisterHandler<StartGameMessage>(OnGameRequest);
    }

    void OnPartyRequest(NetworkConnectionToClient conn, StartPartyMessage msg)
    {
        if (string.IsNullOrWhiteSpace(msg.playerName))
            return;

        conn.authenticationData = msg.playerName;

        state = GameState.Lobby;
        ServerChangeScene("LobbyScene");
    }

    void OnGameRequest(NetworkConnectionToClient conn, StartGameMessage msg)
    {
        if (string.IsNullOrWhiteSpace(msg.playerName))
            return;

        conn.authenticationData = msg.playerName;

        StartGame();
    }

    public void StartGame()
    {
        if (!NetworkServer.active)
            return;

        state = GameState.Playing;
        ServerChangeScene("GameScene");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        string scene = SceneManager.GetActiveScene().name;

        GameObject prefab;

        if (scene == "LobbyScene")
            prefab = lobbyPlayerPrefab;
        else if (scene == "GameScene")
            prefab = playerPrefab;
        else
            return;

        GameObject player = Instantiate(prefab);

        string name = conn.authenticationData as string;
        if (string.IsNullOrEmpty(name))
            name = "Player";

        LobbyPlayer lobbyPlayer = player.GetComponent<LobbyPlayer>();
        if (lobbyPlayer != null)
            lobbyPlayer.playerName = name;

        player.name = name;

        Transform startPos = GetStartPosition();
        if (startPos != null)
            player.transform.position = startPos.position;

        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        Invoke(nameof(CheckEmpty), 0.2f);
    }

    void CheckEmpty()
    {
        if (NetworkServer.connections.Count == 0)
        {
            state = GameState.Bootstrap;
            ServerChangeScene("Bootstrap-Loading");
        }
    }
}