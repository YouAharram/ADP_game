using UnityEngine;
using Mirror;
using System.Reflection.PortableExecutable;

public class CharacterController : NetworkBehaviour
{
    private PlayerEntity playerEntity;

    void Start()
    {
        playerEntity = GetComponent<PlayerEntity>();
    }
    
    [ServerCallback]
    public void TryAttackTargetAt(Vector2 clickPosition)
    {
        playerEntity.AttackCharacter(TargetInfo.FromPosition(clickPosition));
    }

    [ServerCallback]
    public void Move(Vector2 direction)
    {
        playerEntity.ChangePosition(direction);
    }
}
