using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : NetworkBehaviour
{

    private CharacterController playerController;
  
    void Start()
    {
        playerController = GetComponent<CharacterController>();
    }

    void Update()
    {
        
    }
    public void Attack(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        if (context.performed)
        {
            CmdAttack();
        }
    }

    public void Movements(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        Vector2 direction = context.ReadValue<Vector2>();

        CmdMovements(direction);
    }

    [Command]
    private void CmdAttack()
    {
        playerController.Attack();
    }

    [Command]
    private void CmdMovements(Vector2 direction)
    {
        playerController.Move(direction);
    }
    

}