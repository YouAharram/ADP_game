using UnityEngine;
using Mirror;
using System;

public class ServerBootstrap : MonoBehaviour
{
    // Variabile statica per memorizzare i dati tra la fase di boot iniziale e l'Awake
    private static ushort pendingPort = 0;
    private static string pendingPartyCode = "";
    private static bool isServerBuild = false;

    // 1. FASE PRECOCE: Intercettiamo gli argomenti prima che Mirror faccia qualsiasi cosa
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void BootstrapServerBeforeMirror()
    {
        if (Application.isBatchMode)
        {
            isServerBuild = true;
            Debug.Log("[BOOTSTRAP] Lettura riga di comando in corso...");
            
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower() == "-port" && i + 1 < args.Length)
                {
                    ushort.TryParse(args[i + 1], out pendingPort);
                }
                if (args[i].ToLower() == "-partycode" && i + 1 < args.Length)
                {
                    pendingPartyCode = args[i + 1];
                }
            }
            Debug.Log($"[BOOTSTRAP] Parametri salvati in cache -> Porta: {pendingPort}, Codice: {pendingPartyCode}");
        }
    }

    // 2. FASE DI AVVIO: Il GameObject è pronto, applichiamo la porta e accendiamo il server
    private void Awake()
    {
        // Se siamo in modalità server headless e abbiamo intercettato i parametri
        if (Application.isBatchMode || isServerBuild)
        {
            var telepathy = GetComponent<TelepathyTransport>();
            if (telepathy == null && NetworkManager.singleton != null)
            {
                telepathy = NetworkManager.singleton.GetComponent<TelepathyTransport>();
            }

            // Applichiamo la porta dinamica se è stata trovata
            if (telepathy != null && pendingPort > 0)
            {
                telepathy.port = pendingPort;
                Debug.Log($"[SERVER-BOOT] Porta applicata al trasporto Telepathy: {telepathy.port}");
            }
            else
            {
                Debug.LogError("[SERVER-BOOT] Impossibile trovare TelepathyTransport per applicare la porta!");
            }

            // Forziamo l'avvio autoritativo del server Mirror
            StartUnityServer();
        }
    }

    private void StartUnityServer()
    {
        if (NetworkManager.singleton != null)
        {
            // Recuperiamo la porta attuale per il log di verifica
            ushort activePort = 0;
            if (NetworkManager.singleton.transport is TelepathyTransport telepathy) activePort = telepathy.port;

            Debug.Log($"[SERVER-BOOT] Avvio del server Mirror in parallelo sulla porta: {activePort}...");
            NetworkManager.singleton.StartServer();
        }
        else
        {
            Debug.LogError("[SERVER-BOOT] ERRORE CRITICO: NetworkManager.singleton è NULL nell'Awake!");
        }
    }
}