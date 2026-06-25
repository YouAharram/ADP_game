public class AllyMobStats : CharacterStats
{
    public override void Accept(CharacterVisitor characterVisitor)
    {
        characterVisitor.VisitAlly(this);
    }
}