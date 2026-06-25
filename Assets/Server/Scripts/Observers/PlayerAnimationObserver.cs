using UnityEngine;
using Mirror;

public class PlayerAnimationObserver : NetworkBehaviour
{
    private PlayerStats playerStats;
    private Vector2 oldPosition;
    private Animator animator;
    private SpriteRenderer sr;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            oldPosition = playerStats.GetPosition();
            playerStats.OnAttacking += AnimationAttack;
            playerStats.OnDamage += AnimationDamage;
            playerStats.OnPositionChanged += AnimationMovements;
            playerStats.OnDie += AnimationDie;
            animator = GetComponent<Animator>();
            sr = GetComponent<SpriteRenderer>();
        }
    }


    void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnAttacking -= AnimationAttack;
            playerStats.OnDamage -= AnimationDamage;
            playerStats.OnPositionChanged -= AnimationMovements;
            playerStats.OnDie -= AnimationDie;
        }
    }


    [ClientRpc]
    public void AnimationMovements()
    {
        Vector2 currentPosition = playerStats.GetPosition();
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
    
    [ClientRpc]
    public void AnimationAttack()
    {
        animator.SetTrigger("Attack");
    }
    
    public void AnimationDie(CharacterStats characterStats) { Debug.Log("Animazione Player Morte"); }

    public void AnimationDamage() { Debug.Log("Animazione Danno"); }

}