using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;

    private Animator animator;
    private SpriteRenderer sr;

    void Start()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        Vector2 movement = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) movement.y += 1;
        if (Keyboard.current.sKey.isPressed) movement.y -= 1;
        if (Keyboard.current.dKey.isPressed) movement.x += 1;
        if (Keyboard.current.aKey.isPressed) movement.x -= 1;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            animator.SetTrigger("Attack");
        }

        movement = movement.normalized;

        transform.Translate(movement * moveSpeed * Time.deltaTime);

        if (movement.x > 0)
            sr.flipX = false;
        else if (movement.x < 0)
            sr.flipX = true;

        animator.SetFloat("Speed", movement.magnitude);
    }
}