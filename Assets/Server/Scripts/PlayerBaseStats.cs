using Mirror;
using UnityEngine;

public class PlayerBaseStats : NetworkBehaviour
{
    [SerializeField] private int baseDamage;
    [SerializeField] private int baseHealth;
    [SerializeField] private int baseSpeed;

    public int BaseDamage { get => baseDamage;}
    public int BaseHealth { get => baseHealth;}
    public int BaseSpeed { get => baseSpeed;}    
}