using UnityEngine;

public class PlayerEntity : CharacterEntity
{

    public override void Accept(EntityVisitor entityVisitor)
    {
        entityVisitor.VisitPlayer(this);
    }
}
