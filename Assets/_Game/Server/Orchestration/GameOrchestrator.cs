using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;

public class GameOrchestrator : NetworkBehaviour
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

        if (instance != null && instance != this)
        {
            Debug.LogWarning($"[GameOrchestrator] Rilevato un duplicato nella scena su {gameObject.name}. Lo distruggo.");
            Destroy(gameObject);
            return;
        }

        instance = this;
        
        if (levelManager == null)
        {
            levelManager = GetComponent<LevelManager>();
        }

    }
    
    private List<PlayerEntity> players = new List<PlayerEntity>();
    private List<EnemyMobEntity> enemies = new List<EnemyMobEntity>();
    private BuildingEntity castleEntity;

    [SerializeField] private GameObject castle;
    [SerializeField] private int castleBaseHealth;

	[SerializeField] private GameObject archer;
	[SerializeField] private int archerDamage;

    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private UpgradeUIManager upgradeUIManager;
    [SerializeField] private EndGameUIManager endGameUIManager;


    private LevelManager levelManager;

    private PlayerBaseStats playerBaseStats;
    private int aliveEnemies = 0;
    private int alivePlayers = 0;
    private int readyPlayersForNextLevel = 0;

    public List<GameObject> EnemyPrefabs => enemyPrefabs;
    public LevelManager LevelManager  => levelManager;

    public void InitializeOrchestrator()
    {        
        Debug.Log("GameOrchestrator: Avvio la partita!");
        levelManager = GetComponent<LevelManager>();
        levelManager.EnemyExtractor = new EnemyPrefabExpRarityExtractor();
        playerBaseStats = GetComponent<PlayerBaseStats>();

        castleEntity = castle.GetComponent<BuildingEntity>();
        castleEntity.OnDieServer += RemoveEntity;

		archer.GetComponent<CharacterEntity>().Damage = archerDamage;
        
        players.Clear();
        enemies.Clear();
        aliveEnemies = 0;
        alivePlayers = 0;

        StartLevel();
    }

    private void StartLevel()
    {
        levelManager.SetCastleHealth(castleEntity, castleBaseHealth);
        levelManager.GenerateEnemies();
    }
 
 
    public void AddPlayer(PlayerEntity playerEntity)
    {
        players.Add(playerEntity);
        playerEntity.OnDieServer += RemoveEntity;
        alivePlayers++;

        if (levelManager != null && playerBaseStats != null)
        {
            levelManager.SetPlayerStatistics(playerEntity, playerBaseStats);
            Debug.Log($"[GameOrchestrator] Statistiche applicate a {playerEntity.name}");
        }
        
        if (players.Count == NetworkServer.connections.Count)
        {
            Debug.Log("[GameOrchestrator] Tutti i giocatori sono spawnati. Inizio la partita!");
            StartLevel();
        }
    }

    private void AddEnemy(EnemyMobEntity enemyMobEntity)
    {
        enemies.Add(enemyMobEntity);
        enemyMobEntity.OnDieServer += RemoveEntity;
        aliveEnemies++;
    }


    private void RemoveEntity(Entity entity)
    {
        entity.Accept(new RemoveEntityVisitor(this));
        NetworkServer.Destroy(entity.gameObject);
    }

    
    public void GenerateEnemy(GameObject enemyPrefab)
    {
        GameObject enemy = Instantiate(
            enemyPrefab, 
            EnemySpawnerStrategy.EastRandomPosition(), 
            Quaternion.identity);
        
        enemy.GetComponent<MobAI>().TargetPosition = castleEntity.GetPosition();
        EnemyMobEntity enemyStats = enemy.GetComponent<EnemyMobEntity>();
        levelManager.SetEnemyStatistics(enemyStats, enemy.GetComponent<EnemyPrefabBaseStats>());
            
        NetworkServer.Spawn(enemy);
        AddEnemy(enemyStats);
    }

    private void Win()
    {
        Debug.Log("Livello " + levelManager.Level + " vinto!");
        if (levelManager.Level >= levelManager.MaxLevel)
        {
            GameWon();
            return;
        }
        readyPlayersForNextLevel = 0;
        RpcShowUpgradeBanner();
    }

    private void GameOver()
    {
        Debug.Log("Partita persa, tutti i player sono stati eliminati oppure il castello è stato distrutto.");
        RemoveAllCharacters();
        RpcShowGameOverBanner();
        StartCoroutine(DisconnectAll(5f));
    }

    private void GameWon()
    {
        Debug.Log("Raggiunto ultimo livello, gioco vinto!");
        RemoveAllCharacters();
        RpcShowGameWonBanner();
        StartCoroutine(DisconnectAll(5f));

    }

    private void RemoveAllCharacters()
    {
        enemies.ForEach(enemy => Destroy(enemy.gameObject));
        Destroy(castle);
    }

    private IEnumerator DisconnectAll(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        NetworkServer.DisconnectAll();
    }


    [Command(requiresAuthority = false)]
    public void CmdRegisterPlayerChoiceAndReady(int increment, NetworkConnectionToClient sender = null)
    {
        if (sender != null && sender.identity != null)
        {
            PlayerEntity playerEntity = sender.identity.GetComponent<PlayerEntity>();

            if (playerEntity != null)
            {
                levelManager.ApplyIndividualUpgrade(playerEntity, increment);
            }

            readyPlayersForNextLevel++;
            
            if (readyPlayersForNextLevel >= NetworkServer.connections.Count)
            {
                levelManager.LevelUp();
                Debug.Log("Tutti i giocatori sono pronti! Passaggio al livello successivo...");
                StartLevel();
            }
        }

    }

    [ClientRpc]
    private void RpcShowUpgradeBanner()
    {
        if (upgradeUIManager != null)
        {
            upgradeUIManager.ShowBanner();
        }
        else
        {
            Debug.Log("UpgradeUIManager non presente nell'inspector!");
        }
    }

    [ClientRpc]
    private void RpcShowGameOverBanner()
    {
        if (endGameUIManager != null)
        {
            Debug.Log("EndGameUIManager c'è");
            endGameUIManager.ShowGameOverBanner();
        }
        else
        {
            Debug.Log("EndGameUIManager non presente nell'inspector!");
        }
    }

    [ClientRpc]
    private void RpcShowGameWonBanner()
    {
        if (endGameUIManager != null)
        {
            endGameUIManager.ShowGameWonBanner();
        }
        else
        {
            Debug.Log("EndGameUIManager non presente nell'inspector!");
        }
        
    }


    private class RemoveEntityVisitor : EntityVisitor
    {
        private readonly GameOrchestrator gameOrchestrator;
        
        public RemoveEntityVisitor(GameOrchestrator gameOrchestrator)
        {
            this.gameOrchestrator = gameOrchestrator;
        }

        public void VisitPlayer(PlayerEntity playerStats)
        {
            gameOrchestrator.players.Remove(playerStats);
            gameOrchestrator.alivePlayers--;
            if (gameOrchestrator.alivePlayers <= 0)
                gameOrchestrator.GameOver();
        }

        public void VisitEnemy(EnemyMobEntity enemyMobStats)
        {
            gameOrchestrator.enemies.Remove(enemyMobStats);
            gameOrchestrator.aliveEnemies--;
            if (gameOrchestrator.aliveEnemies <= 0)
                gameOrchestrator.Win();
        }

        public void VisitBuilding(BuildingEntity buildingEntity)
        { 
            gameOrchestrator.GameOver();
        }
    
    }

}