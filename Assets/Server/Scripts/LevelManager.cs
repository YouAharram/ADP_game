using System;
using System.Collections;
using Mirror;
using UnityEngine;

public class LevelManager : NetworkBehaviour
{
    private const int maxLevel = 5;
    private int level = 1;
    private int totalEnemiesQuantity;   
    private int enemyHealthIncrement;
    private int enemyDamageIncrement;
    private int enemySpeedIncrement;

    private int castleDamageIncrement;
    
    private EnemyPrefabExtractor enemyExtractor;

    public EnemyPrefabExtractor EnemyExtractor { get => enemyExtractor; set => enemyExtractor = value; }
    public int Level => level;
    public int MaxLevel => maxLevel;

    private void Awake()
    {
        level = 1;
        enemyHealthIncrement = 0;
        enemyDamageIncrement = 0;
        enemySpeedIncrement = 0;

        castleDamageIncrement = 0;
        
        totalEnemiesQuantity = CalculateTotalEnemiesQuantity();
    }

    public void GenerateEnemies()
    {
        EnemyExtractor.Prefabs = GameOrchestrator.Instance.EnemyPrefabs;
        StartCoroutine(SpawnEnemiesWithDeltaTime(1.0f));
    }

    private IEnumerator SpawnEnemiesWithDeltaTime(float deltaTime)
    {
        for (int i = 0; i < totalEnemiesQuantity; i++)
        {
            GameOrchestrator.Instance.GenerateEnemy(EnemyExtractor.ExtractEnemyPrefab());
            Debug.Log("Spawnato mob");

            yield return new WaitForSeconds(deltaTime);
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
        player.MaxHealth = playerBaseStats.BaseHealth;
        player.Damage = playerBaseStats.BaseDamage;
        player.Speed = playerBaseStats.BaseSpeed;
    }

    public void SetCastleHealth(BuildingEntity castle, int castleBaseHealth)
    {
        castle.MaxHealth = castleBaseHealth + castleDamageIncrement;
    }

    public void ApplyIndividualUpgrade(PlayerEntity player, int increment)
    {
        player.Damage += increment; 
        player.MaxHealth += IncrementStatistic(5);
        Debug.Log($"[SERVER] Potenziato attacco a {player.name} di {increment}");
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
        
        castleDamageIncrement += IncrementStatistic(100);
    }

    private int CalculateTotalEnemiesQuantity()
    {
        if (Level < maxLevel)
            return (int) Math.Ceiling(20 + (Level - 1)*(10-0.25*(Level-1)));
        return 120;
    }

    private int IncrementStatistic(int amount)
    {
        if (Level < maxLevel)
            return amount;
        return 0;
    }

  

}