using UnityEngine;

public class EnemyCollisionHandler : CollisionHandler
{
    [SerializeField] private LayerMask layerTarget;

    public override CharacterStats CollidingCharacter()
    {
        return FindCollidingCharacter<CharacterStats>(layerTarget);
    }
}