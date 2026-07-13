using UnityEngine;

public class PlayerEntity : CharacterEntity
{
    public override void Accept(EntityVisitor characterVisitor)
    {
        characterVisitor.VisitPlayer(this);
    }
}
