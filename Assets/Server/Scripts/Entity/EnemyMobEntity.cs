using UnityEngine;

public class EnemyMobEntity : CharacterEntity
{
    public override void Accept(EntityVisitor characterVisitor)
    {
        characterVisitor.VisitEnemy(this);
    }
    
}