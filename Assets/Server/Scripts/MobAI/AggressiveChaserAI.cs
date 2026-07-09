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
            FollowFlow(TargetPosition);
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
            FollowFlow(characterDetected.GetPosition());
        }
    }

    private void FollowFlow(Vector2 destination)
    {
        Vector2 flowDirection = FlowFieldManager.Instance.GetDirection(MyPosition, destination);

        MobEntity.ChangePosition(flowDirection);
        UpdateFacing(flowDirection);
    }

    private void UpdateFacing(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) < 0.01f) return;

        bool shouldFaceRight = direction.x > 0f;
        if (MobEntity.IsFacingRight != shouldFaceRight)
        {
            MobEntity.IsFacingRight = shouldFaceRight;
        }
    }
}