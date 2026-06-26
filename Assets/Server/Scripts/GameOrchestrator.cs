using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class GameOrchestrator : NetworkBehaviour, CharacterVisitor
{
    public static GameOrchestrator Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        
        Instance = this;
    }
    
    private List<PlayerStats> players = new List<PlayerStats>();
    private List<AllyMobStats> allies;
    private List<EnemyMobStats> enemies;
    [SerializeField] private GameObject skeletonPrefab;  
    [SerializeField] private Rect mapBounds;
    private int aliveEnemies = 0;
    private int alivePlayers = 0;
    
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("GameOrchestrator: Avvio la partita!");
        StartGame();
    }
 
 
    public void addPlayer(PlayerStats playerStats)
    {
        players.Add(playerStats);
        playerStats.OnDie += RemoveCharacter;
        alivePlayers++;
    }

    public void StartGame()
    {
        Debug.Log("Game orchestrator c'è");
        GenerateSkeletons(10);
    }

    private void RemoveCharacter(CharacterStats characterStats)
    {
        characterStats.Accept(this);
        NetworkServer.Destroy(characterStats.gameObject);
    }

    public void VisitPlayer(PlayerStats playerStats)
    {
        players.Remove(playerStats);
        alivePlayers--;
        if (alivePlayers == 0)
            GameOver();
    }

    public void VisitEnemy(EnemyMobStats enemyMobStats)
    {
        if(enemies != null)
        { 
            enemies.Remove(enemyMobStats);
        }
        aliveEnemies--;
        if (aliveEnemies == 0)
            Win();
    }

    public void VisitAlly(AllyMobStats allyMobStats)
    {
        allies.Remove(allyMobStats);
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
            skeletonStats.AttackPeriodicity = 100;
            skeletonStats.OnDie += RemoveCharacter;
            aliveEnemies++;
            
            NetworkServer.Spawn(skeleton);
        }
    }

    private void GameOver()
    {
        Debug.Log("Partita persa, tutti i player sono stati eliminati.");
    }

    private void Win()
    {
        Debug.Log("Partita vinta! Tutti i nemici sono stati eliminati");
    }


}
