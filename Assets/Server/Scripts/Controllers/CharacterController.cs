using UnityEngine;
using Mirror;
using System.Reflection.PortableExecutable;

public class CharacterController : NetworkBehaviour
{
    private CharacterStats characterStats;
    private CollisionHandler characterCollisionHandler;

    void Start()
    {
        characterStats = GetComponent<CharacterStats>();
        characterCollisionHandler = GetComponent<CollisionHandler>();
    }

    [ServerCallback]
    public void Attack()
    {        
        CharacterStats collidingCharacter = characterCollisionHandler.CollidingCharacter();
        characterStats.AttackCharacter(collidingCharacter);
        
    }

    [ServerCallback]
    public void Move(Vector2 direction)
    {
        characterStats.ChangePosition(direction);
    }
}
