using UnityEngine;

public class EnemyMobEntity : CharacterEntity
{
    public override void Accept(CharacterVisitor characterVisitor)
    {
        characterVisitor.VisitEnemy(this);
    }
    
}