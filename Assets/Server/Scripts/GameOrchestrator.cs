using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class GameOrchestrator : NetworkBehaviour
{
    private List<PlayerStats> playerList = new List<PlayerStats>();
    private List<AllyMobStats> allyList;
    private List<EnemyMobStats> enemyList;
    private GameObject skeletonPrefab;  
    [SerializeField] private Rect mapBounds;
 
 
    public void addPlayer(PlayerStats playerStats)
    {
        playerList.Add(playerStats);
    }

    public void StartGame()
    {
        GenerateSkeletons(10);
    }

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
        bool victory = true;

        foreach (EnemyMobStats enemy in enemyList)
        {
            if (enemy.CurrentHealth > 0)
            {
                victory = false;
            }
        }

    }

    private void GenerateSkeletons(int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            float xPosition = Random.Range(mapBounds.min.x, mapBounds.max.x);
            float yPosition = Random.Range(mapBounds.min.y, mapBounds.max.y);
            GameObject skeleton = Instantiate(skeletonPrefab, new Vector2(xPosition, yPosition), Quaternion.identity);
            EnemyMobStats skeletonStats = skeleton.GetComponent<EnemyMobStats>();
            skeletonStats.Damage = 10;
            skeletonStats.Speed = 2;
            skeletonStats.MaxHealth = 50;
        }
    }
}
