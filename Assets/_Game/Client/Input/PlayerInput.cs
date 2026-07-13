using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : NetworkBehaviour
{
    private CharacterController playerController;
    private Vector2 currentInput; 
    private bool isFacingRight = true;
    private bool isSprinting = false;
    
    private Camera mainCam;

    void Start()
    {
        playerController = GetComponent<CharacterController>();
        mainCam = Camera.main;
        HandleFacingDirection();
    }

    private Vector2 GetMouseWorldPosition()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Ray ray = mainCam.ScreenPointToRay(mouseScreenPosition);

        Plane gamePlane = new Plane(Vector3.forward, Vector3.zero);
        if (gamePlane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);
            return new Vector2(worldPoint.x, worldPoint.y);
        }
        return transform.position;
    }

    public void Movements(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        currentInput = context.ReadValue<Vector2>();
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        
        isSprinting = context.ReadValueAsButton();
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        CmdMovements(currentInput, isSprinting);

        HandleFacingDirection();
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer || isSprinting) return;
        
        if (context.performed) 
        {
            Vector2 mouseWorldPosition = GetMouseWorldPosition();
            CmdAttackAtPosition(mouseWorldPosition);
        }
    }
    
    private void HandleFacingDirection()
    {
        Vector2 mouseWorldPosition = GetMouseWorldPosition();

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
    private void CmdMovements(Vector2 direction, bool sprint)
    {
        playerController.Move(direction, sprint);
    }
    
    [Command]
    private void CmdChangeFacingDirection(bool facingRight)
    {
        playerController.SetFacing(facingRight);
    }
}