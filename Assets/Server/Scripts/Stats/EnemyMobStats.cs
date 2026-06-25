using UnityEngine;

public class EnemyMobStats : CharacterStats
{

    [SerializeField] private int attackPeriodicity;
    private int lastAttackFrame = 0;

    public int AttackPeriodicity { get => attackPeriodicity; set => attackPeriodicity = value; }

    public override void Accept(CharacterVisitor characterVisitor)
    {
        characterVisitor.VisitEnemy(this);
    }

    protected override bool IsReadyToAttack()
    {
        return Time.frameCount - lastAttackFrame >= attackPeriodicity;
    }


}