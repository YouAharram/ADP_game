public class SentryAI : MobAI
{
    protected override void Trigger(Entity characterDetected)
    {
        MobEntity.AttackCharacter(TargetInfo.FromEntity(characterDetected));
    }
}
