using UnityEngine;
using Mirror;
using System;

public abstract class CharacterStats : NetworkBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private int maxHealth;
    [SerializeField] private int speed;
    
    [SyncVar] private int currentHealth;
    [SyncVar] private bool isAttacking;
   
    public event Action OnStateChanged;

    public int Damage { get => damage; set => damage = value; }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public int CurrentHealth { get => currentHealth; }
    public int Speed { get => speed; set => speed = value; }
    public bool IsAttacking { get => isAttacking; }
    
    public Vector2 GetPosition() => transform.position;

    public override void OnStartServer()
    {
        base.OnStartServer();
        isAttacking = false;
        currentHealth = maxHealth; 
    }
    
    public void ChangePosition(Vector2 direction)
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);
        OnStateChanged?.Invoke();
    }

    public void AttackCharacter(CharacterStats characterStats)
    {
        if (IsReadyToAttack())
        {
            isAttacking = true;
            OnStateChanged?.Invoke();

            if (characterStats != null)
            {
                characterStats.TakeDamage(this.damage);
            }

            isAttacking = false;
            OnStateChanged?.Invoke();   
        }

    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) 
        {
            currentHealth = 0;
        }

        OnStateChanged?.Invoke();
    }

    protected virtual bool IsReadyToAttack()
    {
        return true;
    } 
}