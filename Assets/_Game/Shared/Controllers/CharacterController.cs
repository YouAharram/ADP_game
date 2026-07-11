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
    public void Move(Vector2 direction, bool isSprinting = false)
    {
        playerEntity.ChangePosition(direction, isSprinting);
    }
    
    [ServerCallback]
    public void SetFacing(bool isFacingRight)
    {
        Debug.Log("Chiamo  SetFacing del playerEntity");
        playerEntity.SetFacingDirection(isFacingRight);
    }
}
