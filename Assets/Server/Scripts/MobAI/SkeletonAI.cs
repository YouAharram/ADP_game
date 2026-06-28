using Unity.VisualScripting;
using UnityEngine;

public class SkeletonAI : MobAI
{
    private Vector2 targetPosition;
    private float triggerDistance = 5f;
    
    private float lastAttackTime = 0f;
    [SerializeField] private float attackCooldown = 1.5f; 

    public Vector2 TargetPosition { set => targetPosition = value; }

    protected override void Start()    
    {
        base.Start();
        targetPosition = new Vector2(0f, 0f);   
    }
    
    protected override bool CanBeTriggered()
    {
        return Vector2.Distance(targetPosition, MyPosition) > triggerDistance;
    }

    protected override void MainGoal()
    {
        if (CharacterDetector.CharacterInRange(HitRange) == null)
        {
            Vector2 direction = (targetPosition - MyPosition).normalized;
            MobController.Move(direction);
        }
        else
        {
            MobController.Move(Vector2.zero); 
            
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                MobController.Attack();
                lastAttackTime = Time.time; 
            }
        }
    }

    protected override void Trigger(CharacterStats characterDetected)
    {
        if (CharacterDetector.CharacterInRange(HitRange) != null)
        {
            MobController.Move(Vector2.zero); 
            
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                MobController.Attack();
                lastAttackTime = Time.time; 
            }
        }
        else
        {
            Vector2 direction = (characterDetected.GetPosition() - MyPosition).normalized;
            MobController.Move(direction);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRange);
    }
}