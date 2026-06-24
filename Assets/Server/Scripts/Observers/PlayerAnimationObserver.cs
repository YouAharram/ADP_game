using UnityEngine;
using Mirror;
public class PlayerAnimationObserver : NetworkBehaviour
{
    private PlayerStats playerStats;
    private Vector2 oldPosition;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            oldPosition = playerStats.GetPosition();
            playerStats.OnStateChanged += UpdateObserver;
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
        
        if (playerStats.GetPosition() != oldPosition)
        {
            AnimationMovements();
            oldPosition = playerStats.GetPosition();
        }
    }

    public void AnimationAttack() { Debug.Log("Animazione Player Attacco"); }
    public void AnimationMovements() { Debug.Log("Animazione Player Movimento"); }
    public void AnimationDie() { Debug.Log("Animazione Player Morte"); }
}