using UnityEngine;
using Mirror;
using System;

public abstract class CharacterStats : NetworkBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private int maxHealth;
    [SerializeField] private int speed;
    
    [SyncVar] private int currentHealth;
   
    public event Action OnPositionChanged;
    public event Action OnAttacking;
    public event Action OnDamage;
    public event Action<CharacterStats> OnDie;
    
    private Rigidbody2D rb;
    
    public int Damage { get => damage; set => damage = value; }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public int CurrentHealth { get => currentHealth; }
    public int Speed { get => speed; set => speed = value; }
    
    public Vector2 GetPosition() => transform.position;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHealth = maxHealth; 
    }
    
    public void ChangePosition(Vector2 direction)
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);
        OnPositionChanged.Invoke();
    }

    public void AttackCharacter(CharacterStats characterStats)
    {
        if (IsReadyToAttack())
        {
            OnAttacking?.Invoke();

            if (characterStats != null)
            {
                characterStats.TakeDamage(this.damage);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        OnDamage?.Invoke();

        if (currentHealth <= 0) 
        {
            currentHealth = 0;
            OnDie?.Invoke(this);
        }   
    }

    protected virtual bool IsReadyToAttack()
    {
        return true;
    }

    public abstract void Accept(CharacterVisitor characterVisitor);
}