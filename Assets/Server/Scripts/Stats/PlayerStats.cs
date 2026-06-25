public class PlayerStats : CharacterStats
{
    public override void Accept(CharacterVisitor characterVisitor)
    {
        characterVisitor.VisitPlayer(this);
    }
}