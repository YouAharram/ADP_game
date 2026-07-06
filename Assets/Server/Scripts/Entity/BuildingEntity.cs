public class BuildingEntity : AllyMobEntity
{
    public override void Accept(CharacterVisitor characterVisitor)
    {
        characterVisitor.VisitAlly(this);
    }
}