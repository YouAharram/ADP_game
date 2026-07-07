using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic; // <-- AGGIUNTO: Necessario per usare List<T>
using kcp2k;

public class MyNetworkManager : NetworkManager
{
    public GameObject lobbyPlayerPrefab;
    public GameObject gamePlayerPrefab;
    
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
            // --- LOGICA DI CONTROLLO SLOT AGGIORNATA ---
            int slot = LobbySpawnManager.Instance.GetFirstFreeSlot(conn);
            player.transform.position = LobbySpawnManager.Instance.slots[slot].position;
            Debug.Log($"[SERVER] Assegnato slot numero: {slot} a {name}");
        }

        NetworkServer.AddPlayerForConnection(conn, player);

        listPlayers.Add(gamePlayerPrefab);    
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        string scene = SceneManager.GetActiveScene().name;
        
        if (scene == "LobbyScene")
        {
            LobbySpawnManager.Instance.FreeSlot(conn);
            Debug.Log("[SERVER] Client disconnesso. Slot liberato nella Lobby.");
        }

        // Se disconnette, ricordati di rimuoverlo anche dalla tua lista per evitare NullReference!
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

// --- NUOVO: Viene chiamato in automatico da Mirror quando il server ha finito di caricare la nuova scena ---
    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        if (sceneName == "GameScene")
        {
            Debug.Log("[SERVER] GameScene caricata con successo! Inizializzo l'Orchestrator...");

            // ORA la scena è pronta e il GameOrchestrator ha fatto il suo Awake()
            listPlayers.ForEach(player => 
            {
                if (player != null)
                {
                    var entity = player.GetComponent<PlayerEntity>();
                    if (entity != null)
                    {
                        GameOrchestrator.Instance.AddPlayer(entity);
                    }
                    else
                    {
                        Debug.LogWarning($"[SERVER] Il player {player.name} non ha un componente PlayerEntity!");
                    }
                }
            });

            GameOrchestrator.Instance.StartGame();
        }
    }
}