using UnityEngine;
using Mirror;
using System;

public abstract class CharacterEntity : Entity
{
    private int damage;
    private int speed;
    private float hitRange = 2;

    [SerializeField] private float attackPeriodicity = 0.2f;
    private float lastAttackTime = -Mathf.Infinity;
    
    [SyncVar] private bool isFacingRight;
    
    private AttackStrategy attackStrategy;

    public event Action OnAttackingClient;
    public event Action<bool> OnFlipToRightClient;

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

    public bool IsFacingRight => isFacingRight;

    protected override void Awake()
    {
        base.Awake();
        attackStrategy = GetComponent<AttackStrategy>();
        isFacingRight = transform.localScale.x > 0;
    }

    [Server]
    public void SetFacingDirection(bool faceRight)
    {
        if (isFacingRight == faceRight) return;

        Debug.Log($"[{name}] SetFacingDirection chiamato: {faceRight}");
        isFacingRight = faceRight;
        
        RpcNotifyFlipToRight(faceRight); 
    }

    private void Update()
    {
        Debug.Log("[" + name +"] IsFacingRight: " + isFacingRight);
    }

    [Server]
    public void ChangePosition(Vector2 direction, bool isSprinting = false)
    {
        Rb.linearVelocity = (isSprinting ? 2 : 1) * direction.normalized * speed ;
    }

    [Server]
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
    private void RpcNotifyFlipToRight(bool faceRight)
    {
        isFacingRight = faceRight; 
        
        OnFlipToRightClient?.Invoke(faceRight); 
    }
}