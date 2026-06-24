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
            playerStats.OnStateChanged += UpdateObserver;
            animator = GetComponent<Animator>();
            sr = GetComponent<SpriteRenderer>();
        }
    }


    void OnDestroy()
    {
        if (playerStats != null)
            playerStats.OnStateChanged -= UpdateObserver;
    }

    public void UpdateObserver()
    {
        if (playerStats.CurrentHealth == 0)
        {
            AnimationDie();
            return;
        }

        if (playerStats.IsAttacking)
        {
            AnimationAttack();
        }
        Vector2 currentPosition = playerStats.GetPosition();
        if (currentPosition != oldPosition)
        {
            Vector2 movement = (currentPosition - oldPosition).normalized;
            AnimationMovements(movement);
            oldPosition = currentPosition;
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    [ClientRpc]
    public void AnimationMovements(Vector2 movement)
    {
        if (movement.x > 0)
            sr.flipX = false;
        else if (movement.x < 0)
            sr.flipX = true; 
        animator.SetFloat("Speed", movement.magnitude);
    }
    
    [ClientRpc]
    public void AnimationAttack()
    {
        animator.SetTrigger("Attack");
    }
    
    public void AnimationDie() { Debug.Log("Animazione Player Morte"); }

}