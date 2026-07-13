public class BuildingEntity : Entity
{
    public override void Accept(EntityVisitor entityVisitor)
    {
        entityVisitor.VisitBuilding(this);
    }
}