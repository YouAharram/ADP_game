using UnityEngine;

public class PlayerCollisionHandler : CollisionHandler
{
    [SerializeField] private LayerMask layerEnemy;

    public override CharacterStats CollidingCharacter()
    {
        return FindCollidingCharacter<EnemyMobStats>(layerEnemy);
    }
}