using UnityEngine;
using Mirror;
using System;
public abstract class Entity : NetworkBehaviour
{
    private int maxHealth = 100;
    
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
        set 
        {
            maxHealth = value; 
            currentHealth = maxHealth;       
        }
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
    
    [Server]
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true; // scatena OnDeadChanged su tutti i client via hook
            OnDieServer?.Invoke(this);
        }
    }
    
    // Hook SyncVar: gira automaticamente su ogni client quando
    // currentHealth cambia. Nessun RPC manuale necessario.
    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        OnDamageClient?.Invoke();
        OnDamageServer?.Invoke(this);
    }
    
    // Hook SyncVar: gestisce la morte in modo persistente,
    // corretto anche per chi si connette dopo che il personaggio
    // è già morto (isDead sarà già true al momento dello spawn).
    private void OnDeadChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            OnDieClient?.Invoke();
        }
    }
    
    public abstract void Accept(CharacterVisitor characterVisitor);

}
