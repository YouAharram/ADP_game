public class BuildingEntity : Entity
{
    public override void Accept(EntityVisitor characterVisitor)
    {
        characterVisitor.VisitBuilding(this);
    }
}