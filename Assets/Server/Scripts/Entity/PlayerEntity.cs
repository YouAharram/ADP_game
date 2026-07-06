using UnityEngine;

public class PlayerEntity : CharacterEntity
{
    public override void Accept(CharacterVisitor characterVisitor)
    {
        characterVisitor.VisitPlayer(this);
    }
}
