using UnityEngine;
using Mirror;
using System;

public abstract class Entity : NetworkBehaviour
{
    [SyncVar] private int maxHealth;
    
    [SyncVar] private int currentHealth;
    
    [SyncVar] private bool isDead;
    
    private Rigidbody2D rb;
    
    public event Action OnDamageClient;
    public event Action<int, int> OnHealthChangedClient;
    public event Action OnDieClient;
    
    public event Action<Entity> OnDamageServer;
    public event Action<Entity> OnDieServer;
    
    public Rigidbody2D Rb { get => rb; set => rb = value; }
    
    public int MaxHealth 
    { 
        get => maxHealth; 
        [Server] set { maxHealth = value; currentHealth = maxHealth; }
    }
    
    public int CurrentHealth => currentHealth;
    public bool IsDead => isDead;
    
    public Vector2 GetPosition() => rb.position;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHealth = maxHealth;
        isDead = false;
    }
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        // paint iniziale / late-join: lo stato è già sincronizzato qui
        OnHealthChangedClient?.Invoke(currentHealth, maxHealth);
        if (isDead) OnDieClient?.Invoke();
    }
    
    
    [Server]
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
        
        Debug.Log($"[Entity] TakeDamage su '{name}': hp={currentHealth}/{maxHealth}, isDead={isDead}");

        OnDamageServer?.Invoke(this);
        RpcNotifyDamage(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            RpcNotifyDie();
            OnDieServer?.Invoke(this);
        }
    }
    
    [Server]
    public virtual void Knockback(Vector2 knockbackDirection, float knockbackForce, float knockbackTime, float stunTime) {}
    
    [ClientRpc]
    private void RpcNotifyDamage(int cur, int max)
    {
        OnDamageClient?.Invoke();                 
        OnHealthChangedClient?.Invoke(cur, max);
    }
    
    [ClientRpc]
    private void RpcNotifyDie()
    {
        if (isDead) OnDieClient?.Invoke();
    }
    
    public abstract void Accept(EntityVisitor entityVisitor);
}