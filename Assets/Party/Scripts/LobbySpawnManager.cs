using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class LobbySpawnManager : MonoBehaviour
{
    public static LobbySpawnManager Instance;

    // Trascina qui dentro i tuoi Transform dei punti di spawn nell'Inspector di Unity
    public Transform[] slots; 

    // Questo tiene traccia di quale connessione occupa quale indice di slot
    private Dictionary<NetworkConnectionToClient, int> occupiedSlots = new Dictionary<NetworkConnectionToClient, int>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Trova il primo slot numerico libero, lo assegna alla connessione e lo blocca
    public int GetFirstFreeSlot(NetworkConnectionToClient conn)
    {
        // Se per qualche motivo questo client ha già uno slot registrato, restituiamo quello
        if (occupiedSlots.ContainsKey(conn))
        {
            return occupiedSlots[conn];
        }

        // Cerchiamo ciclicamente il primo indice non occupato da nessuno
        for (int i = 0; i < slots.Length; i++)
        {
            if (!occupiedSlots.ContainsValue(i))
            {
                occupiedSlots[conn] = i; // Blocchiamo lo slot per questa connessione
                return i;
            }
        }

        // Fallback di sicurezza nel caso la lobby sia strapiena
        return 0;
    }

    // Libera lo slot quando il giocatore si disconnette
    public void FreeSlot(NetworkConnectionToClient conn)
    {
        if (occupiedSlots.ContainsKey(conn))
        {
            Debug.Log($"[SPAWN-MANAGER] Liberato lo slot {occupiedSlots[conn]}");
            occupiedSlots.Remove(conn);
        }
    }
}