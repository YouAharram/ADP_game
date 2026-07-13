using Mirror;
using UnityEngine;

public class EnemyPrefabBaseStats : NetworkBehaviour
{

    [SerializeField] private int rarityIndex;
    [SerializeField] private int baseDamage;
    [SerializeField] private int baseHealth;
    [SerializeField] private int baseSpeed;
    [SerializeField] private int baseAttackPeriodicity;

    public int RarityIndex { get => rarityIndex;}
    public int BaseDamage { get => baseDamage;}
    public int BaseHealth { get => baseHealth;}
    public int BaseSpeed { get => baseSpeed;}
    public int BaseAttackPeriodicity { get => baseAttackPeriodicity;}
}