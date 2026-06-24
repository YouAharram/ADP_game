using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : NetworkBehaviour
{
    private CharacterController playerController;
    
    private Vector2 currentInput; 

    void Start()
    {
        playerController = GetComponent<CharacterController>();
    }
    public void Movements(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        currentInput = context.ReadValue<Vector2>();
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (currentInput != Vector2.zero)
        {
            CmdMovements(currentInput);
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        if (context.performed) CmdAttack();
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