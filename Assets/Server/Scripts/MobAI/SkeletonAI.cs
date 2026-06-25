using Unity.VisualScripting;
using UnityEngine;

public class SkeletonAI : MobAI
{
    private Vector2 targetPosition;

    public Vector2 TargetPosition { set => targetPosition = value; }

    protected override void Start()    
    {
        base.Start();
        targetPosition = new Vector2(0f, 0f);   
    }

    protected override void MainGoal()
    {
        Vector2 direction = (targetPosition - MyPosition).normalized;
        MobController.Move(direction);
    }

    protected override void Trigger(CharacterStats characterDetected)
    {
        Vector2 direction = (characterDetected.GetPosition() - MyPosition).normalized;
        MobController.Move(direction);

        if (CharacterDetector.CharacterInRange(HitRange) != null)
        {
            MobController.Attack();
        }
    }
    
}