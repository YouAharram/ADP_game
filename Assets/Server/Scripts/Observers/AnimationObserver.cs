using UnityEngine;
using Mirror;

public class AnimationObserver : NetworkBehaviour
{
    private CharacterStats characterStats;
    private Vector2 oldPosition;
    private Animator animator;
    private SpriteRenderer sr;

    void Start()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        characterStats = GetComponent<CharacterStats>();
        if (characterStats != null)
        {
            oldPosition = characterStats.GetPosition();
            characterStats.OnAttacking += AnimationAttack;
            characterStats.OnDamage += AnimationDamage;
            characterStats.OnPositionChanged += AnimationMovements;
            characterStats.OnDie += AnimationDie;
        }
    }

    
    void OnDestroy()
    {
        if (characterStats != null)
        {
            characterStats.OnAttacking -= AnimationAttack;
            characterStats.OnDamage -= AnimationDamage;
            characterStats.OnPositionChanged -= AnimationMovements;
            characterStats.OnDie -= AnimationDie;
        }
    }


    [ClientRpc]
    public void AnimationMovements()
    {
        if (animator != null && sr != null)
        {
            Vector2 currentPosition = characterStats.GetPosition();
            if (currentPosition == oldPosition)
            {
                animator.SetFloat("Speed", 0f);
                return;
            }

            Vector2 movement = (currentPosition - oldPosition).normalized;
            if (movement.x > 0)
                sr.flipX = false;
            else if (movement.x < 0)
                sr.flipX = true;
            animator.SetFloat("Speed", movement.magnitude);
            oldPosition = currentPosition;
        }
    }
    
    [ClientRpc]
    public void AnimationAttack()
    {
        if (animator != null)
        {
            Debug.Log("Sto attacando");
            animator.SetTrigger("Attack");
        }
    }
    
    public void AnimationDie(CharacterStats characterStats) { Debug.Log("Animazione Player Morte"); }

    public void AnimationDamage() { Debug.Log("Animazione Danno"); }

}