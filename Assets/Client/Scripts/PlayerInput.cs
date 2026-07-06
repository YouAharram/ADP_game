using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : NetworkBehaviour
{
    private CharacterController playerController;
    private Vector2 currentInput; 
    
    // Ci serve la telecamera per tradurre i pixel dello schermo in coordinate 2D
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
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        
        if (context.performed) 
        {
            // 1. Leggiamo i pixel del mouse
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

            // 2. Traduciamo i pixel nella posizione del mondo 2D
            Vector2 mouseWorldPosition = mainCam.ScreenToWorldPoint(mouseScreenPosition);

            // 3. Inviamo al server le coordinate in cui abbiamo cliccato
            CmdAttackAtPosition(mouseWorldPosition);
        }
    }

    [Command]
    private void CmdAttackAtPosition(Vector2 clickPosition)
    {
        // Chiamiamo il nuovo metodo del controller
        playerController.TryAttackTargetAt(clickPosition);
    }

    [Command]
    private void CmdMovements(Vector2 direction)
    {
        playerController.Move(direction);
    }
}