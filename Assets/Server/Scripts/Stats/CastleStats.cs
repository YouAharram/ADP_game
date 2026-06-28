public class CastleStats : AllyMobStats
{
    public override void Accept(CharacterVisitor characterVisitor)
    {
        characterVisitor.VisitAlly(this);
    }
}