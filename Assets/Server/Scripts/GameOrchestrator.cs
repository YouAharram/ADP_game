using UnityEngine;
using Mirror;
using System.Collections.Generic;
public class GameOrchestrator : NetworkBehaviour
{
    private List<PlayerStats> playerStats;
    private List<AllyMobStats> allyListStats;
    private List<EnemyMobStats> enemyMobStats;

    void Start()
    {
        
        // istanzia playerList con giocatori mob buoni e cattivi
        // setta le statistiche di ogni entita
        // avvia il ride
    }

    void Update()
    {
        // controlla lo stato di tutti i character
        // e in base al loro tipo concreto controlla 
        // e esegue certe azioni
    }
}
