using UnityEngine;
using System.Collections.Generic;

public class AggressiveChaserAI : MobAI
{
    [SerializeField] private bool canAggroPlayers = true;
    [SerializeField] private float triggerDistance = 5f;

    private Queue<Vector2> currentPath;
    private float pathUpdateTimer = 0f;
    private float pathUpdateRate = 0.5f; // tempo del ricalolo del percorso: piu alto -> piu preciso ma piu costoso
                                         // todo: vedere che valore mettere alla fine
    private Vector2 lastTargetPosForPath;

    [SerializeField] private float recalcDistance = 3f;
    [SerializeField] private float minRecalcRate = 0.1f;
    [SerializeField] private float maxRecalcRate = 2f;

    private float moveSpeed;
    
    private float PathUpdateRate =>
        Mathf.Clamp(recalcDistance / Mathf.Max(moveSpeed, 0.01f), minRecalcRate, maxRecalcRate);
    
    protected override void Start()
    {
        base.Start();
        moveSpeed = GetComponent<EnemyMobEntity>().Speed;
    }
    
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
            FollowPath(TargetPosition);
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
            FollowPath(characterDetected.GetPosition());
        }
    }

    private Vector2 currentPathDestination; 

    private void FollowPath(Vector2 destination)
    {
        bool pathExhausted = currentPath == null || currentPath.Count == 0;

        bool pathStillValid = !pathExhausted &&
                              Vector2.Distance(destination, currentPathDestination) < recalcDistance;

        bool timeToRecalc = Time.time > pathUpdateTimer;

        if (pathExhausted || !pathStillValid || timeToRecalc)
        {
            currentPath = PathfindingManager.Instance.GetPath(MyPosition, destination);
            currentPathDestination = destination;
            pathUpdateTimer = Time.time + PathUpdateRate;
        }

        if (currentPath == null || currentPath.Count == 0)
        {
            Vector2 direction = (destination - MyPosition).normalized;
            MobEntity.ChangePosition(direction);
            UpdateFacing(direction);
            return;
        }

        Vector2 nextWaypoint = currentPath.Peek();
        Vector2 dirToWaypoint = (nextWaypoint - MyPosition).normalized;
        MobEntity.ChangePosition(dirToWaypoint);
        UpdateFacing(dirToWaypoint);

        float arrivalThreshold = Mathf.Max(0.3f, moveSpeed * Time.deltaTime * 1.5f);
        if (Vector2.Distance(MyPosition, nextWaypoint) < arrivalThreshold)
        {
            currentPath.Dequeue();
        }
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