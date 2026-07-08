using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class GameOrchestrator : NetworkBehaviour, CharacterVisitor
{
    private static GameOrchestrator instance;

    public static GameOrchestrator Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError($"[GameOrchestrator] Attenzione! Stai cercando di accedere all'Instance prima che l'oggetto si sia svegliato (Awake) o l'oggetto non è presente nella scena!");
            }
            return instance;
        }
    }

    private void Awake()
    {
        Debug.Log("[GameOrchestrator] Awake chiamato sul NOSTRO GameObject legittimo.");

        // Controllo anti-duplicazione (Pattern Singleton classico)
        if (instance != null && instance != this)
        {
            Debug.LogWarning($"[GameOrchestrator] Rilevato un duplicato nella scena su {gameObject.name}. Lo distruggo.");
            Destroy(gameObject);
            return;
        }

        // Assegnazione ufficiale dell'istanza
        instance = this;
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
 