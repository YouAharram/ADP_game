using Mirror;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{
    public GameObject lobbyPlayerPrefab;
    public Transform[] lobbySlots;

    // -------------------------
    // SERVER STATE
    // -------------------------

    private Dictionary<string, Party> parties = new();
    private Dictionary<NetworkConnectionToClient, string> playerParty = new();

    // connection -> assigned slot (FIX IMPORTANTE)
    private Dictionary<NetworkConnectionToClient, int> playerSlot = new();

    // -------------------------
    // START SERVER
    // -------------------------

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<IntentMessage>(OnIntent);
    }

    // -------------------------
    // PARTY
    // -------------------------

    string GenerateCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string code = "";

        for (int i = 0; i < 6; i++)
            code += chars[Random.Range(0, chars.Length)];

        return code;
    }

    void CreateParty(NetworkConnectionToClient conn)
    {
        string code = GenerateCode();

        Party party = new Party(6);
        party.code = code;
        party.leader = conn;

        parties[code] = party;
        playerParty[conn] = code;

        // 👉 questa è la parte importante
        var partyNet = FindAnyObjectByType<PartyNetwork>();
        partyNet.partyCode = code;

        Debug.Log("Party created: " + code);
    }

    void JoinParty(NetworkConnectionToClient conn, string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return;

        if (!parties.TryGetValue(code, out Party party))
            return;

        int slot = party.AddPlayer(conn);

        if (slot == -1)
            return;

        playerParty[conn] = code;
        playerSlot[conn] = slot;

        Debug.Log("Joined party: " + code);
    }

    // -------------------------
    // INTENT
    // -------------------------

    void OnIntent(NetworkConnectionToClient conn, IntentMessage msg)
    {
        // -------------------------
        // NAME HANDLING (optional)
        // -------------------------
        if (!string.IsNullOrWhiteSpace(msg.playerName))
        {
            conn.authenticationData = msg.playerName;
        }

        switch (msg.intent)
        {
            case IntentMessage.IntentType.QuickMatch:
                ServerChangeScene("GameScene");
                break;

            case IntentMessage.IntentType.OpenLobby:
                ServerChangeScene("LobbyScene");
                break;

            case IntentMessage.IntentType.CreateParty:
                CreateParty(conn);
                break;

            case IntentMessage.IntentType.JoinParty:
                JoinParty(conn, msg.partyCode);
                break;
        }
    }

    // -------------------------
    // SPAWN PLAYER
    // -------------------------

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        string scene = SceneManager.GetActiveScene().name;

        if (scene == "Bootstrap-Loading")
            return;

        GameObject prefab =
            scene == "LobbyScene"
                ? lobbyPlayerPrefab
                : playerPrefab;

        GameObject player = Instantiate(prefab);

        string name = conn.authenticationData as string ?? "Player";

        var lp = player.GetComponent<LobbyPlayer>();
        if (lp != null)
            lp.playerName = name;

        // -------------------------
        // LOBBY SPAWN FIX
        // -------------------------
        if (scene == "LobbyScene")
        {
            if (playerSlot.TryGetValue(conn, out int slot) &&
                lobbySlots != null &&
                slot >= 0 &&
                slot < lobbySlots.Length &&
                lobbySlots[slot] != null)
            {
                player.transform.position = lobbySlots[slot].position;
            }
        }

        NetworkServer.AddPlayerForConnection(conn, player);
    }

    // -------------------------
    // DISCONNECT
    // -------------------------

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (playerParty.TryGetValue(conn, out string code))
        {
            if (parties.TryGetValue(code, out Party party))
            {
                party.RemovePlayer(conn);

                bool empty = true;
                foreach (var s in party.slots)
                {
                    if (s != null)
                    {
                        empty = false;
                        break;
                    }
                }

                if (empty)
                    parties.Remove(code);
            }

            playerParty.Remove(conn);
            playerSlot.Remove(conn);
        }

        base.OnServerDisconnect(conn);
    }
}