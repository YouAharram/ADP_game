using UnityEngine;
using Mirror;
using System;

public abstract class CharacterEntity : Entity
{
    private int damage = 10;
    private int speed = 5;
    private float hitRange = 2;

    [SerializeField] private float attackPeriodicity = 0.2f;
    private float lastAttackTime = -Mathf.Infinity;
    
    private bool isFacingRight = true;
    
    private AttackStrategy attackStrategy;

    public event Action OnAttackingClient;
    public event Action OnFlipDirectionClient;

    public int Damage
    {
        get => damage;
        set => damage = value;
    }

    public int Speed
    {
        get => speed;
        set => speed = value;
    }

    public float AttackPeriodicity
    {
        get => attackPeriodicity;
        set => attackPeriodicity = value;
    }

    public float HitRange
    {
        get => hitRange;
        set => hitRange = value;
    }

    public bool IsFacingRight
    {
        get => isFacingRight;
        set
        {
            Debug.Log("SetIsFacing chiamato");
            isFacingRight = value;
            RpcNotifyFlipDirection();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        attackStrategy = GetComponent<AttackStrategy>();
    }

    public void ChangePosition(Vector2 direction, bool isSprinting = false)
    {
        // Il movimento visivo (flip, blend animazione) NON passa più
        // da qui: NetworkTransform sincronizza rb.position, e l'
        // AnimationObserver lo legge in Update() lato client.
        
        
        Rb.linearVelocity = (isSprinting ? 2 : 1) * direction.normalized * speed ;
    }

    public void AttackCharacter(TargetInfo targetInfo)
    {
        if (!IsReadyToAttack()) return;
        TriggerAttackEvent();
        attackStrategy.Attack(this, targetInfo);
    }

    protected bool IsReadyToAttack()
    {
        if (Time.time - lastAttackTime < attackPeriodicity) return false;
        lastAttackTime = Time.time;
        return true;
    }

    protected void TriggerAttackEvent()
    {
        RpcNotifyAttack();
    }

    [ClientRpc]
    private void RpcNotifyAttack()
    {
        OnAttackingClient?.Invoke();
    }
    
    [ClientRpc]
    private void RpcNotifyFlipDirection()
    {
        Debug.Log("Chiamo evento");
        OnFlipDirectionClient?.Invoke();
    }
}