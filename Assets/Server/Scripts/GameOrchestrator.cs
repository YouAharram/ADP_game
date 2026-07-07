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
    
    private List<PlayerEntity> players = new List<PlayerEntity>();
    private List<EnemyMobEntity> enemies = new List<EnemyMobEntity>();

    [SerializeField] private GameObject castle;
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private Rect mapBounds;
    private LevelManager levelManager;

    private PlayerBaseStats playerBaseStats;
    private int aliveEnemies = 0;
    private int alivePlayers = 0;
    
    public Rect MapBounds { get => mapBounds;}
    public List<GameObject> EnemyPrefabs { get => enemyPrefabs;}

    public void StartGame()
    {        
        Debug.Log("GameOrchestrator: Avvio la partita!");
        levelManager = GetComponent<LevelManager>();
        levelManager.EnemyExtractor = new EnemyPrefabExpRarityExtractor();
        playerBaseStats = GetComponent<PlayerBaseStats>();
        StartLevel();
    }

    private void StartLevel()
    {
        players.ForEach(player => levelManager.SetPlayerStatistics(player, playerBaseStats));
        levelManager.GenerateEnemies(this);
    }
 
 
    public void AddPlayer(PlayerEntity playerStats)
    {
        players.Add(playerStats);
        playerStats.OnDieServer += RemoveCharacter;
        alivePlayers++;
    }

    private void AddEnemy(EnemyMobEntity enemyMobStats)
    {
        enemies.Add(enemyMobStats);
        enemyMobStats.OnDieServer += RemoveCharacter;
        aliveEnemies++;
    }


    private void RemoveCharacter(Entity characterStats)
    {
        characterStats.Accept(this);
        NetworkServer.Destroy(characterStats.gameObject);
    }

    public void VisitPlayer(PlayerEntity playerStats)
    {
        players.Remove(playerStats);
        alivePlayers--;
        if (alivePlayers == 0)
            GameOver();
    }

    public void VisitEnemy(EnemyMobEntity enemyMobStats)
    {
        enemies.Remove(enemyMobStats);
        aliveEnemies--;
        if (aliveEnemies == 0)
            Win();
    }

    public void VisitBuilding(BuildingEntity allyMobEntity)
    {
        throw new System.NotImplementedException();
    }
    
    public void GenerateEnemy(GameObject enemyPrefab)
    {
        GameObject enemy = Instantiate(
            enemyPrefab, 
            EnemyPositionSelector.RandomPosition(mapBounds), 
            Quaternion.identity);
        
        enemy.GetComponent<MobAI>().TargetPosition = castle.GetComponent<Entity>().GetPosition();
        EnemyMobEntity enemyStats = enemy.GetComponent<EnemyMobEntity>();
        levelManager.SetEnemyStatistics(enemyStats, enemy.GetComponent<EnemyPrefabBaseStats>());
            
        NetworkServer.Spawn(enemy);
        AddEnemy(enemyStats);
    }


    private void GameOver()
    {
        Debug.Log("Partita persa, tutti i player sono stati eliminati.");
        NetworkServer.DisconnectAll();
    }

    private void Win()
    {
        Debug.Log("Livello" + levelManager.Level + " vinto! Tutti i nemici sono stati eliminati. Passaggio al livello successivo...");
        
        levelManager.LevelUp();
        StartLevel();
    }


}
 