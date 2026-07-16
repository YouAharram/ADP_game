using UnityEngine;
using Mirror;
using System;
using System.Collections;

public abstract class CharacterEntity : Entity
{
    [SyncVar] private int damage;
    [SyncVar] private int speed;
    [SyncVar] private float hitRange = 1f;
    [SyncVar] private bool isStunned = false;

    [SerializeField] private float attackPeriodicity = 0.2f;
    private float lastAttackTime = -Mathf.Infinity;
    
    [SyncVar] private bool isFacingRight;
    
    private AttackStrategy attackStrategy;

    public event Action OnAttackingClient;
    public event Action<bool> OnFlipToRightClient;

	public event Action<int> OnDamageChanged;

	
    public int Damage
	{
    	get => damage;
    	set => SetDamage(value);
	}

	[Server]
	private void SetDamage(int value)
	{
   		damage = value;
    	RpcNotifyDamage(value);
	}	

	[ClientRpc]
	private void RpcNotifyDamage(int value)
	{
   		damage = value;
   		OnDamageChanged?.Invoke(value);
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
    public override void Knockback(Vector2 knockbackDirection, float knockbackForce, float knockbackTime, float stunTime)
    {
        if (isStunned || IsDead) return;
        
        Debug.Log($"[{name}] Knockback chiamato da {knockbackDirection} con forza {knockbackForce}, durata knockback {knockbackTime}, durata stun {stunTime}");
        Rb.linearVelocity = knockbackDirection.normalized * knockbackForce;
        StartCoroutine(StunTimer(knockbackTime, stunTime));

    }

    IEnumerator StunTimer(float knockbackTime, float stunTime)
    {
        isStunned = true;
        yield return new WaitForSeconds(knockbackTime);
        Rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
    }
    
    [Server]
    public void SetFacingDirection(bool faceRight)
    {
        if (isStunned) return;
        
        if (isFacingRight == faceRight) return;

        Debug.Log($"[{name}] SetFacingDirection chiamato: {faceRight}");
        isFacingRight = faceRight;
        
        RpcNotifyFlipToRight(faceRight); 
    }

    [Server]
    public void ChangePosition(Vector2 direction, bool isSprinting = false)
    {
        if (isStunned) return;
        Rb.linearVelocity = (isSprinting ? 2 : 1) * direction.normalized * speed ;
    }

    [Server]
    public void AttackCharacter(TargetInfo targetInfo)
    {
        if (isStunned) return;
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