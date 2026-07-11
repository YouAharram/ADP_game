using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using kcp2k;

public class MyNetworkManager : NetworkManager
{
    public GameObject lobbyPlayerPrefab;
    public GameObject gamePlayerPrefab;
    
    // Mantiene traccia dei GameObject dei player spawnati
    private List<GameObject> listPlayers = new List<GameObject>();

    private static ushort cachedPort = 0;
    private static string cachedPartyCode = "";
    private static bool argumentsParsed = false;

    public override void Awake()
    {
        base.Awake();
        if (Application.isBatchMode && !argumentsParsed)
        {
            ParseCommandLineArguments();
        }
    }

    public override void Start()
    {
        base.Start(); 

        if (Application.isBatchMode)
        {
            Debug.Log("[SERVER-BOOT] Configurazione KcpTransport...");
            if (cachedPort > 0)
            {
                var kcp = GetComponent<KcpTransport>();
                if (kcp == null && transport is KcpTransport activeKcp)
                {
                    kcp = activeKcp;
                }

                if (kcp != null)
                {
                    kcp.Port = cachedPort;
                    Debug.Log($"[SERVER-BOOT] Porta KCP impostata su: {kcp.Port}");
                }
            }

            Debug.Log($"[SERVER-BOOT] Avvio server sulla porta {((KcpTransport)transport).Port}...");
            StartServer();
        }
    }

    private void ParseCommandLineArguments()
    {
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].Equals("-port", StringComparison.CurrentCultureIgnoreCase) && i + 1 < args.Length)
            {
                if (ushort.TryParse(args[i + 1], out ushort customPort))
                {
                    cachedPort = customPort;
                }
            }
            if (args[i].Equals("-partyCode", StringComparison.CurrentCultureIgnoreCase) && i + 1 < args.Length)
            {
                cachedPartyCode = args[i + 1];
            }
        }
        argumentsParsed = true;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<AuthMessage>(OnAuth);
        Debug.Log("[SERVER-BOOT] Server KCP pronto a ricevere connessioni.");
    }

    // ================= SINCRO LATO CLIENT =================

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        AuthMessage msg = new AuthMessage { playerName = SceneFlowManager.Instance.playerName };
        NetworkClient.Send(msg);
    }

    // ================= SINCRO LATO SERVER =================

    void OnAuth(NetworkConnectionToClient conn, AuthMessage msg)
    {
        conn.authenticationData = msg.playerName;
        
        if (conn.identity != null)
        {
            LobbyPlayer lp = conn.identity.GetComponent<LobbyPlayer>();
            if (lp != null)
            {
                lp.playerName = msg.playerName;
                Debug.Log($"[SERVER] Nome aggiornato in corsa sul GameObject: {msg.playerName}");
            }
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("ON SERVER ADD PLAYER CALLED");
        string scene = SceneManager.GetActiveScene().name;

        GameObject prefab = scene == "LobbyScene" ? lobbyPlayerPrefab : gamePlayerPrefab;
        GameObject player = Instantiate(prefab);

        string name = conn.authenticationData as string ?? "Player";
        LobbyPlayer lp = player.GetComponent<LobbyPlayer>();
        if (lp != null) lp.playerName = name;

        if (scene == "LobbyScene")
        {
            int slot = LobbySpawnManager.Instance.GetFirstFreeSlot(conn);
            player.transform.position = LobbySpawnManager.Instance.slots[slot].position;
            Debug.Log($"[SERVER] Assegnato slot numero: {slot} a {name}");
        }

        // In GameScene applichiamo le statistiche (MaxHealth incluso) PRIMA dello
        // spawn di rete: così i valori corretti sono già nel messaggio di spawn e
        // la barra vita del player parte piena (stessa logica dei mob).
        PlayerEntity gameEntity = null;
        if (scene == "GameScene")
        {
            gameEntity = player.GetComponent<PlayerEntity>();
            if (gameEntity != null && GameOrchestrator.Instance != null)
            {
                GameOrchestrator.Instance.ApplyInitialPlayerStats(gameEntity);
            }
        }

        NetworkServer.AddPlayerForConnection(conn, player);

        listPlayers.Add(player);    

        // Dopo lo spawn registriamo il player nell'orchestrator (lista, morte, avvio partita).
        if (scene == "GameScene" && gameEntity != null && GameOrchestrator.Instance != null)
        {
            GameOrchestrator.Instance.AddPlayer(gameEntity);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        string scene = SceneManager.GetActiveScene().name;
        
        if (scene == "LobbyScene")
        {
            LobbySpawnManager.Instance.FreeSlot(conn);
            Debug.Log("[SERVER] Client disconnesso. Slot liberato nella Lobby.");
        }

        // Rimozione sicura dalla lista interna
        if (conn.identity != null && listPlayers.Contains(conn.identity.gameObject))
        {
            listPlayers.Remove(conn.identity.gameObject);
        }

        base.OnServerDisconnect(conn);
    }

    // --- CONTROLLO START PARTITA ---
    public void CheckIfAllReady()
    {
        if (!NetworkServer.active) return;
        if (NetworkServer.connections.Count == 0) return;

        bool allReady = true;

        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            if (conn.identity != null)
            {
                LobbyPlayer lp = conn.identity.GetComponent<LobbyPlayer>();
                if (lp != null && !lp.isReady)
                {
                    allReady = false;
                    break;
                }
            }
            else
            {
                allReady = false; 
            }
        }

        if (allReady)
        {
            Debug.Log("[SERVER] Tutti i giocatori sono PRONTI! Avvio della partita in corso...");
            ServerChangeScene("GameScene");
        }
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        if (sceneName == "GameScene")
        {
            Debug.Log("[SERVER] GameScene caricata sul server! Inizializzo l'Orchestrator...");

            if (GameOrchestrator.Instance != null)
            {
                GameOrchestrator.Instance.InitializeOrchestrator();
            }
        }
    }
}