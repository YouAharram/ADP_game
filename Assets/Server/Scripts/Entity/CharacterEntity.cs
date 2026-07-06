using UnityEngine;
using Mirror;
using System;

public abstract class CharacterEntity : Entity
{
    [SerializeField] private int damage;
    [SerializeField] private int speed;
    [SerializeField] private float hitRange;

    [SerializeField] private float attackPeriodicity = 1.5f;
    private float lastAttackTime = -Mathf.Infinity;

    private AttackStrategy attackStrategy;

    public event Action OnAttackingClient;

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

    protected void Awake()
    {
        base.Awake();
        attackStrategy = GetComponent<AttackStrategy>();
    }

    public void ChangePosition(Vector2 direction)
    {
        // Il movimento visivo (flip, blend animazione) NON passa più
        // da qui: NetworkTransform sincronizza rb.position, e l'
        // AnimationObserver lo legge in Update() lato client.
        Rb.linearVelocity = direction.normalized * speed;
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
}