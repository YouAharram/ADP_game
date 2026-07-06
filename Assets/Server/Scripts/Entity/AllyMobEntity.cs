using UnityEngine;

public class AllyMobEntity : CharacterEntity
{
    public override void Accept(CharacterVisitor characterVisitor)
    {
        characterVisitor.VisitAlly(this);
    }
}