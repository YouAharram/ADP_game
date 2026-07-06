using Mirror;

public class MeleeStrategy : NetworkBehaviour, AttackStrategy
{
    public void Attack(CharacterEntity attacker, TargetInfo targetInfo)
    {
        Entity targetEntity = targetInfo.Entity;
        if (targetEntity != null)
        {
            targetEntity.TakeDamage(attacker.Damage);
        }
    }
}
