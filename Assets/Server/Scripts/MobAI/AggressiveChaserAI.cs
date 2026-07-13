using UnityEngine;

public class AggressiveChaserAI : MobAI
{
    [SerializeField] private bool canAggroPlayers = true;

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
