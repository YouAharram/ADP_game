using System;
using Mirror;
using UnityEngine;

public class LevelManager : NetworkBehaviour
{
    private const int MaxLevelIncrement = 20;
    private int level = 1;
    private int totalEnemiesQuantity;   
    private int enemyHealthIncrement;
    private int enemyDamageIncrement;
    private int enemySpeedIncrement;

    private int playerHealthIncrement;
    private int playerDamageIncrement;
    private int playerSpeedIncrement;
    
    private EnemyPrefabExtractor enemyExtractor;

    public EnemyPrefabExtractor EnemyExtractor { get => enemyExtractor; set => enemyExtractor = value; }
    public int Level { get => level; }

    private void Awake()
    {
        level = 1;
        enemyHealthIncrement = 0;
        enemyDamageIncrement = 0;
        enemySpeedIncrement = 0;

        playerHealthIncrement = 0;
        playerDamageIncrement = 0;
        playerSpeedIncrement = 0;

        totalEnemiesQuantity = 1; // CalculateTotalEnemiesQuantity();
    }

    public void GenerateEnemies(GameOrchestrator gameOrchestrator)
    {
        EnemyExtractor.Prefabs = gameOrchestrator.EnemyPrefabs;
        
        for (int i = 0; i < totalEnemiesQuantity; i++)
        {
            gameOrchestrator.GenerateEnemy(EnemyExtractor.ExtractEnemyPrefab());
            Debug.Log("Spawnato mob");
        }
    }

    public void SetEnemyStatistics(EnemyMobEntity enemy, EnemyPrefabBaseStats enemyBaseStats)
    {
        enemy.Damage = enemyDamageIncrement + enemyBaseStats.BaseDamage;
        enemy.Speed = enemySpeedIncrement + enemyBaseStats.BaseSpeed;
        enemy.MaxHealth = enemyHealthIncrement + enemyBaseStats.BaseHealth;
        enemy.AttackPeriodicity = enemyBaseStats.BaseAttackPeriodicity;
    }

    public void SetPlayerStatistics(PlayerEntity player, PlayerBaseStats playerBaseStats)
    {
        player.MaxHealth = playerHealthIncrement + playerBaseStats.BaseHealth;
        player.Damage = playerDamageIncrement +  playerBaseStats.BaseDamage;
        player.Speed = playerSpeedIncrement + playerBaseStats.BaseSpeed;
    }

    public void LevelUp()
    {
        level++;
        UpdateStatistics();
    }

    private void UpdateStatistics()
    {
        totalEnemiesQuantity = CalculateTotalEnemiesQuantity();

        enemyHealthIncrement += IncrementStatistic(10);
        enemyDamageIncrement += IncrementStatistic(5);
        enemySpeedIncrement += IncrementStatistic(1);

        playerHealthIncrement += IncrementStatistic(5);
        playerDamageIncrement += IncrementStatistic(5);
        playerSpeedIncrement += IncrementStatistic(1);
    }

    private int CalculateTotalEnemiesQuantity()
    {
        if (Level < MaxLevelIncrement)
            return (int) Math.Ceiling(20 + (Level - 1)*(10-0.25*(Level-1)));
        return 120;
    }

    private int IncrementStatistic(int amount)
    {
        if (Level < MaxLevelIncrement)
            return amount;
        return 0;
    }

  

}