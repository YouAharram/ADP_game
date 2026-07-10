using UnityEngine;
using Mirror;
using System;

public abstract class Entity : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnMaxHealthChanged))] 
    private int maxHealth;
    
    [SyncVar(hook = nameof(OnHealthChanged))]
    private int currentHealth;
    
    [SyncVar(hook = nameof(OnDeadChanged))]
    private bool isDead;
    
    private Rigidbody2D rb;
    
    public event Action OnDamageClient;
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

        Debug.Log($"[Entity] OnStartClient su '{name}' -> hp={currentHealth}/{maxHealth}, isDead={isDead}");

        // Forza l'aggiornamento grafico appena il client riceve l'oggetto
        OnDamageClient?.Invoke();
        if (isDead) OnDieClient?.Invoke();
    }
    
    [Server]
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            OnDieServer?.Invoke(this);
        }
    }
    
    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        Debug.Log($"[Entity] OnHealthChanged su '{name}': {oldHealth} -> {newHealth}");
        OnDamageClient?.Invoke();
        OnDamageServer?.Invoke(this);
    }
    
    private void OnMaxHealthChanged(int oldVal, int newVal)
    {
        Debug.Log($"[Entity] OnMaxHealthChanged su '{name}': {oldVal} -> {newVal}");
        // Se cambia la vita massima, la barra deve ricalcolarsi
        OnDamageClient?.Invoke();
    }
    
    private void OnDeadChanged(bool oldValue, bool newValue)
    {
        if (newValue) OnDieClient?.Invoke();
    }
    
    public abstract void Accept(CharacterVisitor characterVisitor);
}