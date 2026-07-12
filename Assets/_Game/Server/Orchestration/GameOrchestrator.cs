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
    
    public Rect MapBounds { get => mapBounds; }
    public List<GameObject> EnemyPrefabs { get => enemyPrefabs; }

    public void InitializeOrchestrator()
    {        
        Debug.Log("GameOrchestrator: Inizializzazione manager e statistiche...");
        levelManager = GetComponent<LevelManager>();
        levelManager.EnemyExtractor = new EnemyPrefabExpRarityExtractor();
        playerBaseStats = GetComponent<PlayerBaseStats>();
        
        // Pulizia liste per sicurezza
        players.Clear();
        enemies.Clear();
        aliveEnemies = 0;
        alivePlayers = 0;
        
        // TODO: migliorare questa parte con BIO
        BuildingEntity castleEntity = castle.GetComponent<BuildingEntity>();
        castleEntity.MaxHealth = 10000;
    }

    private void StartLevel()
    {
        Debug.Log($"[GameOrchestrator] Inizio Livello {levelManager.Level}. Generazione nemici...");
        levelManager.GenerateEnemies(this);
    }
 
    public void ApplyInitialPlayerStats(PlayerEntity playerStats)
    {
        if (levelManager != null && playerBaseStats != null)
        {
            levelManager.SetPlayerStatistics(playerStats, playerBaseStats);
            Debug.Log($"[GameOrchestrator] Statistiche applicate a {playerStats.name} (pre-spawn)");
        }
        else
        {
            Debug.LogWarning("[GameOrchestrator] ApplyInitialPlayerStats: manager non ancora inizializzati.");
        }
    }

    public void AddPlayer(PlayerEntity playerStats)
    {
        players.Add(playerStats);
        playerStats.OnDieServer += RemoveCharacter;
        alivePlayers++;

        if (players.Count == NetworkServer.connections.Count)
        {
            Debug.Log("[GameOrchestrator] Tutti i giocatori sono spawnati. Inizio la partita!");
            StartLevel();
        }
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
        
        if (alivePlayers <= 0)
            GameOver();
    }

    public void VisitEnemy(EnemyMobEntity enemyMobStats)
    {
        enemies.Remove(enemyMobStats);
        aliveEnemies--;
        
        if (aliveEnemies <= 0)
            Win();
    }

    public void VisitBuilding(BuildingEntity allyMobEntity)
    {
        // Se il castello viene distrutto, si perde la partita!
        GameOver(); 
    }
    
    public void GenerateEnemy(GameObject enemyPrefab)
    {
		enemyPrefab.GetComponent<EnemyMobEntity>().SetFacingDirection(false); 	

        GameObject enemy = Instantiate(
            enemyPrefab, 
            EnemyPositionSelector.RandomPosition(mapBounds), 
            Quaternion.identity);
        
        // Impostiamo l'intelligenza artificiale verso il castello
        enemy.GetComponent<MobAI>().TargetPosition = castle.GetComponent<Entity>().GetPosition();
        
        // Impostiamo le statistiche base
        EnemyMobEntity enemyStats = enemy.GetComponent<EnemyMobEntity>();

        levelManager.SetEnemyStatistics(enemyStats, enemy.GetComponent<EnemyPrefabBaseStats>());
            
        NetworkServer.Spawn(enemy);
        AddEnemy(enemyStats);
    }

    private void GameOver()
    {
        Debug.Log("Partita persa. Disconnessione generale in corso.");
        NetworkServer.DisconnectAll();
    }

    private void Win()
    {
        Debug.Log($"Livello {levelManager.Level} vinto! Tutti i nemici sono stati eliminati. Passaggio al livello successivo...");
        
        levelManager.LevelUp();
        
        // Riapplichiamo le stat per eventuali Level Up del giocatore
        players.ForEach(player => levelManager.SetPlayerStatistics(player, playerBaseStats));
        
        StartLevel();
    }
}