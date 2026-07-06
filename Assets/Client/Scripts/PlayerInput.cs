using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : NetworkBehaviour
{
    private CharacterController playerController;
    private Vector2 currentInput; 
    private bool isFacingRight = true;
    
    private Camera mainCam;

    void Start()
    {
        playerController = GetComponent<CharacterController>();
        mainCam = Camera.main; 
    }

    public void Movements(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        currentInput = context.ReadValue<Vector2>();
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        CmdMovements(currentInput);

        HandleFacingDirection();
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        
        if (context.performed) 
        {
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

            Vector2 mouseWorldPosition = mainCam.ScreenToWorldPoint(mouseScreenPosition);

            CmdAttackAtPosition(mouseWorldPosition);
        }
    }
    
    private void HandleFacingDirection()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPosition = mainCam.ScreenToWorldPoint(mouseScreenPosition);

        if (mouseWorldPosition.x > transform.position.x && !isFacingRight)
        {
            isFacingRight = true;
            CmdChangeFacingDirection(isFacingRight);
        }
        else if (mouseWorldPosition.x < transform.position.x && isFacingRight)
        {
            isFacingRight = false;
            CmdChangeFacingDirection(isFacingRight);
        }
    }

    [Command]
    private void CmdAttackAtPosition(Vector2 clickPosition)
    {
        playerController.TryAttackTargetAt(clickPosition);
    }

    [Command]
    private void CmdMovements(Vector2 direction)
    {
        playerController.Move(direction);
    }
    
    [Command]
    private void CmdChangeFacingDirection(bool facingRight)
    {
        Debug.Log("Chiamo SetFacing del controller");
        playerController.SetFacing(facingRight);
    }
}