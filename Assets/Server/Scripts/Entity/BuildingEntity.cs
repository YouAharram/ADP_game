public class BuildingEntity : Entity
{
    public override void Accept(CharacterVisitor characterVisitor)
    {
        characterVisitor.VisitBuilding(this);
    }
}