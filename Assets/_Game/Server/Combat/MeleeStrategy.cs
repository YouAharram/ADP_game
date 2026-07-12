using Mirror;
using UnityEngine;
using System;

public class MeleeStrategy : NetworkBehaviour, AttackStrategy
{
    private Rigidbody2D rb;
    private float knockbackForce = 3f;
    private float knockbackTime = 0.3f;
    private float stunTime = 0.2f;
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
