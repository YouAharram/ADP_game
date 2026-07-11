using UnityEngine;
using System.Collections;

public class AnimationObserver : MonoBehaviour
{
    [SerializeField] private GameObject deathVFXPrefab;

    private CharacterEntity characterEntity;
    private Animator animator;
    private SpriteRenderer sr;

    private Vector2 lastPosition;
    private Color originalColor;
    private Coroutine flashRoutine;

    void Start()
{
    animator = GetComponent<Animator>();
    sr = GetComponent<SpriteRenderer>();
    characterEntity = GetComponent<CharacterEntity>();

    if (sr != null)
    {
        originalColor = sr.color;
    }

    if (characterEntity != null)
    {
        lastPosition = characterEntity.GetPosition();

        // Sync iniziale esplicito: allinea subito lo sprite allo stato logico corrente,
        // senza aspettare un cambiamento futuro che potrebbe non arrivare mai.
        FlipToRight(characterEntity.IsFacingRight);

        characterEntity.OnAttackingClient += AnimationAttack;
        characterEntity.OnDamageClient += AnimationDamage;
        characterEntity.OnFlipToRightClient += FlipToRight;
    }
}

    private float checkInterval = 0.1f; // Controlla 10 volte al secondo
    private float nextCheckTime = 0f;

    private void Update()
    {
        if (Time.time < nextCheckTime) return;

        nextCheckTime = Time.time + checkInterval;
        UpdateMovementAnimation();
    }

    private void UpdateMovementAnimation()
    {
        if (animator == null || sr == null || characterEntity == null) return;

        Vector2 currentPosition = characterEntity.GetPosition();
        Vector2 delta = currentPosition - lastPosition;

        if (delta.sqrMagnitude < 0.0001f)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        animator.SetFloat("Speed", 1f);
        lastPosition = currentPosition;
    }

    private void FlipToRight(bool isFacingRight)
    {
        Debug.Log("FlipToRight chiamato in AnimationObserver");
        sr.flipX = !isFacingRight;
    }

    private void AnimationAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    private void AnimationDamage()
    {
        if (sr != null)
        {
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }
            flashRoutine = StartCoroutine(FlashRedEffect());
        }
    }

    private IEnumerator FlashRedEffect()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        if (sr != null)
        {
            sr.color = originalColor;
        }
        flashRoutine = null;
    }

    void OnDestroy()
    {
        if (characterEntity != null)
        {
            characterEntity.OnAttackingClient -= AnimationAttack;
            characterEntity.OnDamageClient -= AnimationDamage;
            characterEntity.OnFlipToRightClient -= FlipToRight;
        }
    }
}