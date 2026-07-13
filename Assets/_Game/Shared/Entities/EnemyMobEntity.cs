using UnityEngine;

public class EnemyMobEntity : CharacterEntity
{
    public override void Accept(EntityVisitor entityVisitor)
    {
        entityVisitor.VisitEnemy(this);
    }
    
}