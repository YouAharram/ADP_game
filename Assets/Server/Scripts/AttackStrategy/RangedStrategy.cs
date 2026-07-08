using System;
using UnityEngine;
using System.Collections;
using Mirror;

public class RangedStrategy : NetworkBehaviour, AttackStrategy
{
    private ProjectileManager projectileManager;
    [SerializeField] private Vector2 fireOrigin = new Vector2(0.5f, 0.5f);
    [SerializeField] private float attackAnimationDelay = 0.5f;

    private void Awake()
    {
        projectileManager = GetComponent<ProjectileManager>();
    }

    public void Attack(CharacterEntity attacker, TargetInfo targetInfo)
    {
        Vector2 targetPosition = targetInfo.Position;
        if (targetPosition != null)
        {
            Debug.Log("Strategy spara");
            StartCoroutine(FireDelayedRoutine(attacker, targetPosition));
        }
    }
    
    private IEnumerator FireDelayedRoutine(CharacterEntity attacker, Vector2 targetPosition)
    {
        yield return new WaitForSeconds(attackAnimationDelay);

        if (projectileManager == null) yield break;

        Vector2 origin = new Vector2(0, 0);
        if (attacker.IsFacingRight)
        {
            origin = attacker.GetPosition() + fireOrigin; 
        }
        else
        {
            origin = attacker.GetPosition() + fireOrigin * new Vector2(-1, 1);
        }
        
        projectileManager.ShootProjectile(origin, targetPosition, attacker.Damage);
    }
}
