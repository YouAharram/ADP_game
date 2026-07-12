using Mirror;
using UnityEngine;
using System;

public class MeleeStrategy : NetworkBehaviour, AttackStrategy
{
    private Rigidbody2D rb;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackTime;
    [SerializeField] private float stunTime;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void Attack(CharacterEntity attacker, TargetInfo targetInfo)
    {
        Entity targetEntity = targetInfo.Entity;
        if (targetEntity != null)
        {
            Vector2 attackDirection = (targetEntity.Rb.position - rb.position).normalized;
            targetEntity.Knockback(attackDirection, knockbackForce, knockbackTime, stunTime);
            targetEntity.TakeDamage(attacker.Damage);
        }
    }
}
