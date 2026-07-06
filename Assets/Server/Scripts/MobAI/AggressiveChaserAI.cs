using UnityEngine;

/// <summary>
/// AI GENERICA per qualsiasi mob che insegue: se non e' "distratto" da un
/// player vicino, punta verso l'edificio bersaglio e lo attacca quando e'
/// a portata; se un player entra in TriggerRange, gli si avvicina e lo
/// attacca appena e' a portata di HitRange.
/// </summary>
public class AggressiveChaserAI : MobAI
{
    [Tooltip("Se false, il mob ignora sempre i player e punta solo all'edificio")]
    [SerializeField] private bool canAggroPlayers = true;

    [Tooltip("Distanza minima dal target attuale sotto la quale si considera 'gia' li' e non serve reagire di nuovo")]
    [SerializeField] private float triggerDistance = 5f;

    protected override bool CanBeTriggered()
    {
        if (!canAggroPlayers) return false;
        return Vector2.Distance(TargetPosition, MyPosition) > triggerDistance;
    }

    protected override void MainGoal()
    {
        BuildingEntity building = Detector.BuildingInRange(MobEntity.HitRange);
        if (building != null)
        {
            MobEntity.ChangePosition(Vector2.zero);
            MobEntity.AttackCharacter(TargetInfo.FromEntity(building));
        }
        else
        {
            MobEntity.ChangePosition((TargetPosition - MyPosition).normalized);
        }
    }

    protected override void Trigger(Entity characterDetected)
    {
        CharacterEntity playerToHit = Detector.CharacterInRange(MobEntity.HitRange);

        if (playerToHit != null)
        {
            MobEntity.ChangePosition(Vector2.zero);
            MobEntity.AttackCharacter(TargetInfo.FromEntity(playerToHit));
        }
        else
        {
            MobEntity.ChangePosition((characterDetected.GetPosition() - MyPosition).normalized);
        }
    }
}
